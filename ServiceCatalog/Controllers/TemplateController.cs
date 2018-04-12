﻿// -----------------------------------------------------------------------
// <copyright file="TemplateController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ServiceCatalog.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Context;
    using Models;
    using System;
    using System.Web;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    /// Manage templates
    /// </summary>
    public class TemplateController : BaseController
    {
        public TemplateController() : base() { }

        /// <summary>
        /// Retrieves the templates
        /// </summary>
        public async Task<List<TemplateViewModel>> GetTemplates()
        {
            using (var context = new WebAppContext())
            {
                var email = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;
                var userGroups = ClaimsPrincipal.Current.FindAll("groups").Select(g => g.Value).ToList();
                var adminUser = "eduadmin2@wwedudemo7.onmicrosoft.com";

                var graphGroups = await new GraphGroupController().GetGraphGroups();
                var graphUsers = await new GraphUserController().GetGetGraphUsers();
                graphGroups.AddRange(graphUsers.Select(user => new GraphGroupsViewModel()
                {
                    ObjectId = user.UserPrincipalName,
                    DisplayName = user.DisplayName
                }));

                var intersectGroups = (from gGroup in graphGroups from uGroup in userGroups where gGroup.ObjectId == uGroup select gGroup).ToList();
                intersectGroups.Add(new GraphGroupsViewModel() { ObjectId = email, DisplayName = email });
                intersectGroups.Add(new GraphGroupsViewModel() { ObjectId = "*", DisplayName = "Available to All" });
                graphGroups.Add(new GraphGroupsViewModel() { ObjectId = email, DisplayName = email });
                graphGroups.Add(new GraphGroupsViewModel() { ObjectId = "*", DisplayName = "Available to All" });

                var templates = await context.TemplateJsons.ToListAsync();
                var result = new List<TemplateViewModel>();
                if (email == adminUser)
                {
                    foreach (var template in templates)
                    {
                        var uGroups = template.TemplateUsersGroup.Split(',');
                        template.TemplateUsersGroup = GetInlineGroups(uGroups, graphGroups);

                        result.Add(template);
                    }
                }
                else
                {
                    foreach (var template in templates)
                    {
                        var uGroups = template.TemplateUsersGroup.Split(',');
                        var intersectResult = GetInlineGroups(uGroups, intersectGroups);
                        if (intersectResult.Length > 0)
                        {
                            var groups = GetInlineGroups(uGroups, graphGroups);
                            template.TemplateUsersGroup = groups;
                            result.Add(template);
                        }
                    }
                }

                return result;
            }
        }

        private string GetInlineGroups(string[] uGroups, List<GraphGroupsViewModel> gGroups)
        {
            var groups = new List<string>();
            foreach (var u in uGroups)
            {
                foreach (var g in gGroups)
                {
                    if (u == g.ObjectId)
                    {
                        groups.Add(g.DisplayName);
                    }
                }
            }

            return string.Join(", ", groups);
        }

        /// <summary>
        /// Retrieves a single template
        /// </summary>
        public async Task<TemplateViewModel> GetTemplate(long templateId)
        {
            using (var context = new WebAppContext())
            {
                return await context.TemplateJsons.FirstOrDefaultAsync(x => x.TemplateId == templateId);
            }
        }

        [HttpPost]
        public ActionResult CreateTemplate(HttpPostedFileBase templateData, TemplateViewModel templateViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidDataException("Invalid Model");
                }

                using (StreamReader reader = new StreamReader(templateData.InputStream))
                using (WebAppContext context = new WebAppContext())
                {
                    TemplateViewModel template = new TemplateViewModel
                    {
                        TemplateName = templateData.FileName,
                        TemplateJson = reader.ReadToEnd(),
                        Date = DateTime.Now,
                        Comment = templateViewModel.Comment,
                        TemplateUsersGroup = templateViewModel.TemplateUsersGroup,
                        TemplateJsonVersion = templateViewModel.TemplateJsonVersion,
                        IsManageTemplate = templateViewModel.IsManageTemplate,
                        UserName = System.Web.HttpContext.Current.User.Identity.Name
                    };
                    context.TemplateJsons.Add(template);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error";
                ViewBag.ErrorDetails = ex.Message;

                return View("Error");
            }

            return RedirectToAction(templateViewModel.IsManageTemplate ? "../Deployment/ManageView" : "../Deployment/DeployView");
        }

        public async Task<ActionResult> UpdateTemplate(HttpPostedFileBase templateData, TemplateViewModel templateViewModel)
        {
            try
            {
                using (WebAppContext context = new WebAppContext())
                {
                    var newTemplate = await context.TemplateJsons.FirstOrDefaultAsync(x => x.TemplateId == templateViewModel.TemplateId);
                    if (newTemplate != null)
                    {
                        newTemplate.Date = DateTime.Now;
                        newTemplate.Comment = templateViewModel.Comment;
                        newTemplate.TemplateJsonVersion = templateViewModel.TemplateJsonVersion;
                        newTemplate.UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        newTemplate.TemplateUsersGroup = templateViewModel.TemplateUsersGroup;
                        if (templateData?.ContentLength > 0)
                        {
                            using (StreamReader reader = new StreamReader(templateData.InputStream))
                            {
                                newTemplate.TemplateJson = reader.ReadToEnd();
                            }
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error";
                ViewBag.ErrorDetails = ex.Message;

                return View("Error");
            }

            return RedirectToAction(templateViewModel.IsManageTemplate ? "../Deployment/ManageView" : "../Deployment/DeployView");
        }

        [HttpDelete]
        [ActionName("DeleteTemplate")]
        public async Task<ActionResult> DeleteTemplate(long templateId)
        {
            using (var context = new WebAppContext())
            {
                TemplateViewModel templateViewModel = new TemplateViewModel() { TemplateId = templateId };
                context.TemplateJsons.Attach(templateViewModel);
                context.TemplateJsons.Remove(templateViewModel);
                await context.SaveChangesAsync();
            }

            return RedirectToAction("../Deployment/DeployView");
        }

        /// <summary>
        /// Retrieves a view to create template
        /// </summary>
        public async Task<ActionResult> CreateTemplateView()
        {
            var graphGroups = await new GraphGroupController().GetGraphGroups();
            ViewBag.Groups = graphGroups;
            var graphUsers = await new GraphUserController().GetGetGraphUsers();
            ViewBag.Users = graphUsers;

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> UpdateTemplateView(long templateId)
        {
            using (var context = new WebAppContext())
            {
                var template = context.TemplateJsons.FirstOrDefaultAsync(x => x.TemplateId == templateId);
                var graphGroups = await new GraphGroupController().GetGraphGroups();
                ViewBag.Groups = graphGroups;
                var graphUsers = await new GraphUserController().GetGetGraphUsers();
                ViewBag.Users = graphUsers;

                return View(template.Result);
            }
        }
    }
}