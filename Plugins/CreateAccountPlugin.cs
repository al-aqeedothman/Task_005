using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Task_005.Plugins
{
    public class CreateAccountPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (context.PreEntityImages.Contains("Deleted_account") && context.PreEntityImages["Deleted_account"] is Entity)
                {
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                IOrganizationServiceFactory serviceFactory  = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);               
                if (context.MessageName.ToLower() != "delete" && context.Stage != 20 )
                {
                    return;
                }           
                try
                {
                    Entity targetEntity = context.PreEntityImages["Deleted_account"];
                    Entity account = new Entity("account");         
                    var accountName = targetEntity.Attributes["name"];
                    account["name"] = accountName;
                     tracingService.Trace("Create_Account_Plugin: Creating the new account with same deleted account name.");                
                    service.Create(account);
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in Create_Account_Plugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("Create_Account_Plugin:", ex.ToString());
                    throw;
                }

            }
        }
    }
}
