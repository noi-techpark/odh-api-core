using Helper;
using Helper.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OdhNotifier;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiImporter
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISettings, Settings>();
            services.AddScoped<QueryFactory, PostgresQueryFactory>();
            services.AddScoped<IOdhPushNotifier, OdhPushNotifier>();
            services.AddSingleton<IMongoDBFactory, MongoDBFactory>();

            //TODO CONFIGURATION for Keycloak

            services.AddLogging(options =>
            {
                options.ClearProviders();

                var levelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel =
                        CurrentEnvironment.IsDevelopment() ?
                            LogEventLevel.Debug:
                            LogEventLevel.Warning
                };
                var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "{Message}{NewLine}")
                    .WriteTo.Debug()
                    //.WriteTo.Elasticsearch(
                    //    new ElasticsearchSinkOptions() {
                    //        AutoRegisterTemplate = true,
                    //        IndexFormat = "odh-tourism-{0:yyyy.MM}",
                    //        //ModifyConnectionSettings = (c) => c.GlobalHeaders(new NameValueCollection { { "Authorization", "Basic " + loggerconfig.elkbasicauthtoken } }),
                    //        FailureCallback = e => System.Console.Error.WriteLine("Unable to submit event " + e.MessageTemplate),
                    //        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                    //                           EmitEventFailureHandling.WriteToFailureSink |
                    //                           EmitEventFailureHandling.RaiseCallback,
                    //        //FailureSink = new FileSink(loggerconfig.filepathfailures, new JsonFormatter(), null),
                    //        MinimumLogEventLevel = LogEventLevel.Information
                    //    }
                    //)
                    .CreateLogger();
                options.AddSerilog(loggerConfiguration, dispose: true);

                // Configure Serilogs own configuration to use
                // the configured logger configuration.
                // This allows to Log via Serilog's Log and ILogger.
                Log.Logger = loggerConfiguration;
            });

            //Initialize JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(jwtBearerOptions =>
                    {
                        jwtBearerOptions.Authority = Configuration.GetSection("OauthServerConfig").GetValue<string>("Authority");
                        //jwtBearerOptions.Audience = "account";                
                        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "preferred_username",
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidIssuer = Configuration.GetSection("OauthServerConfig").GetValue<string>("Authority"),
                            ValidateIssuer = true
                        };
                        jwtBearerOptions.Events = new JwtBearerEvents()
                        {
                            OnAuthenticationFailed = c =>
                            {
                                c.NoResult();
                                c.Response.StatusCode = 401;
                                c.Response.ContentType = "text/plain";

                                //Generate Log
                                //HttpRequestExtensions.GenerateLogResponse(c.HttpContext);

                                return c.Response.WriteAsync("");                                
                            },                            
                        };
                    });

            services.AddMvc();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();

            //CONFIGURATION for Using Authentication and Authorization
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();                       

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World");
                //});
                //endpoints.MapPost("/import", async context =>
                //{
                //    await context.Response.WriteAsync("Importing tourism data...");
                //        // call import
                //});
                endpoints.MapControllers();
            });
        }
    }
}
