using Kusto.Cloud.Platform.Utils;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleCases
{
    internal class EmailUsers
    {
        public ICollector<SendGridMessage> createEmails(ICollector<SendGridMessage> messageCollector, List<KustoResponseModel> responseModels)
        {
             foreach (var customerCase in responseModels)
            {
                var message = new SendGridMessage()
                {
                    From = new EmailAddress("anmanca@microsoft.com", "Andrew Manca"),
                    Subject = $"Email {customerCase.IncidentId}: This case is showing as Idle ",
                    PlainTextContent = $"Hi, please review the following case {customerCase.IncidentId} as it is showing as idle.  The last modified time was {customerCase.ModifiedDateTime}",
                };
                //Hard coding this for testing purposes.
                message.AddTo(new EmailAddress($"({customerCase.AgentAlias}@microsoft.com", $"{customerCase.AgentAlias}"));
                message.AddCcs(new List<EmailAddress>
              {
                   new EmailAddress("anmanca@microsoft.com", "Andrew Manca"),
                   /* commenting out the below emails to not spam during testing
                    
                   new EmailAddress("tyspring@microsoft.com", "Tyler Springer"),
                   new EmailAddress("macavall@microsoft.com", "Matt Cavallo"),
                   new EmailAddress("chnorman@microsoft.com", "Chris Norman"),
                   new EmailAddress("kigranad@microsoft.com", "Kimberly Granados")
                   */
              });

                messageCollector.Add(message);
            }
            return messageCollector;
        }


    }
}
