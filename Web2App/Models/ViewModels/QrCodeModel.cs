using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web2App.Models.ViewModels
{
    public class QrCodeModel
    {
        public string Url { get; set; }
        public string FileName { get; set; }
        public string OperationId { get; set; }
        public QrCodePostModel PostModel { get; set; }
        public QrCodeModel()
        {
            PostModel = new QrCodePostModel()
            {
                CallBackUrl = "https://scanme.sima.az/home/callback",
                IconUri = "https://sima.az/img/logo.24d4a9b7.svg"
            };
        }
    }
}
