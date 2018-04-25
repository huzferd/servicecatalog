// -----------------------------------------------------------------------
// <copyright file="RunBooksController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


namespace ServiceCatalog.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using BusinessLogic.Client;
    using Common.Constants;
    using Models;
    using System;

    public class RunBooksController : BaseController
    {
        // GET: RunBooks
        public async Task<ActionResult> RunBooksView()
        {
            try
            {
                var subscriptions = await new SubscriptionController().GetSubscriptions();
                var subscriptionId = subscriptions.FirstOrDefault()?.SubscriptionId;
                var token = await ServicePrincipal.GetAccessToken();
                var automationAccountClient = new RestApiClient();

                var automationAccountUri = string.Format(UriConstants.GetAutomationAccounts, Url.Encode(subscriptionId));
                var automationAccounts = await automationAccountClient.CallGetListAsync<AutomationAccount>(automationAccountUri, token);
                var automationAccountsResult = automationAccounts.Result;

                var jobList = new List<Job>();
                foreach (var account in automationAccountsResult)
                {
                    var jobAccountClient = new RestApiClient();
                    var jobsUrl = string.Format(UriConstants.GetJobs, account.Id);
                    var jobs = await jobAccountClient.CallGetListAsync<Job>(jobsUrl, token);
                    var jobsResult = jobs.Result;
                    foreach (var job in jobsResult)
                    {
                        var jobOutputClient = new RestApiClient();
                        var jobOutputUrl = string.Format(UriConstants.GetJobOutput, job.Id);
                        var jobOutput = await jobOutputClient.CallGetText(jobOutputUrl, token);
                        var newJob = job;
                        newJob.Outputs = jobOutput;
                        jobList.Add(newJob);
                    }
                }

                return View(jobList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error";
                ViewBag.ErrorDetails = ex.Message;

                Log.Error(ex);

                return View("Error");
            }
        }
    }
}