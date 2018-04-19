// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Data.Entity.Migrations;
using ServiceCatalog.Migrations;

namespace ServiceCatalog.Controllers
{
    using System.Web.Mvc;

    public class HomeController : BaseController
    {
        public HomeController() : base() { }

        public ActionResult Index()
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            if (config.AppSettings.Settings["needCodeFirstMegration"] == null)
            {
                config.AppSettings.Settings.Add("needCodeFirstMegration", "false");
                // CODE FIRST MIGRATIONS
                var migrator = new DbMigrator(new Configuration());
                migrator.Update();
                config.Save();
            }

            return RedirectToAction("../Deployment/DeployView");
        }
    }
}