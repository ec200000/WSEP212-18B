#pragma checksum "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1b9e9d719d69f5c082a09eb69d9d2c1ce9a2585f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1b9e9d719d69f5c082a09eb69d9d2c1ce9a2585f", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.UserModel>
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
#line 1 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "Home Page";

#line default
#line hidden
#nullable disable
            WriteLiteral("<!DOCTYPE html>\r\n\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "1b9e9d719d69f5c082a09eb69d9d2c1ce9a2585f3338", async() => {
                WriteLiteral("\r\n");
                WriteLiteral("<div class=\"indexdiv\">\r\n    <h2 id=\"h2index\">\r\n        Welcome! Please Register:\r\n    </h2>\r\n");
#nullable restore
#line 13 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
     using (Html.BeginForm("Subscribe", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 16 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
       Write(Html.TextBoxFor(model => model.UserName,new { placeholder = "User Name",@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 17 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
       Write(Html.ValidationMessageFor(model => model.UserName));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 21 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
       Write(Html.TextBoxFor(model => model.Age,new { placeholder = "Age",@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 22 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
       Write(Html.ValidationMessageFor(model => model.Age));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 26 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
       Write(Html.PasswordFor(model => model.Password,new { placeholder = "Password",@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 27 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
       Write(Html.ValidationMessageFor(model => model.Password));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <button class=\"btn btn-outline-primary\" type=\"submit\">Register</button>\r\n");
#nullable restore
#line 31 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div>\r\n    <h2 id=\"orInIndexpage\">OR</h2>\r\n</div>\r\n\r\n<div class=\"indexcontinueasquestdiv\">\r\n");
#nullable restore
#line 38 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
     using (Html.BeginForm("ContinueAsGuest", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 41 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
       Write(Html.TextBoxFor(model => model.UserName,new { placeholder = "User Name",@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 42 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
       Write(Html.ValidationMessageFor(model => model.UserName));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <button class=\"btn btn-outline-primary\" type=\"submit\">Continue As Guest</button>\r\n");
#nullable restore
#line 46 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
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
#line 51 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
 if (TempData["alert"] != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">  \r\n            window.onload = function() {  \r\n                alert(\"");
#nullable restore
#line 55 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
                  Write(TempData["alert"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");  \r\n            };  \r\n        </script>  \r\n");
#nullable restore
#line 58 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\Index.cshtml"
}

#line default
#line hidden
#nullable disable
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.UserModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
