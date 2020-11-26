﻿namespace CloudPhoto.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Common;
    using CloudPhoto.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal class TagSeeder : ISeeder
    {
        public TagSeeder(ILogger logger)
        {
            this.Logger = logger;
        }

        public ILogger Logger { get; }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Tags.Any())
            {
                return;
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            ApplicationUser admin = await userManager.FindByEmailAsync(GlobalConstants.DefaultAdministratorEmail);
            if (admin == null)
            {
                this.Logger.LogError($"Not found administartor with email: {GlobalConstants.DefaultAdministratorEmail}");
                return;
            }

            List<Tag> lstTags = new List<Tag>();

            lstTags.Add(new Tag()
            {
                Name = "Cat",
                Description = "Cat",
                AuthorId = admin.Id,
            });

            lstTags.Add(new Tag()
            {
                Name = "Dog",
                Description = "Dog",
                AuthorId = admin.Id,
            });

            lstTags.Add(new Tag()
            {
                Name = "Mouse",
                Description = "Mouse",
                AuthorId = admin.Id,
            });

            lstTags.Add(new Tag()
            {
                Name = "Nature",
                Description = "Nature",
                AuthorId = admin.Id,
            });

            lstTags.Add(new Tag()
            {
                Name = "Bear",
                Description = "Bear",
                AuthorId = admin.Id,
            });
            await dbContext.Tags.AddRangeAsync(lstTags);
        }
    }
}
