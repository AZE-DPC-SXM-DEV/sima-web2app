#pragma checksum "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1a136a40e68a6cd3ff98a79efe822bd2fe7b8021"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_QrCodeGenerate), @"mvc.1.0.view", @"/Views/Home/QrCodeGenerate.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
using Web2App.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1a136a40e68a6cd3ff98a79efe822bd2fe7b8021", @"/Views/Home/QrCodeGenerate.cshtml")]
    #nullable restore
    public class Views_Home_QrCodeGenerate : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<QrCodePostModel>
    #nullable disable
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!DOCTYPE html>\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "1a136a40e68a6cd3ff98a79efe822bd2fe7b80213038", async() => {
                WriteLiteral(@"
    <title>Web2App</title>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3"" crossorigin=""anonymous"">
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "1a136a40e68a6cd3ff98a79efe822bd2fe7b80214428", async() => {
                WriteLiteral("\r\n\r\n    <div class=\"container\">\r\n        <div class=\"row\">\r\n            <div class=\"col-6 mx-auto my-5\">\r\n                <form");
                BeginWriteAttribute("action", " action=\"", 629, "\"", 674, 1);
#nullable restore
#line 17 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
WriteAttributeValue("", 638, Url.Action("QrCodeGenerate","Home"), 638, 36, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" method=\"post\">\r\n                    ");
#nullable restore
#line 18 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
               Write(Html.ValidationSummary());

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
                    <div class=""mb-3"">
                        <label for=""type"" class=""form-label"">Operation Type</label>
                        <select class=""form-control"" id=""type"" name=""Type"">
                            <option value=""Auth"">Auth</option>
                            <option value=""Sign"">Sign</option>
                        </select>
                    </div>
                    <div class=""mb-3"">
                        <label for=""operationId"" class=""form-label"">OperationId</label>
                        <input name=""OperationId""");
                BeginWriteAttribute("value", " value=\"", 1310, "\"", 1336, 1);
#nullable restore
#line 28 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
WriteAttributeValue("", 1318, Model.OperationId, 1318, 18, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" type=""text"" class=""form-control"" id=""operationId"" aria-describedby=""emailHelp"">
                    </div>
                    <div class=""mb-3"">
                        <label for=""secret"" class=""form-label"">Secret key</label>
                        <input name=""SecretKey""");
                BeginWriteAttribute("value", " value=\"", 1617, "\"", 1641, 1);
#nullable restore
#line 32 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
WriteAttributeValue("", 1625, Model.SecretKey, 1625, 16, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" type=""text"" class=""form-control"" id=""secret"" aria-describedby=""emailHelp"">
                    </div>
                    <div class=""mb-3"">
                        <label for=""clientId"" class=""form-label"">Client Id</label>
                        <input name=""ClientId""");
                BeginWriteAttribute("value", " value=\"", 1917, "\"", 1940, 1);
#nullable restore
#line 36 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
WriteAttributeValue("", 1925, Model.ClientId, 1925, 15, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" type=""text"" class=""form-control"" id=""clientId"" aria-describedby=""emailHelp"">
                    </div>

                    <div class=""mb-3"">
                        <label for=""Nbf"" class=""form-label"">NbfUTC</label>
                        <input name=""Start"" type=""date"" class=""form-control"" id=""NbfUTC"" aria-describedby=""emailHelp"">
                    </div>
                    <div class=""mb-3"">
                        <label for=""Exp"" class=""form-label"">ExpUTC</label>
                        <input name=""End"" type=""date"" class=""form-control"" id=""ExpUTC"" aria-describedby=""emailHelp"">
                    </div>
                    <div class=""mb-3"">
                        <label for=""Icon"" class=""form-label"">Icon</label>
                        <input name=""IconUri""");
                BeginWriteAttribute("value", " value=\"", 2736, "\"", 2758, 1);
#nullable restore
#line 49 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
WriteAttributeValue("", 2744, Model.IconUri, 2744, 14, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" type=""text"" class=""form-control"" id=""Icon"" aria-describedby=""emailHelp"">
                    </div>
                    <div class=""mb-3"">
                        <label for=""Callback"" class=""form-label"">Callback</label>
                        <input name=""CallBackUrl""");
                BeginWriteAttribute("value", " value=\"", 3034, "\"", 3060, 1);
#nullable restore
#line 53 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
WriteAttributeValue("", 3042, Model.CallBackUrl, 3042, 18, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" type=""text"" class=""form-control"" id=""Callback"" aria-describedby=""emailHelp"">
                    </div>
                    <div class=""mb-3"">
                        <label for=""Assignee "" class=""form-label"">Assignee</label>
                        <input name=""Assignee""");
                BeginWriteAttribute("value", " value=\"", 3338, "\"", 3361, 1);
#nullable restore
#line 57 "C:\Users\tarlan.usubov\Desktop\gitlab.azintelecom.az\biosign-scanme\Web2App\Views\Home\QrCodeGenerate.cshtml"
WriteAttributeValue("", 3346, Model.Assignee, 3346, 15, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" type=""text"" class=""form-control"" id=""Assignee"" placeholder=""add fin codes with comma"" aria-describedby=""emailHelp"">
                    </div>
                    <button type=""button"" class=""btn btn-primary"">Submit</button>
                </form>
            </div>
        </div>
    </div>


");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<QrCodePostModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
