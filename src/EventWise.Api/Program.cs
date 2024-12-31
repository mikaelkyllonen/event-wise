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

    await using var scope = app.Services.CreateAsyncScope();
    using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.EnsureCreatedAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapGet("/events", async (UserContext userContext, ApplicationDbContext dbContext, CancellationToken ct) =>
        await dbContext.Events
        .AsNoTracking()
        .Where(e => e.EventState == EventState.Published)
        .ToListAsync(ct))
.WithTags("Events");

app.MapPost("/events", async ([FromBody] CreateEventRequest request, UserContext userContext, ApplicationDbContext dbContext, CancellationToken ct) =>
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

    await dbContext.Events.AddAsync(result.Value, ct);
    await dbContext.SaveChangesAsync(ct);

    return Results.Created($"/events/{result.Value.Id}", new CreateEventResponse(result.Value.Id));
})
.RequireAuthorization("User")
.WithTags("Events");

app.MapGet("/events/{id}", async (Guid id, ApplicationDbContext dbContext, CancellationToken ct) =>
{
    var @event = await dbContext.Events.FindAsync([id], cancellationToken: ct);

    return @event is null
    ? Results.NotFound()
    : Results.Ok(
        new GetEventResponse(
            @event.Id,
            @event.Name,
            @event.Description,
            @event.Location,
            @event.EventState,
            @event.StartTimeUtc,
            @event.EndTimeUtc,
            @event.CreatedAtUtc));
})
.WithTags("Events");

app.MapPost("/users", async ([FromBody] RegisterUserRequest request, ApplicationDbContext dbContext, CancellationToken ct) =>
{
    if (await dbContext.Users.AnyAsync(u => u.Id == request.Id, ct))
    {
        return Results.Conflict();
    }

    var result = User.Create(request.Id, request.FirstName, request.LastName, request.Email);
    if (result.IsFailure)
    {
        return Results.BadRequest(result.Error);
    }

    await dbContext.Users.AddAsync(result.Value, ct);
    await dbContext.SaveChangesAsync(ct);

    return Results.Ok();
})
.RequireAuthorization("User") // TODO: Implement BFF Api key authorization
.WithTags("Users");

app.Run();

public sealed record RegisterUserRequest(Guid Id, string FirstName, string LastName, string Email);

public sealed record CreateEventRequest(
    string Name,
    string Description,
    string Location,
    int MaxParticipants,
    DateTime StartTime,
    DateTime EndTime);

public sealed record CreateEventResponse(Guid Id);

public sealed record GetEventResponse(
    Guid Id,
    string Name,
    string Description,
    string Location,
    EventState EventState,
    DateTime StartTime,
    DateTime? EndTime,
    DateTime CreatedAtUtc);

public partial class Program;