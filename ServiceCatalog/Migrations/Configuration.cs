// -----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


namespace ServiceCatalog.Migrations
{
    using System;
    using Models;
    using System.Data.Entity.Migrations;
    using Common.Helpers;
    using System.IO;
    using NLog;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Context.WebAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ServiceCatalog.Context.WebAppContext";
        }

        protected override void Seed(Context.WebAppContext context)
        {
            var log = LogManager.GetLogger(GetType().FullName);
            try
            {
                if (context.TemplateJsons.Any())
                {
                    return;
                }

                var baseDir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin", string.Empty) + "\\Templates";
                log.Info($"Base Dir: {baseDir}");
                log.Info($"templateJson1 path: {Path.Combine(baseDir, "Billing-Invoice.json")}");
                var templateJson1 = File.ReadAllText(Path.Combine(baseDir, "Billing-Invoice.json"));
                var templateJson2 = File.ReadAllText(Path.Combine(baseDir, "Manage-VM.json"));
                var templateJson3 = File.ReadAllText(Path.Combine(baseDir, "Ubuntu-Server-Template.json"));
                var templateJson4 = File.ReadAllText(Path.Combine(baseDir, "Windows-Server-Template.json"));

                context.TemplateJsons.AddOrUpdate(x => x.TemplateId,
                    new TemplateViewModel()
                    {
                        Date = DateTime.Now,
                        IsManageTemplate = true,
                        TemplateId = 1,
                        TemplateJson = templateJson1,
                        TemplateName = "Billing-Invoice.json",
                        Comment = "Billing Invoice",
                        UserName = UserRoleHelper.AdminUserName,
                        TemplateJsonVersion = "1.0",
                        TemplateUsersGroup = "*"
                    },
                    new TemplateViewModel()
                    {
                        Date = DateTime.Now,
                        IsManageTemplate = true,
                        TemplateId = 2,
                        TemplateJson = templateJson2,
                        TemplateName = "Manage-VM.json",
                        Comment = "Manage VM",
                        UserName = UserRoleHelper.AdminUserName,
                        TemplateJsonVersion = "1.0",
                        TemplateUsersGroup = "*"
                    },
                    new TemplateViewModel()
                    {
                        Date = DateTime.Now,
                        IsManageTemplate = false,
                        TemplateId = 3,
                        TemplateJson = templateJson3,
                        TemplateName = "Ubuntu-Server-Template.json",
                        Comment = "Ubuntu Server Template",
                        UserName = UserRoleHelper.AdminUserName,
                        TemplateJsonVersion = "1.0",
                        TemplateUsersGroup = "*"
                    },
                    new TemplateViewModel()
                    {
                        Date = DateTime.Now,
                        IsManageTemplate = false,
                        TemplateId = 4,
                        TemplateJson = templateJson4,
                        TemplateName = "Windows-Server-Template.json",
                        Comment = "Windows Server Template",
                        UserName = UserRoleHelper.AdminUserName,
                        TemplateJsonVersion = "1.0",
                        TemplateUsersGroup = "*"
                    }
                );
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
