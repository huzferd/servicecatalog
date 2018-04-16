using System.Collections.Generic;
using ServiceCatalog.Common.Helpers;

namespace ServiceCatalog.Controllers
{
    using System.Web.Mvc;
    using System.Linq;
    using System.Threading.Tasks;
    using BusinessLogic.Client;
    using Common.Constants;
    using Models;
    using System;

    public class BillingController : BaseController
    {
        public async Task<ViewResult> BillingView()
        {
            try
            {
                var client = new RestApiClient();
                // Get all subscriptions for this tenant
                var subscriptions = await new SubscriptionController().GetSubscriptions();
                // Select default subscription | now use locations from default subscription. Additional research required.
                var subscriptionId = subscriptions.FirstOrDefault()?.SubscriptionId;
                var accessToken = await ServicePrincipal.GetAccessToken();
                var invoicesUri = string.Format(UriConstants.GetInvoices, subscriptionId);
                var invoicesList = await client.CallGetListAsync<InvoicesViewModel>(invoicesUri, accessToken);

                Log.Info(TemplateHelper.ToJson($"BillingView - Request result: {TemplateHelper.ToJson(invoicesList)}"));

                //var invoices = invoicesList.Result?.ToList();
                var list = new List<InvoicesViewModel>();
                list.Add(new InvoicesViewModel()
                {
                    Id = "123",
                    DownloadUrl = "https://gist.githubusercontent.com/huzferd/ac100d245325ef70c516f3d0d6b6335e/raw/b02839a429623a5ea621bb08abea7f426117a68b/usage-report.csv",
                    DownloadUrlExpiry = "",
                    InvoicePeriodEndDate = DateTime.Now,
                    InvoicePeriodStartDate = DateTime.Today,
                    Name = "Test Invoice 1",
                    Type = "Billing"
                });
                ViewBag.Invoices = list;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error";
                ViewBag.ErrorDetails = ex.Message;

                return View("Error");
            }
        }
    }
}