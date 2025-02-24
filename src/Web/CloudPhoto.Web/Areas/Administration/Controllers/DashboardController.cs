﻿namespace CloudPhoto.Web.Areas.Administration.Controllers
{
    using CloudPhoto.Services.Data;
    using CloudPhoto.Web.ViewModels.Administration.Dashboard;

    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;

        public DashboardController(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel { SettingsCount = settingsService.GetCount(), };
            return View(viewModel);
        }
    }
}
