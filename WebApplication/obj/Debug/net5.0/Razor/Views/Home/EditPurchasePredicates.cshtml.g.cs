#pragma checksum "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "386fc441d1330b08557bed009d0edda0bb2860e6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_EditPurchasePredicates), @"mvc.1.0.view", @"/Views/Home/EditPurchasePredicates.cshtml")]
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
#line 1 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
using WSEP212.DomainLayer;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"386fc441d1330b08557bed009d0edda0bb2860e6", @"/Views/Home/EditPurchasePredicates.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_EditPurchasePredicates : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.SalesModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "386fc441d1330b08557bed009d0edda0bb2860e64524", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "386fc441d1330b08557bed009d0edda0bb2860e65518", async() => {
                WriteLiteral("\r\n<div class=\"editsalepredicatesdiv\">\r\n");
#nullable restore
#line 13 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"btn btn-outline-primary\" type=\"submit\" formaction=\"StoreActions\">Add Sale Predicate (not working)</button>\r\n");
#nullable restore
#line 16 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div class=\"editsalepredicatesdiv\">\r\n");
#nullable restore
#line 19 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <label for=\"storesList\">Sale predicates</label>\r\n");
#nullable restore
#line 22 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
        string[] predList = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("sale_predicates");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
   Write(Html.DropDownListFor(model => model.predicate, new SelectList(predList.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
                                                                                                                                           ;


#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"btn btn-outline-primary\" formaction=\"TryRemoveSalePredicate\" type=\"submit\">Remove Sale Predicate</button>\r\n");
                WriteLiteral("        <button class=\"btn btn-outline-primary\" type=\"submit\" formaction=\"StoreActions\">Add Sale Condition (not working)</button>\r\n");
#nullable restore
#line 28 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div class=\"editsalepredicatesdiv\">\r\n");
#nullable restore
#line 31 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
     using (Html.BeginForm("ItemActions", "Home", FormMethod.Post))
    {
        string[] predList = HttpContextAccessor.HttpContext.Session.GetObject<string[]>("sale_predicates");
        SaleCompositionType[] arr = new[]
        {
            SaleCompositionType.XorComposition, SaleCompositionType.MaxComposition, SaleCompositionType.DoubleComposition
        };
        string[] permissions = Array.ConvertAll(arr, value => value.ToString());

#line default
#line hidden
#nullable disable
                WriteLiteral("        <label for=\"storesList\">First predicate</label>\r\n");
#nullable restore
#line 40 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
   Write(Html.DropDownListFor(model => model.firstPred, new SelectList(predList.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 40 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
                                                                                                                                           
        ;

#line default
#line hidden
#nullable disable
                WriteLiteral("        <label for=\"storesList\">Second predicate</label>\r\n");
#nullable restore
#line 43 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
   Write(Html.DropDownListFor(model => model.secondPred, new SelectList(predList.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 43 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
                                                                                                                                            
        ;

#line default
#line hidden
#nullable disable
                WriteLiteral("        <label for=\"storesList\">compositionType</label>\r\n");
#nullable restore
#line 46 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
   Write(Html.DropDownListFor(model => model.compositionType, new SelectList(permissions.OfType<string>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 46 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
                                                                                                                                                    
        ;


#line default
#line hidden
#nullable disable
                WriteLiteral("        <button style=\"margin-top: 20px\" class=\"btn btn-outline-primary\" formaction=\"ComposeSalePredicates\" type=\"submit\">Compose Sale Predicates (without xor)</button>\r\n");
#nullable restore
#line 50 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
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
#line 54 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n    \r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 60 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 63 "C:\Users\irisk\OneDrive\Documents\university\third year - second semester\Sadna\WSEP212\WebApplication\Views\Home\EditPurchasePredicates.cshtml"

}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "386fc441d1330b08557bed009d0edda0bb2860e614303", async() => {
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