#pragma checksum "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "11f048e4cd5c7e98c4b6e2687a40c42f94c3a6a2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_PurchaseTypes), @"mvc.1.0.view", @"/Views/Home/PurchaseTypes.cshtml")]
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
#line 1 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\_ViewImports.cshtml"
using WebApplication;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\_ViewImports.cshtml"
using WebApplication.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"11f048e4cd5c7e98c4b6e2687a40c42f94c3a6a2", @"/Views/Home/PurchaseTypes.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_PurchaseTypes : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.PurchaseTypesModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "11f048e4cd5c7e98c4b6e2687a40c42f94c3a6a23715", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "11f048e4cd5c7e98c4b6e2687a40c42f94c3a6a24709", async() => {
                WriteLiteral("\r\n<div class=\"purchasepredicatediv\">\r\n    <input class=\"btn btn-outline-primary\" type=\"button\" onclick=\"location.href=\'/Home/AddPredicate\';\" value=\"Add Predicate\" />\r\n</div>\r\n");
                WriteLiteral("<div class=\"purchasepredicatediv\">\r\n");
#nullable restore
#line 15 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {
        string[] types = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("storepurchasetypes");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
   Write(Html.DropDownListFor(model => model.purchaseType, new SelectList(types.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
                                                                                                                                           ;


#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"btn btn-outline-primary\" formaction=\"TryRemovePurchaseType\" type=\"submit\">Remove Purchase Type</button>\r\n");
#nullable restore
#line 21 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
        
        string[] types2 = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("purchasetypes");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
   Write(Html.DropDownListFor(model => model.purchaseType2, new SelectList(types2.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
                                                                                                                                             ;


#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"btn btn-outline-primary\" formaction=\"TryAddPurchaseType\" type=\"submit\">Add Purchase Type</button>\r\n");
#nullable restore
#line 26 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
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
            WriteLiteral("\r\n</html>\r\n\r\n");
#nullable restore
#line 31 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
 if (TempData["alert"] != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">  \r\n            window.onload = function() {  \r\n                alert(\"");
#nullable restore
#line 35 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
                  Write(TempData["alert"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");  \r\n            };  \r\n        </script>  \r\n");
#nullable restore
#line 38 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\PurchaseTypes.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.PurchaseTypesModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
