// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Data.Entity;
using ServiceCatalog;
using Microsoft.Owin;
using Owin;
using ServiceCatalog.Context;
using ServiceCatalog.Migrations;

[assembly: OwinStartup(typeof(Startup))]

namespace ServiceCatalog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<WebAppContext, Configuration>());
            ConfigureAuth(app);
        }
    }
}