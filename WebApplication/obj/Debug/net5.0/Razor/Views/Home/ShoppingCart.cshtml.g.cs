#pragma checksum "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e73a70f39ba0bc8096650e09ed98ab49c062d500"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_ShoppingCart), @"mvc.1.0.view", @"/Views/Home/ShoppingCart.cshtml")]
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
#line 1 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e73a70f39ba0bc8096650e09ed98ab49c062d500", @"/Views/Home/ShoppingCart.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_ShoppingCart : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.ShoppingCartModel>
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
  
    ViewData["Title"] = "Shopping Cart";

#line default
#line hidden
#nullable disable
            WriteLiteral("<!DOCTYPE html>\r\n\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e73a70f39ba0bc8096650e09ed98ab49c062d5004274", async() => {
                WriteLiteral("\r\n<div class=\"shoppingcartremovediv\">\r\n");
#nullable restore
#line 12 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
     using (Html.BeginForm("TryRemoveItemFromShoppingCart", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <label for=\"storesList\">Items List</label>\r\n");
#nullable restore
#line 15 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
        string[] ItemsListAndStores = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("shoppingCart");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
   Write(Html.DropDownListFor(model => model.itemID, new SelectList(ItemsListAndStores.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
                                                                                                                                                  ;

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button style=\"margin-top: 20px\" class=\"btn btn-outline-primary\" type=\"submit\">Remove Item From Shopping Cart</button>\r\n");
#nullable restore
#line 18 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div class=\"shoppingcartpurchasediv\">\r\n");
#nullable restore
#line 21 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
     using (Html.BeginForm("TrypurchaseItems", "Home", FormMethod.Post))
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
   Write(Html.TextBoxFor(model => model.Address,new { placeholder = "Address",@class="inputclassindex" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button style=\"margin-left: 15px\" class=\"btn btn-outline-primary\" type=\"submit\">Purchase items</button>\r\n");
#nullable restore
#line 25 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n    ");
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
#line 30 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n    \r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 36 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 39 "D:\Software Engineering\שנה ג\סמסטר ו\סדנא ליישום פרויקט תוכנה\WSEP212-18B\WebApplication\Views\Home\ShoppingCart.cshtml"

}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "e73a70f39ba0bc8096650e09ed98ab49c062d5009243", async() => {
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.ShoppingCartModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
