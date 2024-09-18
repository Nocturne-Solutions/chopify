using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace chopify.Configurations
{
    public static class JwtConfig
    {
        public static IServiceCollection ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSecretKey = configuration.GetValue<string>("JWT_SECRET_KEY");

            if (string.IsNullOrWhiteSpace(jwtSecretKey))
                throw new ArgumentNullException(nameof(jwtSecretKey), "JWT secret key is not configured.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "chopify.com.ar",
                    ValidAudience = "chopify.com.ar",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
                };
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var firstError = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Where(e => !string.IsNullOrEmpty(e.ErrorMessage))
                        .Select(e => e.ErrorMessage)
                        .LastOrDefault();

                    var errorResponse = new
                    {
                        error = firstError ?? "El modelo proporcionado no es válido."
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}
