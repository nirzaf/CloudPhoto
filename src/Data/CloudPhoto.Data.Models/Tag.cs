﻿namespace CloudPhoto.Data.Models
{
    using System;
    using System.Collections.Generic;

    using CloudPhoto.Data.Common.Models;

    public class Tag : BaseDeletableModel<string>
    {
        public Tag()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<ImageTag> ImageTags { get; set; }
    }
}
