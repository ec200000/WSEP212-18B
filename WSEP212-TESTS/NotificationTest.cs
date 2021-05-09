using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.Publisher;
using Xunit;

namespace WSEP212_TEST
{
    public class NotificationTest
    {
        [Fact]
        public async Task reply_with_the_same_message_when_invoke_send_method()
        {
            TestServer server = null;
            var message = "Integration Testing in Microsoft AspNetCore SignalR";
            var echo = string.Empty;
            var webHostBuilder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSignalR();
                })
                .Configure (app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => { endpoints.MapHub<NotificationHub>("test"); });
                });

            server = new TestServer(webHostBuilder);
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44391/test",
                    o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .Build();

            connection.On<string>("OnMessageRecieved", msg =>
            {
                echo = msg;
            });

            await connection.StartAsync();
            await connection.InvokeAsync("Send", message);

            echo.Should().Be(message);
        }
    }
}