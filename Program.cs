using Bangumi.Graph;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services
    .AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("AppDb"),
            opts => opts.UseQuerySplittingBehavior(
                builder.Configuration.GetValue<bool>("UseSingleQuery")
                    ? QuerySplittingBehavior.SingleQuery
                    : QuerySplittingBehavior.SplitQuery));
    });

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<AppDbContext>()
    .AddQueryType<Query>()
    .AddAuthorization()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .ModifyRequestOptions(opt =>
    {
        opt.IncludeExceptionDetails = builder.Environment.IsDevelopment();
        opt.Complexity.Enable = true;
        opt.Complexity.ApplyDefaults = true;
        opt.Complexity.MaximumAllowed = 1500;
    });


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();
app.MapGraphQLWebSocket();
app.MapControllers();

var command =
    app.Configuration.GetValue("command",
        app.Configuration.GetValue<string>("c"));

if (string.IsNullOrEmpty(command))
{
    await app.RunAsync();
}
else if (command == "seed")
{
    await SeedData.SeedDb(app.Services);
}
