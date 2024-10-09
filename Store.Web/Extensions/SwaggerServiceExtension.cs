using Microsoft.OpenApi.Models;
using System.Reflection.Metadata.Ecma335;

namespace Store.Web.Extensions
{
    public static class SwaggerServiceExtension
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc
                    ("v1",
                    new OpenApiInfo 
                    {
                        Title = "Store Api",
                        Version = "v1",
                        Contact= new OpenApiContact
                        {
                            Name="Route Academy",
                            Email="route.@gmail.com",
                            Url= new Uri("https://twitter.com/jwalkner")
                        }
                    });
                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Description = "JWT Authentication header using the Bearer scheme.Example:\"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "bearer",
                        Reference = new OpenApiReference
                        { 
                            Id="bearer",
                            Type=ReferenceType.SecurityScheme
                        }

                    };

                    options.AddSecurityDefinition("bearer", securityScheme);
                    var securityRequirements = new OpenApiSecurityRequirement
                    {
                        { securityScheme,new[]{"bearer"} }
                    };

                    options.AddSecurityRequirement(securityRequirements);
                });
            return services;
        }
    }
}
