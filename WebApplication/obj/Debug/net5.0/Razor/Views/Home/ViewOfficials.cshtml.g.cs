#pragma checksum "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "dd2e7082713564fa3253c6a416ea9c78b7324461"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_ViewOfficials), @"mvc.1.0.view", @"/Views/Home/ViewOfficials.cshtml")]
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
#line 1 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
using WSEP212.DomainLayer;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
using WSEP212.ServiceLayer;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"dd2e7082713564fa3253c6a416ea9c78b7324461", @"/Views/Home/ViewOfficials.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_ViewOfficials : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.OfficialsModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/signalr.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!DOCTYPE html>\r\n\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "dd2e7082713564fa3253c6a416ea9c78b73244614427", async() => {
                WriteLiteral("\r\n    <title>title</title>\r\n");
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "dd2e7082713564fa3253c6a416ea9c78b73244615421", async() => {
                WriteLiteral("\r\n<div class=\"viewofficialsdiv\">\r\n");
#nullable restore
#line 14 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {
        string[] usersList = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("officials");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
   Write(Html.DropDownListFor(model => model.UserName, new SelectList(usersList.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
                                                                                                                                           ;
        string name = HttpContextAccessor.HttpContext.Session.GetString("_Name");
        int store = (int)HttpContextAccessor.HttpContext.Session.GetInt32("_StoreID");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 20 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
         if (SystemController.Instance.hasPermission(name, store, Permissions.RemoveStoreManager).getTag())
        {

#line default
#line hidden
#nullable disable
                WriteLiteral("            <button style=\"margin-top: 20px\" class=\"btn btn-outline-primary\" formaction=\"RemoveManager\" type=\"submit\">Remove Manager</button>\r\n");
#nullable restore
#line 23 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
        }

#line default
#line hidden
#nullable disable
#nullable restore
#line 24 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
         if (SystemController.Instance.isStoreOwner(name, store).getTag())
        {

#line default
#line hidden
#nullable disable
                WriteLiteral("            <button style=\"margin-top: 20px\" class=\"btn btn-outline-primary\" type=\"submit\" formaction=\"RemoveOwner\">Remove Owner</button>\r\n");
#nullable restore
#line 27 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
        }

#line default
#line hidden
#nullable disable
#nullable restore
#line 27 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
         
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div class=\"viewofficialsdiv\">\r\n");
#nullable restore
#line 31 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {
        string[] usersList = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("officials");
        Permissions[] arr = new[]
        {
            Permissions.StorageManagment, Permissions.AppointStoreManager, Permissions.AppointStoreOwner,
            Permissions.EditManagmentPermissions, Permissions.GetOfficialsInformation, Permissions.RemoveStoreManager,
            Permissions.StorePoliciesManagement, Permissions.GetStorePurchaseHistory
        };
        string[] permissions = Array.ConvertAll(arr, value => value.ToString());
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 41 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
   Write(Html.DropDownListFor(model => model.UserName, new SelectList(usersList.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 41 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
                                                                                                                                           ;
        
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 43 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
   Write(Html.DropDownListFor(model => model.Permission, new SelectList(permissions.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 43 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
                                                                                                                                               ;
        

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button style=\"margin-top: 20px\" class=\"btn btn-outline-primary\" formaction=\"AddManagerPermission\" type=\"submit\">Add Manager Permission</button>\r\n");
#nullable restore
#line 46 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n");
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
            WriteLiteral("\r\n</html>\r\n");
#nullable restore
#line 50 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
 if (TempData["alert"] != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">  \r\n            window.onload = function() {  \r\n                alert(\"");
#nullable restore
#line 54 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
                  Write(TempData["alert"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");  \r\n            };  \r\n        </script>  \r\n");
#nullable restore
#line 57 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "dd2e7082713564fa3253c6a416ea9c78b732446112610", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"

    <script>
        let connection = new signalR.HubConnectionBuilder().withUrl(""/notifications"").build();
     
        connection.on('sendToUser', data => {
             alert(data)
        });
        
        connection.start().then(() => {
            console.log(""connection started"");
            connection.invoke('GetConnectionId').then(function (connectionId) {
                //alert(connectionId);
            });
        });
      </script>
");
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IHttpContextAccessor HttpContextAccessor { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.OfficialsModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
