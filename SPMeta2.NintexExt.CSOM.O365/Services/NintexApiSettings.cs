﻿using SPMeta2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMeta2.NintexExt.CSOM.O365.Services
{

    [Obsolete("Use NintexApiSettings")]
    public class NintexFormApiKeys
    {
        public static string ApiKey
        {
            get { return NintexApiSettings.ApiKey; }
            set { NintexApiSettings.ApiKey = value; }
        }
        public static string WebServiceUrl
        {
            get { return NintexApiSettings.WebServiceUrl; }
            set { NintexApiSettings.WebServiceUrl = value; }
        }
    }

    public class NintexApiSettings
    {
        /// <summary>
        /// The api key for the nintex operations
        /// </summary>
        /// <see cref="https://help.nintex.com/en-US/sdks/sdko365/#FormSDK/Topics/SDK_NFO_PRC__REST_QSG.htm#Get_your_API_key"/>
        public static string ApiKey;

        /// <summary>
        /// The web service url for the nintex operations
        /// </summary>
        /// <see cref="https://help.nintex.com/en-US/sdks/sdko365/#FormSDK/Topics/SDK_NFO_PRC__REST_QSG.htm#Get_your_API_key"/>
        public static string WebServiceUrl;
        //TODO: calculate the url if not specified

        /// <summary>
        /// The timeout for HttpClient.Timeout that is used in the implementation
        /// </summary>
        public static TimeSpan HttpRequestTimeout = TimeSpan.FromMinutes(3);

        /// <summary>
        /// Maximum amount of retries in a case of error coming from httpclient
        /// </summary>
        public static int  MaxRetries = 3;

        /// <summary>
        /// Amount in milliseconds to wait after first error
        /// </summary>
        public static int  FirstRetryTimeoutMs = 5000;
    
        /// <summary>
        /// How the timeout increases after second and subsequent errors
        /// </summary>
        public static int TimeoutIncreaseMs = 5000;

        /// <summary>
        /// Defines the lists of codes that are treated as semi-success for workflow publish and assigned use operations.
        /// if one of these codes comes up, then the library checks that the publish and assigned use has actually happened.
        /// in this case the error code is changed back to 299 so you can still check the error message
        /// </summary>
        public static System.Net.HttpStatusCode[] SemiSuccessFullPublishHttpErrorCodes =
            { System.Net.HttpStatusCode.BadGateway};
    }
}
