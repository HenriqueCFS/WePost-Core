using Microsoft.EntityFrameworkCore;
using core.Data;
using core.Data.GraphQL;
using core.Controllers.Errors.GraphQL;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProjectContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("ProjectContext")));
builder.Services
    .AddGraphQLServer()
    .AddMutationConventions(applyToAllMutations: true)
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();
builder.Services.AddErrorFilter<GraphQLErrorFilter>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ProjectContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

app.Run();
