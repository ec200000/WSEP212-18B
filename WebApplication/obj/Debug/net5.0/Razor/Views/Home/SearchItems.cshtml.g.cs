#pragma checksum "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c7a6686f6b90f09853a872421a94fdfd8c212541"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_SearchItems), @"mvc.1.0.view", @"/Views/Home/SearchItems.cshtml")]
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
#line 1 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\_ViewImports.cshtml"
using WebApplication.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
using WSEP212.ConcurrentLinkedList;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
using WSEP212.DomainLayer;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
using System.Collections.Concurrent;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c7a6686f6b90f09853a872421a94fdfd8c212541", @"/Views/Home/SearchItems.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_SearchItems : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.SearchModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c7a6686f6b90f09853a872421a94fdfd8c2125414109", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c7a6686f6b90f09853a872421a94fdfd8c2125415103", async() => {
                WriteLiteral("\r\n<div>\r\n");
#nullable restore
#line 15 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
     using (Html.BeginForm("TryAddItemToShoppingCart", "Home", FormMethod.Post))
    {
        ConcurrentDictionary<Store,ConcurrentLinkedList<Item>> ItemsList = HttpContextAccessor.HttpContext.Session.GetObject<ConcurrentDictionary<Store,ConcurrentLinkedList<Item>>>("allitems!");
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
   Write(Html.DropDownListFor(model => model.itemChosen, new SelectList(Model.items.OfType<string>().ToList())));

#line default
#line hidden
#nullable disable
#nullable restore
#line 19 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
   Write(Html.TextBoxFor(model => model.quantity));

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" type=\"submit\">Add Item To Shopping Cart</button>\r\n");
#nullable restore
#line 21 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div>\r\n");
#nullable restore
#line 24 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
     using (Html.BeginForm("TrySearchItems", "Home", FormMethod.Post))
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 26 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
   Write(Html.TextBoxFor(model => model.minPrice));

#line default
#line hidden
#nullable disable
#nullable restore
#line 27 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
   Write(Html.TextBoxFor(model => model.maxPrice));

#line default
#line hidden
#nullable disable
#nullable restore
#line 28 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
   Write(Html.TextBoxFor(model => model.keyWords));

#line default
#line hidden
#nullable disable
#nullable restore
#line 29 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
   Write(Html.TextBoxFor(model => model.category));

#line default
#line hidden
#nullable disable
#nullable restore
#line 30 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
   Write(Html.TextBoxFor(model => model.itemName));

#line default
#line hidden
#nullable disable
                WriteLiteral("        <button class=\"w-10 btn btn-lg btn-primary\" type=\"submit\">Search Items</button>\r\n");
#nullable restore
#line 32 "C:\Users\talsk\RiderProjects\WSEP212-18B\WebApplication\Views\Home\SearchItems.cshtml"
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
            WriteLiteral("\r\n</html>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<WebApplication.Models.SearchModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
