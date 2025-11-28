using FitCoachPro.API.Endpoints;
using FitCoachPro.API.Exceptions;
using FitCoachPro.API.Extensions;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Infrastructure;
using FitCoachPro.Infrastructure.Persistence;
using FitCoachPro.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerWithAuth();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddAppAuthorizationPolicies();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    IdentitySeed.Seed(context);
    UserSeed.Seed(context);
    ExerciseSeed.Seed(context);
    WorkoutPlanSeed.Seed(context);
    WorkoutItemSeed.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapWorkoutPlanEndpoints();
app.MapExerciseEndpoints();

app.Run();
