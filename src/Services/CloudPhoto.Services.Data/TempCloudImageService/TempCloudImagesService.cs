﻿namespace CloudPhoto.Services.Data.TempCloudImageService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using Mapping;

    public class TempCloudImagesService : ITempCloudImagesService
    {
        public TempCloudImagesService(
            IRepository<TempCloudImage> repository)
        {
            Repository = repository;
        }

        public IRepository<TempCloudImage> Repository { get; }

        public async Task<string> CreateAsync(TempCloudImage newCloudImage)
        {
            await Repository.AddAsync(newCloudImage);
            await Repository.SaveChangesAsync();
            return newCloudImage.Id;
        }

        public IEnumerable<T> GetByImageId<T>(string imageId)
        {
            var query =
                  Repository.All()
                  .Where(c => c.ImageId == imageId);
            return query.To<T>().ToList();
        }
    }
}
