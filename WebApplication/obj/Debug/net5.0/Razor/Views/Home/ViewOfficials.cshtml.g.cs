#pragma checksum "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f6a4f485dc6a987b41e56a74f7e42fb7f4bf5557"
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f6a4f485dc6a987b41e56a74f7e42fb7f4bf5557", @"/Views/Home/ViewOfficials.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_ViewOfficials : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.OfficialsModel>
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
            WriteLiteral("<!DOCTYPE html>\r\n\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f6a4f485dc6a987b41e56a74f7e42fb7f4bf55573719", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f6a4f485dc6a987b41e56a74f7e42fb7f4bf55574713", async() => {
                WriteLiteral("\r\n<div>\r\n");
#nullable restore
#line 13 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {
        string[] usersList = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("officials");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
   Write(Html.DropDownListFor(model => model.UserName, new SelectList(usersList.OfType<string>().ToList())));

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
                                                                                                           ;
        

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" formaction=\"RemoveManager\" type=\"submit\">Remove Manager</button>\r\n");
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" type=\"submit\" formaction=\"RemoveOwner\">Remove Owner</button>\r\n");
#nullable restore
#line 21 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div>\r\n");
#nullable restore
#line 24 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {
        string[] usersList = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("officials");
        Permissions[] arr = new[]
        {
            Permissions.StorageManagment, Permissions.AppointStoreManager, Permissions.AppointStoreOwner,
            Permissions.EditManagmentPermissions, Permissions.GetOfficialsInformation, Permissions.RemoveStoreManager,
            Permissions.StorePoliciesManagement, Permissions.GetStorePurchaseHistory
        };
        int[] permissions = Array.ConvertAll(arr, value => (int) value);
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 34 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
   Write(Html.DropDownListFor(model => model.UserName, new SelectList(usersList.OfType<string>().ToList())));

#line default
#line hidden
#nullable disable
#nullable restore
#line 34 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
                                                                                                           ;
        
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 36 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
   Write(Html.DropDownListFor(model => model.Permission, new SelectList(permissions.OfType<int>().ToList())));

#line default
#line hidden
#nullable disable
#nullable restore
#line 36 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
                                                                                                            ;
        

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" formaction=\"AddManagerPermission\" type=\"submit\">Add Manager Permission</button>\r\n");
#nullable restore
#line 39 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
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
#line 43 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n    \r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 49 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 52 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ViewOfficials.cshtml"

}

#line default
#line hidden
#nullable disable
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
