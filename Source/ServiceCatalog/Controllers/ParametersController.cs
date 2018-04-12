// -----------------------------------------------------------------------
// <copyright file="ParametersController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ServiceCatalog.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc;
    using BusinessLogic.Exceptions;
    using Common.Helpers;
    using Context;
    using Models;

    public class ParametersController : Controller
    {
        // GET: Parameters
        public async Task<ActionResult> ParametersView(
            [FromUri] int templateId
        )
        {
            JsonTemplateParametersViewModel model;
            try
            {
                model = await GetJsonTemplateParametersViewModel(templateId);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error";
                ViewBag.ErrorDetails = ex.Message;
                return View("Error");
            }

            return View(model);
        }

        private async Task<JsonTemplateParametersViewModel> GetJsonTemplateParametersViewModel(int templateId)
        {
            var parameters = await GetParametersFromTemplate(templateId);

            var subscriptions = await new SubscriptionController().GetSubscriptions();

            if (subscriptions == null)
            {
                throw new ServiceCatalogException("Couldn't retrieve a list of subscriptions");
            }

            return new JsonTemplateParametersViewModel
            {
                TemplateId = templateId,
                Parameters = parameters,
                Subscriptions = subscriptions,
            };
        }

        private async Task<List<JsonTemplateParameter>> GetParametersFromTemplate(int templateId)
        {
            TemplateViewModel template;
            using (var context = new WebAppContext())
            {
                template = await context.TemplateJsons.FirstOrDefaultAsync(tj => tj.TemplateId == templateId);
            }
            if (template == null)
            {
                throw new ServiceCatalogException("Template with specified ID couldn't be found.");
            }

            List<JsonTemplateParameter> parameters;
            try
            {
                parameters = TemplateHelper.ReadParametersFromTemplateJson(template.TemplateJson);
            }
            catch (Exception ex)
            {
                throw new ServiceCatalogException("Invalid JSON", ex);
            }

            return parameters;
        }
    }
}