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

        public List<KustoResponseModel> GetData(List<string> engineerNames)
        {
            IEnumerable<KustoResponseModel> responseModels = new List<KustoResponseModel>();   

            var kcsb = new KustoConnectionStringBuilder(Cluster, Database)
            {
                FederatedSecurity = true,
            };

            using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb))
            {
                //Query to get the data, we should be pulling in the engineer names from the input blob
                var query = $"AllCloudsSupportIncidentPPE | where State =~ \"open\"| where AgentAlias in~({engineerNames})| project IncidentId, ModifiedDateTime, AgentAlias| where ModifiedDateTime < ago(10d)| order by ModifiedDateTime asc";

                var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };
                using (var reader = queryProvider.ExecuteQuery(query, clientRequestProperties))
                {
                   //trying to conver response diretly into model
                   responseModels = reader.ToEnumerable<KustoResponseModel>();                
                }
            }
            return responseModels.ToList();
        }

    }
}
