using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using core.Data;
using core.Data.Models;

namespace core.Data.Utilities;

public static class UserUtilities
{
    public static async Task<User?> GetSecurityCurrentUser(this ProjectContext context,
        IHttpContextAccessor contextAccessor)
    {
        var email = contextAccessor.HttpContext?.User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
        if (email == null) return null;
        var user = await context.GetSecurityUser(email);
        return user ?? null;
    }

    public static async Task<User?> GetSecurityUser(this ProjectContext context, string? email)
    {
        if (email == null) return null;
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
        return user ?? null;
    }
}