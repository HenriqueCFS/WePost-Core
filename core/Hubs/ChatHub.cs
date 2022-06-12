using core.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Collections.Generic;
using static core.Controllers.FriendsController;
using Microsoft.EntityFrameworkCore;

namespace core.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public readonly ProjectContext context;
        public readonly AppUser botUser;
        public readonly UserManager<AppUser> userManager;
        public readonly IDictionary<string, UserConnection> connections;
        public ChatHub(UserManager<AppUser> userManager, IDictionary<string, UserConnection> connections, ProjectContext context)
        {
            this.context = context;
            this.userManager = userManager;
            this.connections = connections;
            this.botUser = new AppUser()
            {
                Name = "Saber",
                Id="-1",
                UserName = "Saber"
            };
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection)) {
                connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", botUser.UserName, $"{userConnection.UserName} has left");
                SendConnectedUsers(userConnection.Room);
            }
            return base.OnDisconnectedAsync(exception);
        }

        [Authorize(Policy = "IsUser")]
        public async Task SendMessage(string? message = null, string? files = null)
        {
            AppUser currentUser = await userManager.FindByNameAsync(Context.User.Identity.Name);
            string friendUserName = connections.Where(c => c.Value.UserName == currentUser.UserName)?.FirstOrDefault().Value.FriendName;
            AppUser friendUser = await userManager.FindByNameAsync(friendUserName);
            if (friendUser == null) return;
            ChatMessage newMessage = new()
            {
                Sender = currentUser,
                Receiver = friendUser,
                SenderId = currentUser?.Id,
                ReceiverId = friendUser?.Id,
                Message = message,
                Files = files
            };
            context.ChatMessages.Add(newMessage);
            try
            {
                await context.SaveChangesAsync();
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            if (connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", userConnection.UserName, new { newMessage.Id, Sender = newMessage.Sender.UserName, Receiver = newMessage.Receiver.UserName,  newMessage.SeenAt, newMessage.SentAt, newMessage.Message, files });
            }
        }

        [Authorize(Policy = "IsUser")]
        public async Task JoinRoom(string? friendUsername)
        {
            AppUser currentUser = await userManager.FindByNameAsync(Context.User.Identity.Name);
            ICollection<Friend> myFriends = currentUser.Friends;
            List<string> errors;
            string friendshipId;
            if (friendUsername != null)
            {
                AppUser friendUser = await userManager.FindByNameAsync(friendUsername);
                friendshipId = context.Friends
                .Where(f => f.RequestedById == currentUser.Id && f.RequestedToId == friendUser.Id
                         || f.RequestedToId == currentUser.Id && f.RequestedById == friendUser.Id)
                .Select(f => $"{f.RequestedById}<->{f.RequestedToId}")
                .FirstOrDefault();
            }            
            else friendshipId = $"{currentUser?.Id} -> PrivateRoom";
            if (currentUser == null ) return;
            await Groups.AddToGroupAsync(Context.ConnectionId, friendshipId);
            UserConnection userConnection = new UserConnection()
            {
                UserName =  currentUser.UserName,
                FriendName = friendUsername,
                Room =  friendshipId
            };

            connections[Context.ConnectionId] = userConnection;
            await Clients.Group(friendshipId).SendAsync("ReceiveMessage", botUser.UserName,
                $"{currentUser.UserName} has joined {friendshipId}");
            await SendConnectedUsers(friendshipId);
            await Clients.Client(Context.ConnectionId).SendAsync("ConfirmRoomJoin", botUser);

        }


        [Authorize(Policy = "IsUser")]
        public Task SendConnectedUsers(string roomId)
        {
            IEnumerable<string> users = connections.Values.Where(c => c.Room == roomId).Select(c => c.UserName);
            return Clients.Group(roomId).SendAsync("UsersInRoom", users);
        }


        [Authorize(Policy = "IsUser")]
        public async Task<Task> SendOnlineFriends()
        {
            AppUser currentUser = await userManager.FindByNameAsync(Context.User.Identity.Name);

            if (context.Friends == null)
            {
                return Clients.Client(Context.ConnectionId).SendAsync("OnlineFriends", new List<AnyType>());
            }
            IEnumerable<string> users = connections.Values.Where(c => c.UserName != Context.User.Identity.Name).Select(c => c.UserName);
            var friendList = await context.Friends.Where(friend => (friend.RequestedById == currentUser.Id || friend.RequestedToId == currentUser.Id) && (friend.FriendRequestFlag == FriendRequestFlag.Approved || friend.FriendRequestFlag == FriendRequestFlag.None))
                .Select(friend => new { friend.RequestedById, friend.RequestedToId, friend.FriendRequestFlag, RequestedBy = new UserContactInfo() { Id = friend.RequestedBy.Id, UserName = friend.RequestedBy.UserName, Email = friend.RequestedBy.Email }, RequestedTo = new UserContactInfo() { Id = friend.RequestedTo.Id, UserName = friend.RequestedTo.UserName, Email = friend.RequestedTo.Email } })
                .ToListAsync();
            return Clients
                .Client(Context.ConnectionId)
                .SendAsync("OnlineFriends", users.Where(f => friendList.Select(fl => fl.RequestedBy.UserName).Contains(f) || friendList.Select(fl => fl.RequestedTo.UserName).Contains(f)));
        }



        [Authorize(Policy = "IsUser")]
        public async Task<Task> MessageSeen(int messageId)
        {
            AppUser currentUser = await userManager.FindByNameAsync(Context.User.Identity.Name);
            ChatMessage seenMessage = context.ChatMessages.Where(m => m.Id == messageId).FirstOrDefault();
            if (currentUser == null || seenMessage == null || seenMessage.ReceiverId != currentUser.Id || seenMessage.SeenAt != null)
            {
                return Clients
                .Client(Context.ConnectionId)
                .SendAsync("ServerError", "Could not mark message as seen");
            }
            seenMessage.SeenAt = DateTime.UtcNow;            await context.SaveChangesAsync();

            if (connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                return Clients
                       .Group(userConnection.Room)
                    .SendAsync("MessageSeenConfirmation", new { Id = seenMessage.Id, SenderId = seenMessage.SenderId, SeenAt = seenMessage.SeenAt });
            } 
            return Clients
            .Client(Context.ConnectionId)
            .SendAsync("ServerError", "Could not mark message as seen");
            
        }



        [Authorize(Policy = "IsUser")]
        public async Task<Task> FetchLastMessages(int count = 15)
        {
            AppUser currentUser = await userManager.FindByNameAsync(Context.User.Identity.Name);
            string friendUserName = connections.Where(c => c.Value.UserName == currentUser.UserName)?.FirstOrDefault().Value.FriendName;
            AppUser friendUser = await userManager.FindByNameAsync(friendUserName);
            var messages = context
                .ChatMessages
                .Where(cm => cm.Receiver.UserName == friendUserName && cm.Sender.UserName == currentUser.UserName
                    || cm.Sender.UserName == friendUserName && cm.Receiver.UserName == currentUser.UserName)
                .Select(ChatMessage=> new {ChatMessage.Id, Sender = ChatMessage.Sender.UserName, Receiver = ChatMessage.Receiver.UserName, ChatMessage.Message, ChatMessage.SentAt, ChatMessage.SeenAt})
                .OrderBy(msg => msg.SentAt)
                .Take(count)
                .ToList();
            Console.WriteLine(messages);
            return Clients
            .Client(Context.ConnectionId)
            .SendAsync("FetchMessages", messages);

        }

    }
}
