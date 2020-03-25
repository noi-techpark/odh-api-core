using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OdhApiCore.Controllers;
using System.Linq;
using System.Text;

namespace OdhApiCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

            services.AddResponseCompression();

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddControllers().AddNewtonsoftJson();
            services.AddRazorPages();

            services.AddSingleton<ISettings, Settings>();
            services.AddSingleton<Helper.IPostGreSQLConnectionFactory, PostGreSQLConnectionFactory>();

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
                jwtBearerOptions.Authority = "https://auth.opendatahub.testingmachine.eu/auth/realms/noi/";
                jwtBearerOptions.Audience = "account";
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
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
            });

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OdhApi .Net Core", Version = "v1" });
                //c.IncludeXmlComments(filePath);
                c.MapType<LegacyBool>(() => new OpenApiSchema
                {
                    Type = "boolean"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {            
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
            });

            app.UseRouting();

            //app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseResponseCompression();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");                
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
