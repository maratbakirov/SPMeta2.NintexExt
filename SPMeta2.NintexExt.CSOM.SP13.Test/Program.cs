using Microsoft.SharePoint.Client;
using SPMeta2.NintexExt.CSOM.SP13.Handlers;
using SPMeta2.NintexExt.CSOM.SP13.Test.Model;
using SPMeta2.CSOM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMeta2.NintexExt.CSOM.SP13.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (ClientContext context = DH.NintexFormsSupport.Provisioning.Helpers.GetClientContext())
            {
                var service = new CSOMProvisionService();
                service.RegisterModelHandlers(typeof(NintexFormHandler).Assembly);

                WebModel.Provision(context, service);
                //service.PreDeploymentServices.Add(new DefaultRequiredPropertiesValidationService());
            }

            int q = 1; 
        }
    }
}
