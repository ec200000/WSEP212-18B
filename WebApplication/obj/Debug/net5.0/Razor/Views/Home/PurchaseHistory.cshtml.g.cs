#pragma checksum "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c4b411bc6881fb1391bb99659a37886bbf92db1b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_PurchaseHistory), @"mvc.1.0.view", @"/Views/Home/PurchaseHistory.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
using System.Collections.Concurrent;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
using System;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c4b411bc6881fb1391bb99659a37886bbf92db1b", @"/Views/Home/PurchaseHistory.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_PurchaseHistory : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.PurchaseModel>
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 5 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
  
    ViewData["Title"] = "Purchase History";

#line default
#line hidden
#nullable disable
            WriteLiteral("<!DOCTYPE html>\r\n\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c4b411bc6881fb1391bb99659a37886bbf92db1b3987", async() => {
                WriteLiteral("\r\n");
                WriteLiteral("    <div>\r\n");
#nullable restore
#line 14 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
         using (Html.BeginForm("TryShowPurchaseHistory", "Home", FormMethod.Post))
        {

            string purchases = HttpContextAccessor.HttpContext.Session.GetString("_History");
            string[] singleItems = purchases.Split(";");
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 19 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
       Write(Html.DropDownListFor(model => model.itemInfo, new SelectList(singleItems.OfType<string>().ToList())));

#line default
#line hidden
#nullable disable
#nullable restore
#line 19 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
                                                                                                                 ;

#line default
#line hidden
#nullable disable
                WriteLiteral("            <button class=\"w-10 btn btn-lg btn-primary\" type=\"submit\">Add Review</button>\r\n");
#nullable restore
#line 21 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
        }

#line default
#line hidden
#nullable disable
                WriteLiteral("    </div>\r\n");
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
            WriteLiteral("\r\n</html>\r\n\r\n");
#nullable restore
#line 26 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 31 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 34 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\PurchaseHistory.cshtml"

}

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.PurchaseModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
