#pragma checksum "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9b8ff8bdd6c6c2687ab2a60906ce79701778d7ba"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_ItemReview), @"mvc.1.0.view", @"/Views/Home/ItemReview.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9b8ff8bdd6c6c2687ab2a60906ce79701778d7ba", @"/Views/Home/ItemReview.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_ItemReview : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.ReviewModel>
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
#line 1 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
  
    ViewData["Title"] = "Item Review";

#line default
#line hidden
#nullable disable
            WriteLiteral("<!DOCTYPE html>\r\n\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9b8ff8bdd6c6c2687ab2a60906ce79701778d7ba3372", async() => {
                WriteLiteral("\r\n");
                WriteLiteral("    <div>\r\n");
#nullable restore
#line 10 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
         using (Html.BeginForm("TryReviewItem", "Home", FormMethod.Post))
        {

            

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
       Write(Html.TextBoxFor(model => model.review));

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
       Write(Html.ValidationMessageFor(model => model.review));

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
       Write(Html.TextBoxFor(model => model.itemID));

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
       Write(Html.ValidationMessageFor(model => model.itemID));

#line default
#line hidden
#nullable disable
#nullable restore
#line 19 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
       Write(Html.TextBoxFor(model => model.storeID));

#line default
#line hidden
#nullable disable
#nullable restore
#line 20 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
       Write(Html.ValidationMessageFor(model => model.storeID));

#line default
#line hidden
#nullable disable
                WriteLiteral("            <button class=\"w-10 btn btn-lg btn-primary\" type=\"submit\">Review Item</button>\r\n");
#nullable restore
#line 22 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
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
#line 27 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 32 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 35 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemReview.cshtml"

}

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.ReviewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
