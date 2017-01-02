using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DSBroker.ASPdotNET.Controllers
{
    [Route("conn")]
    public class ConnController : Controller
    {
        [HttpPost]
        public string Post()
        {
            var body = new StreamReader(Request.Body).ReadToEnd();
            var queryParams = new Dictionary<string, string>();

            foreach (KeyValuePair<string, StringValues> pair in Request.Query)
            {
                queryParams[pair.Key] = pair.Value.ToString();
            }

            try
            {
                return Program.Broker.HttpHandler.PostConnEndpoint(body, queryParams);
            }
            catch (Exception e)
            {
                Response.StatusCode = 400;
                Debug.WriteLine(e.StackTrace);
                return e.Message;
            }
        }
    }
}
