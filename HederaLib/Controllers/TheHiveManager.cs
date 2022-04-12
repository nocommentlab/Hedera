using ncl.hedera.HederaLib.Models.TheHive;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Controllers
{
    public class TheHiveManager
    {
        #region Constants
        private const string THEHIVE_ENDPOINT_CREATE_CASE_FORMAT = "/api/case";
        private const string THEHIVE_ENDPOINT_ADD_TTP_CASE_FORMAT = "/api/v1/procedure";

        #endregion
        #region Members
        private string STRING_TheHiveEndpointAddress;
        private int INT32_TheHiveEnpointPort;
        private string STRING_ApiKey;
        private readonly RestClient _restClient;
        #endregion


        public TheHiveManager(string STRING_Address, int INT32_Port, string STRING_ApiKey)
        {

            this.STRING_TheHiveEndpointAddress = STRING_Address;
            this.INT32_TheHiveEnpointPort = INT32_Port;
            this.STRING_ApiKey = STRING_ApiKey;

            
            
        }

        public async Task<string> CreateCase(Case CASE_NewCase)
        {
            var client = new RestClient("http://192.168.1.64:9000/api/case");

            var request = new RestRequest
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Authorization", $"Bearer {this.STRING_ApiKey}");
            request.AddHeader("Content-Type", "application/json");



            request.AddParameter("application/json", JsonSerializer.Serialize(CASE_NewCase), ParameterType.RequestBody);
            RestResponse response = await client.PostAsync(request);
            //Console.WriteLine(response.Content);

            return response.Content;
        }
    }

    
}
