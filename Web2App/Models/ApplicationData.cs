using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2App.Controllers;
using Web2App.Models.ViewModels;

namespace Web2App.Models
{
    public static class ApplicationData
    {
        public static Dictionary<string,string> QrCodes { get; set; }
        public static Dictionary<string, CallbackPostModel> PostedOperations { get; set; }
        public static Dictionary<string,LiveModel> Lives { get; set; }
        static ApplicationData()
        {
            QrCodes = new Dictionary<string, string>();
            PostedOperations = new Dictionary<string, CallbackPostModel>();
            Lives = new Dictionary<string, LiveModel>();
        }
    }
}
