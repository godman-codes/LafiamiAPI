using Lafiami.Models.Internals;
using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Middlewares;
using LafiamiAPI.Models;
using LafiamiAPI.Models.Internals;
using LafiamiAPI.Services;
using LafiamiAPI.Services.BackgroundServices;
using LafiamiAPI.Services.UnitofWorkServices;
using LafiamiAPI.Utilities.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LafiamiAPI
{
    public class Startup
    {
        private readonly ILogger _logger;
        public Startup(ILogger<Startup> logger
                      , IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
               options =>
               {
                   options.AddPolicy("CorsPolicy",
                       builder => builder.SetIsOriginAllowed(_ => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       //.AllowAnyOrigin()
                       .AllowCredentials());
               }
           );
            services.AddResponseCompression();
            services.Configure<IISOptions>(options => { });

            services.AddDbContext<LafiamiContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LafiamiConnection")));

            services.AddIdentity<UserViewModel, ApplicationRoleModel>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
                config.User.RequireUniqueEmail = true;
                config.Password.RequireLowercase = true;
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequireUppercase = true;
                config.Password.RequiredLength = 8;
                config.Password.RequireDigit = true;
                config.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            })
            .AddEntityFrameworkStores<LafiamiContext>()
            .AddDefaultTokenProviders();


            services.AddScoped<IBusinessUnitofWork, BusinessUnitofWork>();
            services.AddScoped<ISystemUnitofWork, SystemUnitofWork>();
            services.AddScoped<IWebAPI, WebAPIService>();

            services.AddHostedService<LafiamiBackgroundService>();

            services.AddMemoryCache();
            services.AddResponseCompression();
            services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            // configure strongly typed settings objects
            IConfigurationSection jwtSettingsSection = Configuration.GetSection("JWTSettings");

            services.Configure<JWTSettings>(jwtSettingsSection);
            services.Configure<OrderSettings>(Configuration.GetSection("OrderSettings"));
            services.Configure<FlutterwaveSettings>(Configuration.GetSection("FlutterwaveSettings"));
            services.Configure<PaystackSettings>(Configuration.GetSection("PaystackSettings"));
            services.Configure<PaymentSettings>(Configuration.GetSection("PaymentSettings"));
            services.Configure<SMTPSettings>(Configuration.GetSection("SMTPSettings"));
            services.Configure<WebsiteSettings>(Configuration.GetSection("WebsiteSettings"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));


            // configure jwt authentication
            JWTSettings jwtSettings = jwtSettingsSection.Get<JWTSettings>();
            TokenManagement tokenManagement = new TokenManagement(jwtSettings);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(tokenManagement.ValidateToken());


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Lafiami API",
                    Description = "Insurance repositories system",
                    TermsOfService = new Uri("https://www.lafiami.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Samuel Adekoya",
                        Email = string.Empty,
                        Url = new Uri("https://igunle.com"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://www.lafiami.com/license"),
                    }
                });

                c.AddSecurityDefinition(Constants.Bearer, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    Name = Constants.Authorization,
                    Type = SecuritySchemeType.ApiKey,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = Constants.Bearer}
                        },
                        new List<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AppDomain.CurrentDomain.SetData(Constants.WebRootPath, env.WebRootPath);
            AppDomain.CurrentDomain.SetData(Constants.ContentRootPath, env.ContentRootPath);

            app.UseGlobalExceptionHandler(_logger, respondWithJsonErrorDetails: true);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lafiami API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseStaticFiles();
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
