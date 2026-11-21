namespace TicTacToe.Server;

using Microsoft.AspNetCore.Authentication;
using TicTacToe.Server.DI;
using TicTacToe.Server.Web.Authentication;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();

        builder.Services.AddAuthentication("Basic").AddScheme<AuthenticationSchemeOptions, UserAuthenticator>("Basic", null);
        builder.Services.AddAuthorization();

        var corsOrigins = builder.Configuration["CorsOrigins"] ?? "";
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod();
            });
        });

        Configuration.Configure(builder.Services, builder.Configuration);

        var app = builder.Build();

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.MapHealthChecks("/health");
        app.Run();
    }
}