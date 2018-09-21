﻿using Microsoft.SharePoint.Client;
using SPMeta2.Common;
using SPMeta2.CSOM.ModelHandlers;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.Definitions;
using SPMeta2.NintexExt.Core.Definitions;
using SPMeta2.NintexExt.CSOM.O365.Services;
using SPMeta2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SPMeta2.NintexExt.CSOM.O365.Handlers
{
    public class NintexFormO365Handler : CSOMModelHandlerBase
    {
        public override Type TargetType
        {
            get { return typeof(NintexFormO365Definition); }
        }

        public override void DeployModel(object modelHost, DefinitionBase model)
        {
            // we need to have list id and the sharepoint authentication cookie
            NintexFormO365Definition formModel = (NintexFormO365Definition)model;
            //TODO: add some specifics?
            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioning,
                Object = null,
                ObjectType = typeof(Object),
                ObjectDefinition = formModel,
                ModelHost = modelHost
            });
            base.DeployModel(modelHost, model);
            var listModelHost = modelHost.WithAssertAndCast<ListModelHost>("modelHost", value => value.RequireNotNull());
            var web = listModelHost.HostWeb;
            var list = listModelHost.HostList;
            var clientContext = listModelHost.HostClientContext;
            string formDigestValue = clientContext.GetFormDigestDirect().DigestValue;

            var clientCredentials = clientContext.Credentials.WithAssertAndCast<SharePointOnlineCredentials>("sharepoint online credentials", value => value.RequireNotNull());
            var spSiteUrl = clientContext.Url;

            // Create a new HTTP client and configure its base address.
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(spSiteUrl);
            // Add common request headers for the REST API to the HTTP client.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Api-Key", NintexFormApiKeys.ApiKey);

            string spoCookie = clientCredentials.GetAuthenticationCookie(new Uri(spSiteUrl));
            spoCookie.WithAssert("spoCookie", value => value.RequireStringNotOrEmpty());
            //var authHeader = new AuthenticationHeaderValue(
            //    "cookie",
            //    String.Format("{0} {1}", spSiteUrl, spoCookie)
            //);
            var authHeader = new AuthenticationHeaderValue(
                "cookie",
                $"{spSiteUrl} {spoCookie}"
            );
            // Add the defined Authorization header to the HTTP client's
            // default request headers.
            client.DefaultRequestHeaders.Authorization = authHeader;

            var importFormUri = String.Format("{0}/api/v1/forms/{1}",
                NintexFormApiKeys.WebServiceUrl.TrimEnd('/'),
                Uri.EscapeUriString(list.Id.ToString()));

            //TODO: figure out how to add content type in JSON
            HttpContent saveContent = null;
            //if (string.IsNullOrEmpty(formModel.ListContentTypeNameOrId))
            {
                saveContent = new ByteArrayContent(formModel.FormData);
            }
            //    else
            //    {
            //        var result = string.Format(@"{{
            //            ""contentTypeId"":""{0}"",
            //            ""listId"":""{1}"",
            //            ""attachment"":{{
            //                ""name"" : ""file.txt"",
            //                ""fileName"" : ""file.txt"",
            //                ""data"": ""{2}""
            //            }}
            //}}",
            //            //                        ""mimeType"" : ""text/plain"",

            //            formModel.ListContentTypeNameOrId,
            //            Uri.EscapeUriString(list.Id.ToString("B").ToUpper()),
            //            Convert.ToBase64String(formModel.FormData));
            //        saveContent = new StringContent(result);
            //    }
            HttpResponseMessage saveResponse = client.PutAsync(importFormUri, saveContent).Result;

            if (formModel.Publish)
            {
                var publishFormUri = String.Format("{0}/api/v1/forms/{1}/publish",
                    NintexFormApiKeys.WebServiceUrl.TrimEnd('/'),
                    Uri.EscapeUriString(list.Id.ToString()));
                var content = "";
                if (!string.IsNullOrEmpty(formModel.ListContentTypeNameOrId))
                {
                    content = string.Format(@"{{""contentTypeId"":""{0}"",""listId"":""{1}""}}",

                        formModel.ListContentTypeNameOrId,
                        list.Id.ToString("B").ToUpper());
                }
                HttpResponseMessage puiblishResponse = client.PostAsync(publishFormUri, new StringContent(content)).Result;
            }
            //if (formModel.AssignedUseForProduction.HasValue)
            //{
            //    //TODO: add the content type here
            //    var publishFormUri = String.Format("{0}/api/v1/forms/{1},0x01006C3A1D644D3AED45AC37DDCD403CCFBE/assigneduse",
            //        NintexFormApiKeys.WebServiceUrl.TrimEnd('/'),
            //        Uri.EscapeUriString(list.Id.ToString()));
            //    var content = "";
            //    content = string.Format(@"{{""value"":""{0}""}}",
            //        formModel.AssignedUseForProduction.Value ? "production" : "development");
            //    // interesting, this can return 405 and in details ()puiblishResponse.Content.ReadAsStringAsync()
            //    // in my case i had  a message saying "your license does not allow this" or something like this
            //    HttpResponseMessage puiblishResponse = client.PutAsync(publishFormUri, 
            //        new StringContent(content,null, "application/json")).Result;
            //}

        }

    }
}