#pragma checksum "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "78c28ab452126a71e27d8fead503169135e257d2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_EditItemDetails), @"mvc.1.0.view", @"/Views/Home/EditItemDetails.cshtml")]
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
#line 1 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"78c28ab452126a71e27d8fead503169135e257d2", @"/Views/Home/EditItemDetails.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_EditItemDetails : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.ItemModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "78c28ab452126a71e27d8fead503169135e257d23530", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "78c28ab452126a71e27d8fead503169135e257d24524", async() => {
                WriteLiteral("\r\n<div class=\"edititemdetailsdiv\">\r\n");
#nullable restore
#line 12 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
     using (Html.BeginForm("TryEditDetails", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 15 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.quantity,new { @class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 16 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.quantity,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 20 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.itemName,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 21 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.itemName,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 25 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.description,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 26 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.description,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 30 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.price,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 31 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.price,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 35 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.category,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 36 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.category,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <button style=\"margin-top: 15px\" class=\"btn btn-outline-primary\" type=\"submit\">Edit Details</button>\r\n");
#nullable restore
#line 40 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
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
#line 44 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n    \r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 50 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 53 "C:\Users\shell\RiderProjects\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.ItemModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
