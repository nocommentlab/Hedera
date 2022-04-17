using ncl.hedera.HederaLib.Models.TheHive;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ncl.hedera.HederaLib.Controllers
{
    public class TheHiveManager
    {
        #region Constants
        private const string THEHIVE_ENDPOINT_CREATE_CASE_FORMAT = "/api/case";
        private const string THEHIVE_ENDPOINT_ADD_OBSERVABLE_CASE_FORMAT = "/api/case/{0}/artifact";
        private const string THEHIVE_ENDPOINT_ADD_TTP_CASE_FORMAT = "/api/v1/procedure";

        #endregion
        #region Members
        private string _STRING_TheHiveEndpointAddress;
        private int _INT32_TheHiveEnpointPort;
        private string _STRING_ApiKey;
        private string _STRING_CaseId;
        #endregion



        /*
         
        TheHiveManager thm = new("192.168.1.70", 9000, "az0iLODBSpQIDmVrkd/lijhZaZIbHjYh");
            string caseid = thm.CreateCase(new Case
            {
                Description = "thm Description",
                Pap = 1,
                Severity = 1,
                StartDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Title = "thm Title",
                Tlp = 1,
                Tags = new string[] { "t1", "t2" },
                Status = "Open"

            }).Result;

            string observableid =  thm.AddObservableToCase(new Observable
            {
                DataType = "registry",
                Tlp = 2,
                Ioc = true,
                Sighted = false,
                Tags = new string[] { "t1", "t2" },
                Data = new string[] { "Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows" },
                Message= "my first registry observable"

            }, caseid).Result;

            string procedureid = thm.AddProcedureToCase(new Procedure
            {
                CaseId = caseid,
                Description = "# Test",
                OccurDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                PatternId = "T1589",
                Tactic = "reconnaissance"

            }).Result;
        */
        public TheHiveManager(string STRING_Address, int INT32_Port, string STRING_ApiKey)
        {

            _STRING_TheHiveEndpointAddress = STRING_Address;
            _INT32_TheHiveEnpointPort = INT32_Port;
            _STRING_ApiKey = STRING_ApiKey;


        }

        public async Task CreateCase(Case CASE_NewCase)
        {


            var client = new RestClient($"http://{_STRING_TheHiveEndpointAddress}:{_INT32_TheHiveEnpointPort}{THEHIVE_ENDPOINT_CREATE_CASE_FORMAT}");

            var request = new RestRequest
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Authorization", $"Bearer {this._STRING_ApiKey}");
            request.AddHeader("Content-Type", "application/json");


            request.AddJsonBody(CASE_NewCase);

            RestResponse response = await client.PostAsync(request);

            JsonObject deserializedResponse = JsonNode.Parse(response.Content).AsObject();

            _STRING_CaseId = deserializedResponse["_id"].ToString();
            //return deserializedResponse["_id"].ToString();
        }

        public async Task<string> AddObservableToCase(Observable OBSERVABLE_ToAdd)
        {

            string STRING_ObservableId = null;
            string STRING_BaseUrl = $"http://{_STRING_TheHiveEndpointAddress}:{_INT32_TheHiveEnpointPort}{THEHIVE_ENDPOINT_ADD_OBSERVABLE_CASE_FORMAT}";
            var client = new RestClient(String.Format(STRING_BaseUrl, _STRING_CaseId));

            var request = new RestRequest
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Authorization", $"Bearer {this._STRING_ApiKey}");
            request.AddHeader("Content-Type", "application/json");


            request.AddJsonBody(OBSERVABLE_ToAdd);

            RestResponse response = await client.PostAsync(request);

            try
            {
                JsonNode deserializedResponse = JsonArray.Parse(response.Content);
                STRING_ObservableId = deserializedResponse[0].AsObject()["id"].ToString();
            }
            catch (Exception) { }

            return STRING_ObservableId;
        }

        public async Task<string> AddProcedureToCase(Procedure PROCEDURE_ToAdd)
        {


            var client = new RestClient($"http://{_STRING_TheHiveEndpointAddress}:{_INT32_TheHiveEnpointPort}{THEHIVE_ENDPOINT_ADD_TTP_CASE_FORMAT}");

            var request = new RestRequest
            {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Authorization", $"Bearer {this._STRING_ApiKey}");
            request.AddHeader("Content-Type", "application/json");

            PROCEDURE_ToAdd.CaseId = _STRING_CaseId;

            request.AddJsonBody(PROCEDURE_ToAdd);

            RestResponse response = await client.PostAsync(request);

            JsonObject deserializedResponse = JsonNode.Parse(response.Content).AsObject();

            return deserializedResponse["_id"].ToString();


        }
    }


}
