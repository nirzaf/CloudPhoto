﻿namespace CloudPhoto.Web.ViewComponents
{
    using System.Collections.Generic;

    using CloudPhoto.Services.Data.CategoriesService;
    using ViewModels.FilterBar;
    using Microsoft.AspNetCore.Mvc;

    [ViewComponent(Name = "FilterBar")]
    public class FilterBarViewComponent : ViewComponent
    {
        public FilterBarViewComponent(ICategoriesService categoriesService)
        {
            CategoriesService = categoriesService;
        }

        public ICategoriesService CategoriesService { get; }

        public IViewComponentResult Invoke()
        {
            FilterBarSearchDataViewModel data = new FilterBarSearchDataViewModel
            {
                Category = (List<CategoryCheckBoxViewModel>)CategoriesService.GetAll<CategoryCheckBoxViewModel>(),
            };
            return View(data);
        }
    }
}
