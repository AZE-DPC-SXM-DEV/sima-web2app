using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web2App.Models
{
    public class ClientInfo
    {
        public int ClientId { get; set; }
        public string IconURI { get; set; }
        public string Callback { get; set; }
        public string ClientName { get; set; } = "Web2App Scan";
        public List<string> HostName { get; set; }

        public ClientInfo()
        {

        }

        public ClientInfo(int clientId , string iconUri, string callBack,string hostName)
        {
            ClientId = clientId;
            IconURI = iconUri;
            Callback = callBack;

            if (hostName != null)
            {
                string[] hostNames = hostName.Split(",");
                HostName = new List<string>();
                
                for(var i=0;i<hostNames.Length;i++)
                {
                    HostName.Add(hostNames[i]);
                }
            }
        }

    }
}
