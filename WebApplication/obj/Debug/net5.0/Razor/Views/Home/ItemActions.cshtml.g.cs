#pragma checksum "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fcdcbb440ba575d71f8e97cee0689e80cbd37438"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_ItemActions), @"mvc.1.0.view", @"/Views/Home/ItemActions.cshtml")]
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
#line 1 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fcdcbb440ba575d71f8e97cee0689e80cbd37438", @"/Views/Home/ItemActions.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c58edd3a6b5e9ca63b10fbb3cbb99bbeb61e4bcd", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_ItemActions : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<WebApplication.Models.ItemModel>
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fcdcbb440ba575d71f8e97cee0689e80cbd374383731", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fcdcbb440ba575d71f8e97cee0689e80cbd374384725", async() => {
                WriteLiteral("\r\n<div class=\"itemactionsadddiv\">\r\n");
#nullable restore
#line 13 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
     using (Html.BeginForm("TryAddItem", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 16 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.EditorFor(model => model.quantity,new { htmlAttributes = new  {@class="inputclassitemactions" , @placeholder = "Quantity"}}));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 17 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.ValidationMessageFor(model => model.quantity));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 21 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.TextBoxFor(model => model.itemName,new { placeholder = "Item name",@class="inputclassitemactions" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 22 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.ValidationMessageFor(model => model.itemName));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 26 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.TextBoxFor(model => model.description,new { placeholder = "Description",@class="inputclassitemactions" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 27 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.ValidationMessageFor(model => model.description));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 31 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.EditorFor(model => model.price,new { htmlAttributes = new  {@class="inputclassitemactions" , @placeholder = "Price"}}));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 32 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.ValidationMessageFor(model => model.price));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <div>\r\n            ");
#nullable restore
#line 36 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.TextBoxFor(model => model.category,new { placeholder = "Category",@class="inputclassitemactions" }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n            ");
#nullable restore
#line 37 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
       Write(Html.ValidationMessageFor(model => model.category));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n        </div>\r\n");
                WriteLiteral("        <button class=\"btn btn-outline-primary\" type=\"submit\" type=\"submit\">Add Item To Store</button>\r\n");
#nullable restore
#line 41 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div class=\"itemactionsremovediv\">\r\n");
#nullable restore
#line 44 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
     using (Html.BeginForm("TryRemoveItem", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <label for=\"storesList\">Items List</label>\r\n");
#nullable restore
#line 47 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
        string str = "items" + HttpContextAccessor.HttpContext.Session.GetInt32("_StoreID"); 
        int[] ItemsList = HttpContextAccessor.HttpContext.Session.GetObject<int[]>(str); 
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 49 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
   Write(Html.DropDownListFor(model => model.itemID, new SelectList(ItemsList.OfType<int>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 49 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
                                                                                                                                      ;


#line default
#line hidden
#nullable disable
                WriteLiteral("        <button style=\"margin-top: 20px\" class=\"btn btn-outline-primary\" type=\"submit\" type=\"submit\">Remove Item From Store</button>\r\n");
#nullable restore
#line 52 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
    }

#line default
#line hidden
#nullable disable
                WriteLiteral("</div>\r\n<div class=\"itemactionseditdiv\">\r\n");
#nullable restore
#line 55 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
     using (Html.BeginForm("EditItemDetails", "Home", FormMethod.Post))
    {

#line default
#line hidden
#nullable disable
                WriteLiteral("        <label for=\"storesList\">Items List</label>\r\n");
#nullable restore
#line 58 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
        string str = "items" + HttpContextAccessor.HttpContext.Session.GetInt32("_StoreID"); 
        int[] ItemsList = HttpContextAccessor.HttpContext.Session.GetObject<int[]>(str); 
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 60 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
   Write(Html.DropDownListFor(model => model.itemID, new SelectList(ItemsList.OfType<int>().ToList()), new {@class = "custom-select"}));

#line default
#line hidden
#nullable disable
#nullable restore
#line 60 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
                                                                                                                                      ;



#line default
#line hidden
#nullable disable
                WriteLiteral("        <button style=\"margin-top: 20px\" class=\"btn btn-outline-primary\" type=\"submit\" type=\"submit\">Edit Item\'s Details</button>\r\n");
#nullable restore
#line 64 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
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
#line 68 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
 if (ViewBag.Alert != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <script type=\"text/javascript\">\r\n    \r\n\r\n            window.onload = function() {\r\n                alert(\"");
#nullable restore
#line 74 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"
                  Write(ViewBag.Alert);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n            };\r\n    </script>\r\n");
#nullable restore
#line 77 "C:\Users\ec200\RiderProjects\WSEP212-18B\WebApplication\Views\Home\ItemActions.cshtml"

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
