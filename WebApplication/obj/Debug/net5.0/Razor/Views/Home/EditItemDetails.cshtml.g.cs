#pragma checksum "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "493d815b10ae99831b07f09f3efb3b23cf403ef1"
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
#line 1 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"493d815b10ae99831b07f09f3efb3b23cf403ef1", @"/Views/Home/EditItemDetails.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_EditItemDetails : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.ItemModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "493d815b10ae99831b07f09f3efb3b23cf403ef14177", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "493d815b10ae99831b07f09f3efb3b23cf403ef15171", async() => {
                WriteLiteral("\r\n<div class=\"edititemdetailsdiv\">\r\n");
#nullable restore
#line 12 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
     using (Html.BeginForm("TryEditDetails", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 15 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.quantity,new { @class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 16 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.quantity,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 20 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.itemName,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 21 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.itemName,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 25 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.description,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 26 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.description,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 30 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.price,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 31 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.price,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 35 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.LabelFor(model => model.category,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 36 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
       Write(Html.EditorFor(model => model.category,new {@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <button style=\"margin-top: 15px\" class=\"btn btn-outline-primary\" type=\"submit\">Edit Details</button>\r\n");
#nullable restore
#line 40 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
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
#line 44 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n    \r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 50 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 53 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\EditItemDetails.cshtml"

}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "493d815b10ae99831b07f09f3efb3b23cf403ef111718", async() => {
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.ItemModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
