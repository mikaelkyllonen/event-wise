using EventWise.Api;
using EventWise.Api.Events;
using EventWise.Api.Extensions;

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



app.Run();