using AspNetCore.CacheOutput.Extensions;
using AspNetCore.CacheOutput.InMemory.Extensions;
using Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Npgsql;
using OdhApiCore.Controllers;
using OdhApiCore.Factories;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using SqlKata.Compilers;
using SqlKata.Execution;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace OdhApiCore
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddHttpClient("mss", client =>
            {
                //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
            }).ConfigureHttpMessageHandlerBuilder(config => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            services.AddHttpClient("lcs"); // TODO: put LCS config here

            services.AddInMemoryCacheOutput();
            services.AddSingleton<CustomCacheKeyGenerator>();

            services.AddLogging(options =>
            {
                options.ClearProviders();

                var levelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel =
                        CurrentEnvironment.IsDevelopment() ?
                            LogEventLevel.Debug :
                            LogEventLevel.Information
                };
                var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
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

            services.AddResponseCompression();

            services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .SetIsOriginAllowed(hostName => true);
                    //AllowAnyOrigin()
                    //builder.SetIsOriginAllowed(_ => true) //Hack
                });
                o.AddPolicy("DataBrowserCorsPolicy", builder =>
                {
                    builder.WithOrigins("https://localhost:6001")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials();
                });
            });
            
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddRazorPages();

            services.AddSingleton<ISettings, Settings>();
            services.AddScoped<QueryFactory, PostgresQueryFactory>();

            //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var filePath = Path.Combine(System.AppContext.BaseDirectory, xmlFile);

            //JWT on .Net 2
            //services.AddAuthentication("Bearer")
            //  .AddJwtBearer("Bearer", options =>
            //  {
            //      options.Authority = "https://auth.testingmachine.eu";
            //      options.RequireHttpsMetadata = false;

            //      options.Audience = "api1";
            //  });

            //services.AddAuthentication("Bearer")
            //  .AddJwtBearer("Bearer", options =>
            //  {
            //      options.Authority = "https://auth.opendatahub.testingmachine.eu/auth/realms/noi";
            //      options.RequireHttpsMetadata = false;

            //      options.Audience = "account";
            //  });

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
                        ValidateAudience = false
                    };
                    //jwtBearerOptions.Events = new JwtBearerEvents()
                    //{
                    //    OnAuthenticationFailed = c =>
                    //    {
                    //        c.NoResult();

                    //        c.Response.StatusCode = 500;
                    //        c.Response.ContentType = "text/plain";
                    //        //if (env.IsDevelopment())
                    //        //{
                    //        //    return c.Response.WriteAsync(c.Exception.ToString());
                    //        //}
                    //        //return c.Response.WriteAsync("An error occured processing your authentication.");
                    //        return c.Response.WriteAsync(c.Exception.ToString());
                    //    }
                    //};
                });

            services.AddMvc(options =>
                {
                    options.OutputFormatters.Add(new Formatters.CsvOutputFormatter());
                    options.FormatterMappings.SetMediaTypeMappingForFormat("csv", "text/csv");
                });
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.PropertyNameCaseInsensitive = new DefaultContractResolver();
                //});

            //TO TEST
            // Here I stored necessary permissions/roles in a constant
            //services.AddAuthorization(options =>
            //{
            //    // Here I stored necessary permissions/roles in a constant
            //    foreach (var prop in typeof(ClaimPermission).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            //    {
            //        options.AddPolicy(prop.GetValue(null).ToString(), policy => policy.RequireClaim(ClaimType.Permission, prop.GetValue(null).ToString()));
            //    }
            //});


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("DataReader", policy => policy.RequireClaim("roles", "[DataReader]"));
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "OdhApi Tourism .Net Core", 
                    Version = "v1",
                    Description = "ODH Tourism Api based on .Net Core with PostgreSQL",
                    TermsOfService = new System.Uri("https://opendatahub.readthedocs.io/en/latest/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Open Data Hub Team",
                        Email = "help@opendatahub.bz.it",
                        Url = new System.Uri("https://opendatahub.bz.it/"),
                    },
                });
                //c.IncludeXmlComments(filePath);
                c.MapType<LegacyBool>(() => new OpenApiSchema
                {
                    Type = "boolean"
                });

                c.MapType<PageSize>(() => new OpenApiSchema
                {
                    Type = "integer"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                           TokenUrl = new Uri(Configuration.GetSection("OauthServerConfig").GetValue<string>("Authority") + "protocol/openid-connect/token")
                        }                        
                    },
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.OperationFilter<AuthenticationRequirementsOperationFilter>();
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "bearer"
                //            }
                //        }, new List<string>()
                //    }
                //});
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            //services.AddHttpContextAccessor();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                //ForwardedHeaders = ForwardedHeaders.XForwardedProto
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                },
                //FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                //RequestPath = new PathString("")
                //FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "wwwroot")), RequestPath = "/StaticFiles" 
            });

            //app.UseSerilogRequestLogging(); throwing exception
 
            app.UseRouting();

            //app.UseCookiePolicy();

            //Important! Register Cors Policz before Using Authentication and Authorization
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCompression();

            // Put app.UseCacheOutput() before app.UseMvc()
            app.UseCacheOutput();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                });
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ODH Tourism API V1");
                c.RoutePrefix = "swagger";
                c.OAuthClientSecret("");
                c.OAuthRealm("noi");
            });

           
            //LOG EVERY REQUEST WITH HEADERs
            app.Use(async (context, next) =>
            {
                //TODO If Root is requested forward to Databrowser (Compatibility reason)
                if(String.IsNullOrEmpty(context.Request.Path.Value) || context.Request.Path.Value == "/")
                {                          
                    context.Response.Redirect(Configuration.GetSection("DataBrowserConfig").GetValue<string>("Url"));                                     
                    return;
                }

                //Log only if api is requested!
                //if(context.Request.Path.StartsWithSegments("/v1/", StringComparison.OrdinalIgnoreCase))
                if (!String.IsNullOrEmpty(context.Request.Path.Value) && context.Request.Path.Value.StartsWith("/v1/", StringComparison.OrdinalIgnoreCase))
                {
                    //TODO IF THE REFERER IS NOT PROVIDED IN THE HEADERS SEARCH IF A QS IS THERE
                    var referer = "not provided";

                    if (context.Request.Headers.ContainsKey("Referer"))
                        referer = context.Request.Headers["Referer"].ToString();
                    else
                    {
                        //Search the QS for Referer
                        if (context.Request.Query.ContainsKey("Referer"))
                            referer = context.Request.Query["Referer"].ToString();
                    }

                    //User Agent
                    var useragent = "not provided";
                    if (context.Request.Headers.ContainsKey("User-Agent"))
                        useragent = context.Request.Headers["User-Agent"].ToString();
                   
                    var urlparameters = context.Request.QueryString.Value != null ? context.Request.QueryString.HasValue ? context.Request.QueryString.Value.Replace("?", "") : "" : "";

                    HttpRequestLog httplog = new HttpRequestLog()
                    {
                        host = context.Request.Host.ToString(),
                        path = context.Request.Path.ToString(),
                        urlparams = urlparameters, //.Replace("&", "-"),  //Helper.StringHelpers.GenerateDictionaryFromQuerystring(context.Request.QueryString.ToString()),
                        referer = referer,
                        schema = context.Request.Scheme,
                        useragent = useragent,
                        username = context.User.Identity != null ? context.User.Identity.Name != null ? context.User.Identity.Name.ToString() : "anonymous" : "anonymous",
                        ipaddress = context.Request.HttpContext.Connection.RemoteIpAddress != null ? context.Request.HttpContext.Connection.RemoteIpAddress.ToString() : ""
                    };
                    LogOutput<HttpRequestLog> logoutput = new LogOutput<HttpRequestLog>() { id = "", type = "HttpRequest", log = "apiaccess", output = httplog };

                    string output = JsonConvert.SerializeObject(logoutput);

                    Console.WriteLine(JsonConvert.SerializeObject(logoutput));

                    //Log.Information(output);
                }

                await next();
            });

            //REWRITE, REDIRECT RULES
            //var rwoptions = new RewriteOptions()
            //    .AddRedirect("api/(.*)", "v1/$1");
            //.AddRedirectToHttpsPermanent();
            //.AddRedirect("redirect-rule/(.*)", "redirected/$1")
            //.AddRewrite(@"^rewrite-rule/(\d+)/(\d+)", "rewritten?var1=$1&var2=$2",
            //skipRemainingRules: true)

            app.UseRewriter(
                new RewriteOptions()
                .AddRedirect("api/(.*)", "v1/$1")
                //.AddRewrite(@"^(?=/api)", "/v1", skipRemainingRules: true)
                );

            //Not needed at moment
            //app.UseHttpContext();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapDefaultControllerRoute();
            });
        }
    }

    public class AuthenticationRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Security == null)
                operation.Security = new List<OpenApiSecurityRequirement>();


            var scheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" } };
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [scheme] = new List<string>()
            });
        }
    }

}
