using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Web2App.Enums;
using Web2App.Hubs;
using Web2App.Interfaces;
using Web2App.Models;
using Web2App.Models.ViewModels;

namespace Web2App.Controllers
{


    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly ITsContainerService _tsContainerService;
        private readonly IQrGenerator _qrGenerator;
        private readonly IApplicationDbContext _context;

        private readonly string _baseUrl;
        public HomeController(ILogger<HomeController> logger,
                            ITsContainerService tsContainerService,
                            IQrGenerator qrGenerator,
                            IApplicationDbContext context
                         )
        {
            _logger = logger;
            _tsContainerService = tsContainerService;
            _qrGenerator = qrGenerator;
            _context = context;
        }

        public IActionResult Index(string operationId)
        {
            ApplicationData.QrCodes.TryGetValue(operationId ??= "", out var fileName);

            return View(new QrCodeModel()
            {
                FileName = fileName,
                OperationId = operationId
            });
        }



        [HttpPost]
        public async Task<JsonResult> QrCodeGenerate(QrCodePostModel model, string oldOperationId)
        {
            if (oldOperationId != "")
                ClearSession(oldOperationId);

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    status = 400,
                    errorMessage = "Pleasae check your data..."

                });
            }
            //make ts container
            var tsContainer = await _tsContainerService.MakeTsContainer(model);

            //serialize full jsoncontainer
            string fullJsonContainer = JsonConvert.SerializeObject(tsContainer);

            if (tsContainer.SignableContainer.DataInfo.DataURI == null)
            {
                var jobject = JObject.Parse(fullJsonContainer);
                var s = (JObject)jobject.SelectToken("SignableContainer", false);
                ((JObject)jobject.SelectToken("SignableContainer", false)).Properties()
                         .Where(attr => attr.Name.StartsWith("DataInfo"))
                        .ToList()
                        .ForEach(attr => attr.Remove());
                fullJsonContainer = Regex.Replace(jobject.ToString(), "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
            }

            string base64JsonData = Convert.ToBase64String(Encoding.UTF8.GetBytes(fullJsonContainer));

            //make url for getting file with tsquery params
            var url = "https" + "://" + HttpContext.Request.Host.ToUriComponent() + "/Home/GetFile/?tsquery=" + base64JsonData;

            //generate qr code
            var generatedQrFileName = await _qrGenerator.GenerateQr(url);

            var operationId = tsContainer.SignableContainer.OperationInfo.OperationId;

            bool qrCodeExsistResult = ApplicationData.QrCodes.TryGetValue(operationId, out string r);
            bool postedOperationResult = ApplicationData.PostedOperations.TryGetValue(operationId, out CallbackPostModel a);

            if (qrCodeExsistResult)
            {
                ApplicationData.QrCodes.Remove(operationId);
            }

            if (postedOperationResult)
            {
                ApplicationData.PostedOperations.Remove(operationId);
            }


            if (model.Type == OperationType.Sign)
            {
                if (model.OperationId != "test1contract" && model.OperationId != "kreditmuqavilesi")
                {

                    #region Create temp file for current operation if operation is Sign (it is just for scanme.sima.az)
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", "test.pdf");

                    using (FileStream reader = new FileStream(path, FileMode.Open))
                    {
                        var buffer = new byte[reader.Length];
                        await reader.ReadAsync(buffer, 0, buffer.Length);

                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", operationId + ".pdf");
                        using (FileStream filestream = new FileStream(path, FileMode.Create))
                        {
                            await filestream.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }
                    #endregion
                }
                else
                {
                    string path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", "test1contract.docx");
                    string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "test1contract.docx");
                    if (!System.IO.File.Exists(basePath))
                    {
                        using (FileStream reader = new FileStream(path1, FileMode.Open))
                        {
                            var buffer = new byte[reader.Length];
                            await reader.ReadAsync(buffer, 0, buffer.Length);

                            path1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "test1contract.docx");
                            using (FileStream filestream = new FileStream(path1, FileMode.Create))
                            {
                                await filestream.WriteAsync(buffer, 0, buffer.Length);
                            }
                        }
                    }

                    string path2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", "test2contract.pdf");
                    string basePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "kreditmuqavilesi.pdf");
                    if (!System.IO.File.Exists(basePath2))
                    {
                        using (FileStream reader = new FileStream(path2, FileMode.Open))
                        {
                            var buffer = new byte[reader.Length];
                            await reader.ReadAsync(buffer, 0, buffer.Length);

                            path2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "kreditmuqavilesi.pdf");
                            using (FileStream filestream = new FileStream(path2, FileMode.Create))
                            {
                                await filestream.WriteAsync(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
            }


            ApplicationData.QrCodes.Add(operationId, generatedQrFileName);
            ApplicationData.PostedOperations.Add(operationId, null);
            HttpContext.Session.SetString("scanmeuser", operationId);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _context.LogAsync(new SimaLog
            {
                SimaLogTypeId = (int)SimaLogTypeEnum.Information,
                Created = DateTime.Now,
                IpAddress = ipAddress,
                Description = "QrCode uğurla yaradıldı.",
                ErrorMessage = "",
                RequestBody = fullJsonContainer,
                Headers = ""
            });


            return Json(new
            {
                status = 200,
                fileName = generatedQrFileName,
                operationId = operationId,
                url = url
            }); 
        }

        [HttpGet]
        public async Task<IActionResult> GetFile(string tsquery)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var headers = GetHeaders(HttpContext.Request.Headers);


                var displayUrl = HttpContext.Request.GetDisplayUrl();

                await _context.LogAsync(new SimaLog
                {
                    SimaLogTypeId = (int)SimaLogTypeEnum.Information,
                    Created = DateTime.Now,
                    IpAddress = ipAddress,
                    Description = "GetFile üçün sorğu başladı.",
                    ErrorMessage = "",
                    RequestBody = tsquery,
                    Headers = headers
                });

                //#region Header Validation
                ////ts cert
                //var tcCertResult = HttpContext.Request.Headers.TryGetValue("ts-cert", out StringValues tcCert);
                //if (!tcCertResult)
                //{
                //    await _context.LogAsync(new SimaLog
                //    {
                //        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                //        Created = DateTime.Now,
                //        IpAddress = ipAddress,
                //        Description = "Header xətası baş verdi.",
                //        ErrorMessage = "Sorğu zamanı headerda TS - CERT qeyd edilməyib",
                //        RequestBody = tsquery,
                //        Headers = headers
                //    });

                //    return Json(new
                //    {
                //        errormessage = "Header does not have ts-cert"
                //    });
                //}

                ////ts-sign
                //var tcSignResult = HttpContext.Request.Headers.TryGetValue("ts-sign", out StringValues tcSign);

                //if (!tcSignResult)
                //{
                //    await _context.LogAsync(new SimaLog
                //    {
                //        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                //        Created = DateTime.Now,
                //        IpAddress = ipAddress,
                //        Description = "Header xətası baş verdi.",
                //        ErrorMessage = "Sorğu zamanı headerda TS-SIGN qeyd edilməyib",
                //        RequestBody = tsquery,
                //        Headers = headers
                //    });

                //    return Json(new
                //    {
                //        errormessage = "Header does not have ts-sign"
                //    });
                //}


                ////ts sign alg
                //var tcSignAlgResult = HttpContext.Request.Headers.TryGetValue("ts-sign-alg", out StringValues tcAlg);
                //if (!tcSignAlgResult)
                //{
                //    await _context.LogAsync(new SimaLog
                //    {
                //        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                //        Created = DateTime.Now,
                //        IpAddress = ipAddress,
                //        Description = "Header xətası baş verdi.",
                //        ErrorMessage = "Sorğu zamanı headerda TS-SIGN-ALG qeyd edilməyib",
                //        RequestBody = tsquery,
                //        Headers = headers
                //    });
                //    return Json(new
                //    {
                //        errormessage = "Header does not have ts-sign-alg"
                //    });
                //}
                //#endregion


                //#region CERT VALIDATION
                //X509CertificateParser certParser = new X509CertificateParser();

                //var cert = certParser.ReadCertificate(Convert.FromBase64String(tcCert));

                //var signer = SignerUtilities.GetSigner("SHA-256withECDSA");

                //var pub = cert.GetPublicKey();

                //var stpem = ConvertToStringPem(pub);

                //// Verify using the certificate - the certificate's public key is extracted using the GetPublicKey method.
                //signer.Init(false, cert.GetPublicKey());

                ////Get query string from url dynamically
                //string queryString = HttpContext.Request.GetEncodedPathAndQuery(); // /Home/GetFile/?tsQuery=asdadsa

                //var queryStringBuffer = Encoding.UTF8.GetBytes(queryString);
                //signer.BlockUpdate(queryStringBuffer, 0, queryStringBuffer.Length);
                //var success = signer.VerifySignature(Convert.FromBase64String(tcSign));

                //if (!success)
                //{

                //    await _context.LogAsync(new SimaLog
                //    {
                //        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                //        Created = DateTime.Now,
                //        IpAddress = ipAddress,
                //        Description = "Cert validasiya xətaşı baş verdi.",
                //        ErrorMessage = "Cert validasiya edilə bilmədi.",
                //        RequestBody = tsquery,
                //        Headers = headers
                //    });

                //    return Json(new
                //    {
                //        errormessage = "Certificate validation is unsuccessfully"
                //    });

                //}
                //#endregion

                try
                {
                    byte[] data = Convert.FromBase64String(tsquery);
                    string jsonContainer = Encoding.UTF8.GetString(data);

                    var result = System.Text.Json.JsonSerializer.Deserialize(jsonContainer, typeof(TSContainer));
                    TSContainer container = JsonConvert.DeserializeObject<TSContainer>(jsonContainer);


                    if (container.SignableContainer.DataInfo != null && container.SignableContainer.DataInfo.DataURI != displayUrl)
                    {
                        return Json(new
                        {
                            filename = "",
                            data = ""
                        });
                    }


                    string base64 = String.Empty;
                    if (container.SignableContainer.OperationInfo.Type == OperationType.Auth)
                    {
                        var challange = Guid.NewGuid().ToString();
                        base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(challange));

                    }

                    else
                    {
                        string extension = container.SignableContainer.OperationInfo.OperationId == "test1contract" ? ".docx" : ".pdf";
                        //You need to find and get correct base64 of file (I will return random file)
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", container.SignableContainer.OperationInfo.OperationId + extension);

                        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            base64 = Convert.ToBase64String(buffer);
                        }

                    }

                    LogToSession(tsquery, "getfile", container.SignableContainer.OperationInfo.OperationId, headers, Guid.NewGuid().ToString());

                    return Json(new
                    {
                        filename = container.SignableContainer.OperationInfo.Type == OperationType.Auth ? "challange" : container.SignableContainer.OperationInfo.OperationId + ".pdf",
                        data = base64
                    });
                }
                catch (Exception exp)
                {
                    await _context.LogAsync(new SimaLog
                    {
                        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                        Created = DateTime.Now,
                        IpAddress = ipAddress,
                        Description = "Xəta baş verdi.",
                        ErrorMessage = exp.Message,
                        RequestBody = tsquery,
                        Headers = headers
                    });

                    return Json(new
                    {
                        errormessage = exp.Message
                    });
                }
            }

            catch(Exception exp)
            {
                await _context.LogAsync(new SimaLog
                {
                    SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                    Created = DateTime.Now,
                    IpAddress = "",
                    Description = "Xəta baş verdi.",
                    ErrorMessage = exp.Message,
                    RequestBody = tsquery,
                    Headers = ""
                });

                return Json(new
                {
                    errormessage = exp.Message
                });
            }
        }
        public static string ConvertToStringPem(AsymmetricKeyParameter pem)
        {
            byte[] publicKeyDer = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pem).GetDerEncoded();
            return Convert.ToBase64String(publicKeyDer);
        }


        [HttpGet]
        public async Task<IActionResult> GetStatus(string operationId)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();



            _logger.Log(LogLevel.Information, $"-------GET STATUS started with IP:{ipAddress}-------");
            var headers = GetHeaders(HttpContext.Request.Headers);
            _logger.Log(LogLevel.Information, $"-------HEADERS :" + headers);
            CallbackPostModel callbackM = null;
            bool result = ApplicationData.PostedOperations.TryGetValue(operationId, out callbackM);

            if (!result)
                return Json(new
                {
                    status = false,
                    callback = ""
                });

            if (callbackM != null)
            {
                return Json(new
                {
                    status = true,
                    callback = callbackM
                });
            }

            return Json(new
            {
                status = false,
                callback = ""
            });
        }


        [HttpPost]
        public async Task<IActionResult> Callback()
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var headers = GetHeaders(HttpContext.Request.Headers);


                string body = "";
                CallbackPostModel model = null;


                //get request body
                using (var streamReader = new StreamReader(HttpContext.Request.Body))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(await streamReader.ReadToEndAsync());

                    body = stringBuilder.ToString();

                    //int versionStartIndex = body.IndexOf("version");

                    //int startToDelete = versionStartIndex - 2;

                    //body = body.Remove(startToDelete, 18);

                    model = JsonConvert.DeserializeObject<CallbackPostModel>(body);


                    string data = JsonConvert.SerializeObject(model);

                    await _context.LogAsync(new SimaLog
                    {
                        SimaLogTypeId = (int)SimaLogTypeEnum.Information,
                        Created = DateTime.Now,
                        IpAddress = ipAddress,
                        Description = "Sorgu callbacke geldi",
                        ErrorMessage = "",
                        RequestBody = body,
                        Headers = headers
                    });


                    //get request body bytes
                    var bodyBuffer = Encoding.UTF8.GetBytes(body);

                    #region Header Validation
                    //ts cert
                    var tcCertResult = HttpContext.Request.Headers.TryGetValue("ts-cert", out StringValues tcCert);
                    if (!tcCertResult)
                    {
                        await _context.LogAsync(new SimaLog
                        {
                            SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                            Created = DateTime.Now,
                            IpAddress = ipAddress,
                            Description = "Header xətası baş verdi.",
                            ErrorMessage = "Sorğu zamanı headerda TS - CERT qeyd edilməyib",
                            RequestBody = body,
                            Headers = headers
                        });

                        return Json(new
                        {
                            errormessage = "Header does not have ts-cert"
                        });
                    }

                    //ts-sign
                    var tcSignResult = HttpContext.Request.Headers.TryGetValue("ts-sign", out StringValues tcSign);

                    if (!tcSignResult)
                    {
                        await _context.LogAsync(new SimaLog
                        {
                            SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                            Created = DateTime.Now,
                            IpAddress = ipAddress,
                            Description = "Header xətası baş verdi.",
                            ErrorMessage = "Sorğu zamanı headerda TS-SIGN qeyd edilməyib",
                            RequestBody = body,
                            Headers = headers
                        });

                        return Json(new
                        {
                            errormessage = "Header does not have ts-sign"
                        });
                    }

                    //ts sign alg
                    var tcSignAlgResult = HttpContext.Request.Headers.TryGetValue("ts-sign-alg", out StringValues tcAlg);
                    if (!tcSignAlgResult)
                    {
                        await _context.LogAsync(new SimaLog
                        {
                            SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                            Created = DateTime.Now,
                            IpAddress = ipAddress,
                            Description = "Header xətası baş verdi.",
                            ErrorMessage = "Sorğu zamanı headerda TS-SIGN-ALG qeyd edilməyib",
                            RequestBody = body,
                            Headers = headers
                        });
                        return Json(new
                        {
                            errormessage = "Header does not have ts-sign-alg"
                        });
                    }
                    #endregion
                    //t

                    #region CERT VALIDATION
                    X509CertificateParser certParser = new X509CertificateParser();

                    var cert = certParser.ReadCertificate(Convert.FromBase64String(tcCert));

                    var signer = SignerUtilities.GetSigner("SHA-256withECDSA");


                    // Verify using the certificate - the certificate's public key is extracted using the GetPublicKey method.
                    signer.Init(false, cert.GetPublicKey());

                    signer.BlockUpdate(bodyBuffer, 0, bodyBuffer.Length);
                    var success = signer.VerifySignature(Convert.FromBase64String(tcSign));

                    if (!success)
                    {
                        await _context.LogAsync(new SimaLog
                        {
                            SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                            Created = DateTime.Now,
                            IpAddress = ipAddress,
                            Description = "Cert validasiya xətası baş verdi.",
                            ErrorMessage = "Cert validasiya edilə bilmədi.",
                            RequestBody = body,
                            Headers = headers
                        });

                        return Json(new
                        {
                            errormessage = "Certificate validation is unsuccessfully"
                        });
                    }
                    #endregion

                }


                #region Model Validation and OperationId Update
                try
                {

                    if (model != null && model.OperationId != null)
                    {
                        CallbackPostModel callbackM = null;
                        bool result = ApplicationData.PostedOperations.TryGetValue(model.OperationId, out callbackM);
                        if (result)
                        {
                            string extension = model.OperationId == "test1contract" ? ".docx" : ".pdf";

                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", model.OperationId + extension));

                            using (FileStream stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", model.OperationId + extension), FileMode.Create))
                            {
                                var buffer = Convert.FromBase64String(model.DataSignature);
                                stream.Write(buffer, 0, buffer.Length);
                            }

                            ApplicationData.PostedOperations[model.OperationId] = model;
                        }
                    }

                    LogToSession(body, "callback", model.OperationId, headers, Guid.NewGuid().ToString());

                    return Json(new
                    {
                        status = "success"
                    });
                }
                catch (Exception exp)
                {
                    await _context.LogAsync(new SimaLog
                    {
                        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                        Created = DateTime.Now,
                        IpAddress = ipAddress,
                        Description = "Xəta baş verdi.",
                        ErrorMessage = exp.Message,
                        RequestBody = body,
                        Headers = headers
                    });
                    return Json(new
                    {
                        status = exp.Message
                    });

                }
                #endregion
            }
            catch (Exception exp)
            {
                await _context.LogAsync(new SimaLog
                {
                    SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                    Created = DateTime.Now,
                    IpAddress = "",
                    Description = "Xəta baş verdi.",
                    ErrorMessage = exp.Message,
                    RequestBody = "",
                    Headers = ""
                });
                return Json(new
                {
                    status = exp.Message
                });

            }


        }

        [HttpGet]
        public IActionResult Clear(string operationId)
        {
            ApplicationData.QrCodes.Remove(operationId);
            ApplicationData.PostedOperations.Remove(operationId);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public static long ConvertDatetimeToUnixTimeStamp(DateTime date)
        {
            DateTime originDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - originDate;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        private string GetHeaders(IHeaderDictionary headersDictionary)
        {
            string headers = String.Empty;
            foreach (var header in headersDictionary)
            {
                headers += header.Key + ":" + header.Value + ",";
            }

            headers = headers.Remove(headers.LastIndexOf(","), 1);
            return headers;
        }

        private void LogToSession(string requestbodyOrQuery, string processDesc, string operationId, string headers, string guid)
        {
            bool result = ApplicationData.Lives.TryGetValue(processDesc + operationId, out var model);
            if (result)
            {
                ApplicationData.Lives.Remove(processDesc + operationId);
            }

            ApplicationData.Lives.Add(processDesc + operationId, new LiveModel
            {
                Body = requestbodyOrQuery,
                Desc = processDesc,
                Guid = guid,
                Headers = headers,
                OperationId = operationId
            });
        }

        private void ClearSession(string operationId)
        {
            ApplicationData.Lives.Remove("getfile" + operationId);
            ApplicationData.Lives.Remove("callback" + operationId);
        }

        public async Task<IActionResult> GetFileByOperationId(string operationId,string type)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var headers = GetHeaders(HttpContext.Request.Headers);


                var displayUrl = HttpContext.Request.GetDisplayUrl();

                await _context.LogAsync(new SimaLog
                {
                    SimaLogTypeId = (int)SimaLogTypeEnum.Information,
                    Created = DateTime.Now,
                    IpAddress = ipAddress,
                    Description = "GetFile üçün sorğu başladı.",
                    ErrorMessage = "",
                    RequestBody = operationId,
                    Headers = headers
                });

                #region Header Validation
                //ts cert
                var tcCertResult = HttpContext.Request.Headers.TryGetValue("ts-cert", out StringValues tcCert);
                if (!tcCertResult)
                {
                    await _context.LogAsync(new SimaLog
                    {
                        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                        Created = DateTime.Now,
                        IpAddress = ipAddress,
                        Description = "Header xətası baş verdi.",
                        ErrorMessage = "Sorğu zamanı headerda TS - CERT qeyd edilməyib",
                        RequestBody = operationId,
                        Headers = headers
                    });

                    return Json(new
                    {
                        errormessage = "Header does not have ts-cert"
                    });
                }

                //ts-sign
                var tcSignResult = HttpContext.Request.Headers.TryGetValue("ts-sign", out StringValues tcSign);

                if (!tcSignResult)
                {
                    await _context.LogAsync(new SimaLog
                    {
                        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                        Created = DateTime.Now,
                        IpAddress = ipAddress,
                        Description = "Header xətası baş verdi.",
                        ErrorMessage = "Sorğu zamanı headerda TS-SIGN qeyd edilməyib",
                        RequestBody = operationId,
                        Headers = headers
                    });

                    return Json(new
                    {
                        errormessage = "Header does not have ts-sign"
                    });
                }


                //ts sign alg
                var tcSignAlgResult = HttpContext.Request.Headers.TryGetValue("ts-sign-alg", out StringValues tcAlg);
                if (!tcSignAlgResult)
                {
                    await _context.LogAsync(new SimaLog
                    {
                        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                        Created = DateTime.Now,
                        IpAddress = ipAddress,
                        Description = "Header xətası baş verdi.",
                        ErrorMessage = "Sorğu zamanı headerda TS-SIGN-ALG qeyd edilməyib",
                        RequestBody = operationId,
                        Headers = headers
                    });
                    return Json(new
                    {
                        errormessage = "Header does not have ts-sign-alg"
                    });
                }
                #endregion


                #region CERT VALIDATION
                X509CertificateParser certParser = new X509CertificateParser();

                var cert = certParser.ReadCertificate(Convert.FromBase64String(tcCert));

                var signer = SignerUtilities.GetSigner("SHA-256withECDSA");

                var pub = cert.GetPublicKey();

                var stpem = ConvertToStringPem(pub);

                // Verify using the certificate - the certificate's public key is extracted using the GetPublicKey method.
                signer.Init(false, cert.GetPublicKey());

                //Get query string from url dynamically
                string queryString = HttpContext.Request.GetEncodedPathAndQuery(); // /Home/GetFile/?tsQuery=asdadsa

                var queryStringBuffer = Encoding.UTF8.GetBytes(queryString);
                signer.BlockUpdate(queryStringBuffer, 0, queryStringBuffer.Length);
                var success = signer.VerifySignature(Convert.FromBase64String(tcSign));

                if (!success)
                {

                    await _context.LogAsync(new SimaLog
                    {
                        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                        Created = DateTime.Now,
                        IpAddress = ipAddress,
                        Description = "Cert validasiya xətaşı baş verdi.",
                        ErrorMessage = "Cert validasiya edilə bilmədi.",
                        RequestBody = operationId,
                        Headers = headers
                    });

                    return Json(new
                    {
                        errormessage = "Certificate validation is unsuccessfully"
                    });

                }
                #endregion

                try
                {


                    string base64 = String.Empty;
                    if (type == OperationType.Auth)
                    {
                        var challange = Guid.NewGuid().ToString();
                        base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(challange));

                    }

                    else
                    {
                        string extension = operationId == "test1contract" ? ".docx" : ".pdf";
                        //You need to find and get correct base64 of file (I will return random file)
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", operationId + extension);

                        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            base64 = Convert.ToBase64String(buffer);
                        }

                    }

                   // LogToSession(opertaio, "getfile", container.SignableContainer.OperationInfo.OperationId, headers, Guid.NewGuid().ToString());

                    return Json(new
                    {
                        filename = type == OperationType.Auth ? "challange" : operationId + ".pdf",
                        data = base64
                    });
                }
                catch (Exception exp)
                {
                    await _context.LogAsync(new SimaLog
                    {
                        SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                        Created = DateTime.Now,
                        IpAddress = ipAddress,
                        Description = "Xəta baş verdi.",
                        ErrorMessage = exp.Message,
                        RequestBody = operationId,
                        Headers = headers
                    });

                    return Json(new
                    {
                        errormessage = exp.Message
                    });
                }
            }

            catch (Exception exp)
            {
                await _context.LogAsync(new SimaLog
                {
                    SimaLogTypeId = (int)SimaLogTypeEnum.Error,
                    Created = DateTime.Now,
                    IpAddress = "",
                    Description = "Xəta baş verdi.",
                    ErrorMessage = exp.Message,
                    RequestBody = operationId,
                    Headers = ""
                });

                return Json(new
                {
                    errormessage = exp.Message
                });
            }
        }




        [HttpGet]
        public JsonResult CheckSession(string processDesc, string operationId)
        {
            bool process = ApplicationData.Lives.TryGetValue(processDesc + operationId, out var model);
            if (!process)
                return Json(new
                {
                    status = 400,
                    data = ""
                });

            ApplicationData.Lives.Remove(processDesc + operationId);

            return Json(new
            {
                status = 200,
                data = new
                {
                    desc = model.Desc,
                    body = model.Body,
                    operationId = model.OperationId,
                    headers = model.Headers,
                    guid = model.Guid
                }
            });

        }


        [HttpPost]
        public async Task<JsonResult> MakeDefaultDataURI(QrCodePostModel model)
        {
            //make ts container
            var tsContainer = await _tsContainerService.MakeTsContainer(model);

            //serialize full jsoncontainer
            string fullJsonContainer = JsonConvert.SerializeObject(tsContainer);

            if (tsContainer.SignableContainer.DataInfo.DataURI == null)
            {
                var jobject = JObject.Parse(fullJsonContainer);
                var s = (JObject)jobject.SelectToken("SignableContainer", false);
                ((JObject)jobject.SelectToken("SignableContainer", false)).Properties()
                         .Where(attr => attr.Name.StartsWith("DataInfo"))
                        .ToList()
                        .ForEach(attr => attr.Remove());
                fullJsonContainer = Regex.Replace(jobject.ToString(), "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
            }

            string base64JsonData = Convert.ToBase64String(Encoding.UTF8.GetBytes(fullJsonContainer));

            //make url for getting file with tsquery params
            var url = "https" + "://" + HttpContext.Request.Host.ToUriComponent() + "/Home/GetFile/?tsquery=" + base64JsonData;

            return Json(new
            {
                status = 200,
                data = url
            });
        }
    }

}
