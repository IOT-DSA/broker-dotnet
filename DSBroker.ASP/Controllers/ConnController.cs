using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DSBroker.ASP.Controllers
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
                var ret = Program.Broker.HttpHandler.PostConnEndpoint(body, queryParams);
                var tree = new JObject();
                Program.Broker.BrokerTree.SuperRoot.ToTree(tree);
                Debug.WriteLine(tree.ToString());

                return ret;
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
