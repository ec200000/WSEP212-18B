#pragma checksum "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c22ae3d8cf273dd4aecae39ff61cc1f4f05c0507"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_StoreActions), @"mvc.1.0.view", @"/Views/Home/StoreActions.cshtml")]
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
#line 1 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c22ae3d8cf273dd4aecae39ff61cc1f4f05c0507", @"/Views/Home/StoreActions.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_StoreActions : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.StoreModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c22ae3d8cf273dd4aecae39ff61cc1f4f05c05073513", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c22ae3d8cf273dd4aecae39ff61cc1f4f05c05074507", async() => {
                WriteLiteral("\r\n<div>\r\n    <input class=\"w-10 btn btn-lg btn-primary\" type=\"button\" onclick=\"location.href=\'/Home/OpenStore\';\" value=\"Open Store\" />\r\n</div>\r\n<div>\r\n");
#nullable restore
#line 15 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {
        int[] StoresList = HttpContextAccessor.HttpContext.Session.GetObject<int[]>("stores");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml"
   Write(Html.DropDownListFor(model => model.storeID, new SelectList(StoresList.OfType<int>().ToList())));

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml"
                                                                                                        ;
        

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" formaction=\"ItemActions\" type=\"submit\">See Items In Store</button>\r\n");
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" type=\"submit\" formaction=\"GetStoreInformation\">Get Store Information</button>\r\n");
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" type=\"submit\" formaction=\"GetStoreInformation\">Edit Store\'s Purchase Predicates</button>\r\n");
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" type=\"submit\" formaction=\"GetStoreInformation\">Edit Store\'s Sale Predicates</button>\r\n");
#nullable restore
#line 27 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml"

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
#line 32 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n    \r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 38 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 41 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\StoreActions.cshtml"

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.StoreModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
