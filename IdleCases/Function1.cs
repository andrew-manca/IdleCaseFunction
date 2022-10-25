using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System.Collections;
using System.Linq;

namespace IdleCases
{
    public class Function1
    {
        [FunctionName("IdleCases")]
        
        public void Run([TimerTrigger("%timerSchedule%")]TimerInfo myTimer, 
                        [Blob("casemonitoring/EngineerList.txt", FileAccess.Read)] string myBlob, ILogger log,
                        [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> messageCollector)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            log.LogInformation($"Need to create engineer list based on blob file");
     

            EmailUsers email = new EmailUsers();
            List<KustoResponseModel> idleCases = new List<KustoResponseModel>();
            string[] engineerAlias = myBlob.Split(',');
            log.LogInformation("Logging out the cases we return " + engineerAlias[0]);
            //hard coding for test

            KustoResponseModel test = new KustoResponseModel
            {
                IncidentId = "12345",
                AgentAlias = "anmanca",
                ModifiedDateTime = DateTime.Now
            };
            idleCases.Add(test);
            log.LogInformation($"Creating the email list");
            messageCollector = email.createEmails(messageCollector, idleCases);

        }
    }
}

