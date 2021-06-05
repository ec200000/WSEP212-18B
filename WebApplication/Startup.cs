using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using WebApplication.Communication;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer.ExternalDeliverySystem;
using WSEP212.DomainLayer.ExternalPaymentSystem;
using WSEP212.DomainLayer.SystemLoggers;
using WSEP212.ServiceLayer;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SystemController.Instance.initRepo();
            readConfigurationFile();
        }

        public IConfiguration Configuration { get; }
        
        public static void readConfigurationFile()
        {
            try
            {
                string jsonFilePath = "configuration.json";
                string json = File.ReadAllText(jsonFilePath);
                dynamic array = JsonConvert.DeserializeObject(json);
                string tests = array.tests;
                string database = "", server = "", userID = "", password = "";
                if (tests.Equals("false"))
                {
                    var db = array.db[0];
                    database = db.database;
                    server = db.server;
                    userID = db.userID;
                    password = db.password;
                }
                else
                {
                    SystemDBAccess.mock = true;
                    var db = array.db[1];
                    database = db.database;
                    server = db.server;
                    userID = db.userID;
                    password = db.password;
                }
                string link = array.external;
                SystemDBAccess.database = database;
                SystemDBAccess.server = server;
                SystemDBAccess.userID = userID;
                SystemDBAccess.password = password;

                var systemDbAccess = SystemDBAccess.Instance;

                DeliverySystemAPI.webAddressAPI = link;
                PaymentSystemAPI.webAddressAPI = link;
                
                var deliverySystemAPI = DeliverySystemAPI.Instance;
                var paymentSystemAPI = PaymentSystemAPI.Instance;
            }
            catch (Exception e)
            {
                var msg = e.Message + " ";
                var inner = e.InnerException;
                if (inner != null)
                    msg += inner.Message;
                Logger.Instance.writeErrorEventToLog(msg);
                System.Environment.Exit(-1);
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllersWithViews();
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate(options => // code from ASP.NET Core sample
                {
                    options.AllowedCertificateTypes = CertificateTypes.All;
                    options.Events = new CertificateAuthenticationEvents
                    {
                        OnCertificateValidated = context =>
                        {
                            var validationService =
                                context.HttpContext.RequestServices.GetService<MyCertificateValidationService>();
 
                            if (validationService != null && validationService.ValidateCertificate(context.ClientCertificate))
                            {
                                var claims = new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                                };
 
                                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                context.Success();
                            }
                            else
                            {
                                context.Fail("invalid cert");
                            }
 
                            return Task.CompletedTask;
                        }
                    };
                });
 
            services.AddAuthorization();
            services.AddControllersWithViews();
            
            services.AddDistributedMemoryCache();  
            services.AddSession(options => {  
                options.IdleTimeout = TimeSpan.FromMinutes(20);//You can set Time   
            });  
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseSession();  

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("notifications");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}