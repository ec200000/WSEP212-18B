
using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using WSEP212.DomainLayer;

namespace WebApplication.Publisher
{
    public class ChartNotifications : Hub
    {
        // Create the instance of ChartDataUpdate    
        // Send Data every 1 seconds    
        readonly int _updateInterval = 1000;       
        //Timer Class    
        private Timer _timer;    
        private volatile bool _sendingChartData = false;    
        private readonly object _chartUpateLock = new object();
        PieChart pieChart = new PieChart();
        
        public ChartNotifications() {}    
        
     
        public void InitChartData()    
        {
            //Show Chart initially when InitChartData called first time    
            pieChart.SetPieChartData();
            //sendChartData(pieChart);
            //Call GetChartData to send Chart data every chosen interval
            //GetChartData();
        }       
        
        /*public void GetChartData()    
        {
            _timer = new Timer(ChartTimerCallBack, null, _updateInterval, _updateInterval);
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
           await Clients.All.SendAsync("updateChart", chart.GetPieChartData());
        }*/
    }
}