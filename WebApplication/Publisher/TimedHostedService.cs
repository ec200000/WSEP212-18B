using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WSEP212.DomainLayer;

namespace WebApplication.Publisher
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly IHubContext<ChartNotifications> _hub;
        private Timer _timer;
        private volatile bool _sendingChartData = false;    
        private readonly object _chartUpateLock = new object();
        PieChart pieChart = new PieChart();
        
        public TimedHostedService(IHubContext<ChartNotifications> hub)
        {
            _hub = hub;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(ChartTimerCallBack, null, 1000,1000);

            return Task.CompletedTask;
        }

        private void ChartTimerCallBack(object state)    
        {
            if (_sendingChartData)    
            {    
                return;    
            }    
            lock (_chartUpateLock)    
            {    
                if (!_sendingChartData)    
                {    
                    _sendingChartData = true;
                    pieChart.SetPieChartData();
                    sendChartData(pieChart);    
                    _sendingChartData = false;    
                }    
            }
        }    
        
        public async void sendChartData(PieChart chart)
        {
            await _hub.Clients.All.SendAsync("updateChart", chart.GetPieChartData());
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}