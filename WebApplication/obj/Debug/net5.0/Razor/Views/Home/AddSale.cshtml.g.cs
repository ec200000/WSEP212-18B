#pragma checksum "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a444aed8891a8938153cfbce724488f16753a608"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_AddSale), @"mvc.1.0.view", @"/Views/Home/AddSale.cshtml")]
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
#line 1 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
using WSEP212.DomainLayer;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
using WSEP212.DomainLayer.SalePolicy;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a444aed8891a8938153cfbce724488f16753a608", @"/Views/Home/AddSale.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_AddSale : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.SalesModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a444aed8891a8938153cfbce724488f16753a6084667", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a444aed8891a8938153cfbce724488f16753a6085661", async() => {
                WriteLiteral("\r\n<h2>Please choose one of the sales options:</h2>\r\n<div class=\"editsalepredicatesdiv\">\r\n");
#nullable restore
#line 15 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
     using (Html.BeginForm("TryAddSale", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <h5>1. Sale On Item</h5>\r\n        <label for=\"storesList\">Items list</label>\r\n");
#nullable restore
#line 19 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
        string[] predList = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("itemsList");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 20 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
   Write(Html.DropDownListFor(model => model.itemID, new SelectList(predList.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 20 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
                                                                                                                                        
        ;
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 22 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
   Write(Html.TextBoxFor(model => model.salePercentage,new { placeholder = "Sale Percentage",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
   Write(Html.TextBoxFor(model => model.saleDescription,new { placeholder = "Sale Description",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"btn btn-outline-primary\" type=\"submit\">Add Sale</button>\r\n");
#nullable restore
#line 25 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div class=\"editsalepredicatesdiv\">\r\n");
#nullable restore
#line 28 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
     using (Html.BeginForm("TryAddSale", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <h5>2. Sale On All Store</h5>\r\n");
#nullable restore
#line 31 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
   Write(Html.TextBoxFor(model => model.salePercentage,new { placeholder = "Sale Percentage",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 32 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
   Write(Html.TextBoxFor(model => model.saleDescription,new { placeholder = "Sale Description",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"btn btn-outline-primary\" formaction=\"TryAddSale\" type=\"submit\">Add Sale</button>\r\n");
#nullable restore
#line 34 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div class=\"editsalepredicatesdiv\">\r\n");
#nullable restore
#line 37 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
     using (Html.BeginForm("TryAddSale", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <h5>3. Sale On Category</h5>\r\n        <label for=\"storesList\">Category List</label>\r\n");
#nullable restore
#line 41 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
        ItemCategory[] arr = new[]
        {
            ItemCategory.AllCategories, ItemCategory.Dairy, ItemCategory.Meat, ItemCategory.Clothing,
            ItemCategory.Footwear, ItemCategory.Cleaners, ItemCategory.Vegetables, ItemCategory.Electronics, ItemCategory.Health,
            ItemCategory.Sport, ItemCategory.Dinnerware, ItemCategory.Fruits, ItemCategory.Snacks,ItemCategory.Pastries,
            ItemCategory.Drinks, ItemCategory.Tools, ItemCategory.Other
        };
        string[] itemCategory = Array.ConvertAll(arr, value => value.ToString());
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 49 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
   Write(Html.DropDownListFor(model => model.category, new SelectList(itemCategory.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 49 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
                                                                                                                                              
        ;
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 51 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
   Write(Html.TextBoxFor(model => model.salePercentage,new { placeholder = "Sale Percentage",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
#nullable restore
#line 52 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
   Write(Html.TextBoxFor(model => model.saleDescription,new { placeholder = "Sale Description",@class="inputclassopenstore" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"btn btn-outline-primary\" formaction=\"TryAddSale\" type=\"submit\">Add Sale</button>\r\n");
#nullable restore
#line 54 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
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
#line 58 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
 if (TempData["alert"] != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 63 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"
                  Write(TempData["alert"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 66 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\AddSale.cshtml"

}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a444aed8891a8938153cfbce724488f16753a60814874", async() => {
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.SalesModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
