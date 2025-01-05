using EventWise.Api;
using EventWise.Api.Events;
using EventWise.Api.Extensions;
using EventWise.Api.Users;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

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

builder.Services.AddFeatureManagement();

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
{
    var events = await dbContext.Events
        .AsNoTracking()
        .Where(e => e.EventState == EventState.Published && !(e is UserEvent))
        .Select(e => new EventResponse(e.Name, e.Description, e.Location, e.StartTimeUtc, e.EndTimeUtc))
        .ToListAsync(ct);

    return Results.Ok(new GetEventsResponse(events));
})
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

app.MapDelete("/events/{eventId}/participants", async (Guid eventId, UserContext userContext, ApplicationDbContext dbContext, CancellationToken ct) =>
{
    var userId = userContext.UserId();
    var @event = await dbContext.Events
        .Include(e => e.Participants
            .Where(p => p.ParticipantId == userId))
        .FirstOrDefaultAsync(e => e.Id == eventId, ct);
    if (@event is null)
    {
        return Results.NotFound();
    }

    var result = @event.Leave(userId);
    if (result.IsFailure)
    {
        return Results.BadRequest(result.Error);
    }

    await dbContext.SaveChangesAsync(ct);

    return Results.Ok();
})
//.AddEndpointFilter<LeaveEventFeatureFilter>()
.RequireAuthorization("User")
.WithTags("Events");

app.MapPost("/events/{eventId}/participants", async (Guid eventId, UserContext userContext, ApplicationDbContext dbContext, CancellationToken ct) =>
{
    var @event = await dbContext.Events
        .Include(e => e.Participants)
        .FirstOrDefaultAsync(e => e.Id == eventId, ct);
    if (@event is null)
    {
        return Results.NotFound();
    }

    var userId = userContext.UserId();
    var result = @event.Participate(userId);
    if (result.IsFailure)
    {
        return Results.BadRequest(result.Error);
    }

    await dbContext.SaveChangesAsync(ct);

    return Results.Ok();
})
.RequireAuthorization("User")
.WithTags("Events");

app.MapPost("/users", async ([FromBody] RegisterUserRequest request, ApplicationDbContext dbContext, CancellationToken ct) =>
{
    if (await dbContext.Users.AnyAsync(u => u.Id == request.Id || u.Email == request.Email, ct))
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

public sealed record GetEventsResponse(List<EventResponse> Events);
public sealed record EventResponse(
    string Name,
    string Description,
    string Location,
    DateTime StartTimeUtc,
    DateTime? EndTimeUtc);

public partial class Program;