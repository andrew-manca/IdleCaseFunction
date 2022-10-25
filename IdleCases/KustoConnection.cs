using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kusto.Cloud.Platform.Data;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Linq;
using Kusto.Data.Net.Client;

namespace IdleCases
{
    internal class KustoConnection
    {
        const string Cluster = "https://usage360.kusto.windows.net";
        const string Database = "Product360";

        public List<KustoResponseModel> GetData(string engineerNames)
        {
            List<KustoResponseModel> responseModels = new List<KustoResponseModel>();



            string bearerToken = "";

            var kcsb = new KustoConnectionStringBuilder(Cluster, Database)
            {
                FederatedSecurity = true
            }.WithAadApplicationTokenAuthentication(bearerToken);

            using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb))
            {
                //Query to get the data, we should be pulling in the engineer names from the input blob
                var query = $"AllCloudsSupportIncidentPPE | where State =~ \"open\"| where AgentAlias in~({engineerNames})| project IncidentId, ModifiedDateTime, AgentAlias| where ModifiedDateTime < ago(10d)| order by ModifiedDateTime asc";

                var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };
                using (var reader = queryProvider.ExecuteQuery(query, clientRequestProperties))
                {
                    //trying to conver response diretly into model
                    //responseModels = reader.ToEnumerable<KustoResponseModel>();  
                    while (reader.Read())
                    {
                        KustoResponseModel responseModel = new KustoResponseModel();
                        responseModel.IncidentId = reader.GetString(0);
                        responseModel.ModifiedDateTime = reader.GetDateTime(1);
                        responseModel.AgentAlias = reader.GetString(2);
                        responseModels.Add(responseModel);
                    }             
                }
            }
            return responseModels.ToList();
        }

    }
}
