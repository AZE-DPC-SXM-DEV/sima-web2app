using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web2App.Hubs
{
    public class LogHub:Hub
    {
        public async Task ReceiveLog(string requestBody,string process, string operationId, string headers, string guid)
        {
            await Clients.All.SendAsync("ReceiveLog", requestBody, process, operationId, headers, guid);
        }
    }
}
