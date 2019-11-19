using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
namespace AzureIOTHubAPI
{
    public static class DirectCall 
    {
        [FunctionName("DirectCall")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string connString = System.Environment.GetEnvironmentVariable("connString");
            string payload = await new StreamReader(req.Body).ReadToEndAsync();
            req.Headers.TryGetValue("method_name", out var method_name);
            req.Headers.TryGetValue("target_device", out var target_device);
            ServiceClient client = ServiceClient.CreateFromConnectionString(connString);
            CloudToDeviceMethod method = new CloudToDeviceMethod(method_name);
            method.ResponseTimeout = TimeSpan.FromSeconds(30);
            if  (string.IsNullOrEmpty(payload) == false)
                {
                try
                {
                    JToken.Parse(payload);
                    method.SetPayloadJson(payload);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult("Invalid JSON");
                }
            }
            try
            {
                CloudToDeviceMethodResult result = await client.InvokeDeviceMethodAsync(target_device, method);
                return new OkObjectResult(result.GetPayloadAsJson().ToString());
            }
            catch(Exception e)
            {
                log.LogDebug(e.ToString());
                return new OkObjectResult(e.Message);
            }    
        }
    }
    public static class GetDevices
    {
        [FunctionName("GetDevices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string connString = System.Environment.GetEnvironmentVariable("connString");
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connString);
            JArray devicearray = new JArray();
            IQuery query = registryManager.CreateQuery("select * from devices", null);
            IEnumerable<Twin> page = await query.GetNextAsTwinAsync();
            foreach(Twin twin in page)
            {
                devicearray.Add(new JObject(new JProperty("device_status", twin.ConnectionState), new JProperty("device_name", twin.DeviceId)));
            }
           return new OkObjectResult(devicearray.ToString());
        }
    }
    public static class test
    {
        [FunctionName("test")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("TEST RESPONSE");
        }
    }
}
