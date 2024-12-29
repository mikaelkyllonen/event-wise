using EventWise.Api;
using EventWise.Api.Events;
using EventWise.Api.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("User", policy => policy.RequireRole("user"));

builder.AddAuthentication();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserContext>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapGet("/events", async (UserContext userContext, ApplicationDbContext dbContext) => 
    await dbContext.Events
        .Where(e => e.EventState == EventState.Published)
        .ToListAsync())
.RequireAuthorization("User")
.WithTags("Events");

app.MapPost("/events", async ([FromBody] CreateEventRequest request, UserContext userContext, ApplicationDbContext dbContext) =>
{
    var userId = userContext.UserId();
    var result = UserEvent.Create(
        userId,
        request.Name,
        request.Description,
        request.Location,
        request.MaxParticipants,
        request.StartTime,
        request.EndTime);
    if (result.IsFailure)
    {
        return Results.BadRequest(result.Error);
    }

    await dbContext.Events.AddAsync(result.Value);
    await dbContext.SaveChangesAsync();

    return Results.Ok(result.Value);
})
.RequireAuthorization("User")
.WithTags("Events");

app.Run();

public sealed record class CreateEventRequest(
    string Name,
    string Description,
    string Location,
    int MaxParticipants,
    DateTime StartTime,
    DateTime EndTime);