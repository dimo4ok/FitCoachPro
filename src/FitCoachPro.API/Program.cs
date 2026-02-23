using FitCoachPro.API.Endpoints;
using FitCoachPro.API.Exceptions;
using FitCoachPro.API.Extensions;
using FitCoachPro.Application.Common.ServiceRegistration;
using FitCoachPro.Infrastructure;
using FitCoachPro.Infrastructure.Persistence;
using FitCoachPro.Infrastructure.Persistence.Seed.Users;
using FitCoachPro.Infrastructure.Persistence.Seed.Wokrouts;
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
    //context.Database.Migrate();

    IdentitySeed.Seed(context);
    UserSeed.Seed(context);
    ExerciseSeed.Seed(context);
    WorkoutPlanSeed.Seed(context);
    WorkoutItemSeed.Seed(context);
    TemplateWorkoutPlanSeed.Seed(context);
    TemplateWorkoutItemSeed.Seed(context);
}

// Configure the HTTP request pipeline.
app.UseSwagger();

if (app.Environment.IsProduction())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FitCoachPro API V1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapAdminEndpoints();
app.MapCoachEndpoints();
app.MapClientEndpoints();
app.MapCoachClientRequestEndpoints();

app.MapTempalteWorkoutPlanEndpoints();
app.MapWorkoutPlanEndpoints();
app.MapExerciseEndpoints();

app.Run();