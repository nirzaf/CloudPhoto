﻿namespace CloudPhoto.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Common;
    using Data;
    using CloudPhoto.Data.Common;
    using CloudPhoto.Data.Common.Repositories;
    using Data.Models;
    using Data.Repositories;
    using Data.Seeding;
    using CloudPhoto.Services.Data;
    using CloudPhoto.Services.Data.BackgroundServices;
    using CloudPhoto.Services.Data.BackgroundServices.BackgroundQueue;
    using CloudPhoto.Services.Data.BackgroundServices.ImageHelper;
    using CloudPhoto.Services.Data.CategoriesService;
    using CloudPhoto.Services.Data.DapperService;
    using CloudPhoto.Services.Data.ImagiesService;
    using CloudPhoto.Services.Data.SubscribesService;
    using CloudPhoto.Services.Data.TagsService;
    using CloudPhoto.Services.Data.TempCloudImageService;
    using CloudPhoto.Services.Data.UsersServices;
    using CloudPhoto.Services.Data.VotesService;
    using Services.ImageManipulationProvider;
    using Services.ImageValidate;
    using Services.LocalStorage;
    using Services.Mapping;
    using Services.Messaging;
    using Services.RemoteStorage;
    using ViewModels;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                IConfigurationSection facebookAuthNSection =
                configuration.GetSection("Authentication:Facebook");

                facebookOptions.AppId = facebookAuthNSection["AppId"];
                facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
                facebookOptions.Fields.Add("picture");
                facebookOptions.Events.OnCreatingTicket = ctx =>
                {
                    List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                    var identity = (ClaimsIdentity)ctx.Principal.Identity;

                    var profileImg = ctx.User.GetProperty("picture").GetProperty("data").GetProperty("url").ToString();
                    ctx.Identity.AddClaim(new Claim(GlobalConstants.ExternalClaimAvatar, profileImg));

                    tokens.Add(new AuthenticationToken()
                    {
                        Name = "TicketCreated",
                        Value = DateTime.UtcNow.ToString(),
                    });

                    ctx.Properties.StoreTokens(tokens);

                    return Task.CompletedTask;
                };
            });

            services.AddAuthentication().AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection =
                configuration.GetSection("Authentication:Google");

                // Provide the Google Client ID
                options.ClientId = googleAuthNSection["ClientId"];

                // Provide the Google Client Secret
                options.ClientSecret = googleAuthNSection["ClientSecret"];

                options.ClaimActions.MapJsonKey(GlobalConstants.ExternalClaimAvatar, "picture", "url");
                options.SaveTokens = true;

                options.Events.OnCreatingTicket = ctx =>
                {
                    List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                    tokens.Add(new AuthenticationToken()
                    {
                        Name = "TicketCreated",
                        Value = DateTime.UtcNow.ToString(),
                    });

                    ctx.Properties.StoreTokens(tokens);

                    return Task.CompletedTask;
                };
            });

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddControllersWithViews(
                options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }).AddRazorRuntimeCompilation();

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSingleton(configuration);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            services.AddTransient<ICategoriesService, CategoriesService>();
            services.AddTransient<IRemoteStorageService, CloudinaryStorageServer>();
            services.AddTransient<IImageValidatorService, ImageValidator>();
            services.AddTransient<ILocalStorageServices, LocalStorage>();
            services.AddTransient<IImagesService, ImagesService>();
            services.AddTransient<ITagsService, TagsService>();
            services.AddTransient<IVotesService, VotesService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IImageManipulationProvider, SkiaSharpImageManipulationProvider>();
            services.AddTransient<IDapperService, DapperService>();
            services.AddTransient<ITempCloudImagesService, TempCloudImagesService>();
            services.AddTransient<ISubscribesService, SubscribesService>();

            services
                .AddHostedService<BackgroundWorker>()
                .AddSingleton<IBackgroundQueue<ImageInfoParams>, BackgroundQueue<ImageInfoParams>>();
            services.AddScoped<IImageHelper, ImageHelper>();

            // Application services
            services.AddTransient<IEmailSender, NullMessageSender>();
            services.AddTransient<ISettingsService, SettingsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                /* For test purpose
                 * When run web test use memory database provider
                 * which not allow migrate data
                 */
                if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                {
                    dbContext.Database.Migrate();
                    new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapRazorPages();
                    });
        }
    }
}
