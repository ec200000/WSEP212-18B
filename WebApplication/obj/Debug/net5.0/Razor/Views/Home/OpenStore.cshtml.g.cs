#pragma checksum "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0c6f7c8dd21b1c6cfeaf1ffe53bebdbd25486988"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_OpenStore), @"mvc.1.0.view", @"/Views/Home/OpenStore.cshtml")]
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
#line 1 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0c6f7c8dd21b1c6cfeaf1ffe53bebdbd25486988", @"/Views/Home/OpenStore.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_OpenStore : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.StoreModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0c6f7c8dd21b1c6cfeaf1ffe53bebdbd254869883961", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0c6f7c8dd21b1c6cfeaf1ffe53bebdbd254869884955", async() => {
                WriteLiteral("\r\n<div class=\"openstorediv\">\r\n");
#nullable restore
#line 10 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
     using (Html.BeginForm("TryOpenStore", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 13 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
       Write(Html.TextBoxFor(model => model.storeName,new { placeholder = "Store Name",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 14 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
       Write(Html.ValidationMessageFor(model => model.storeName));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 18 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
       Write(Html.TextBoxFor(model => model.storeAddress,new { placeholder = "Store Address",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 19 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
       Write(Html.ValidationMessageFor(model => model.storeAddress));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 23 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
       Write(Html.TextBoxFor(model => model.salesPolicy,new { placeholder = "Sales Policy",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 24 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
       Write(Html.ValidationMessageFor(model => model.salesPolicy));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 28 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
       Write(Html.TextBoxFor(model => model.purchasePolicy,new { placeholder = "Purchase Policy",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 29 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
       Write(Html.ValidationMessageFor(model => model.purchasePolicy));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <button class=\"btn btn-outline-primary\" type=\"submit\">OpenStore</button>\r\n");
#nullable restore
#line 33 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
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
#line 38 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
 if (ViewBag.Alert!= null)  
{  

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">  \r\n            window.onload = function() {  \r\n                alert(\"");
#nullable restore
#line 42 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");  \r\n            };  \r\n        </script>  \r\n");
#nullable restore
#line 45 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212-18B\WebApplication\Views\Home\OpenStore.cshtml"
} 

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0c6f7c8dd21b1c6cfeaf1ffe53bebdbd2548698810947", async() => {
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
        
        connection.on('save', data => {
            alert(data)
        });
     
        connection.start().then(() => {
            console.log(""connection started"");
            connection.invoke('GetConnectionId').then(function (connectionId) {
                alert(connectionId);
            });
        });
     
        /*function sendMessage() {
          var msg = document.getElementById(""txtMessage"").value;
          connection.invoke('send', msg);
        }*/
      </script>
");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.StoreModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
