
using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using WSEP212.DomainLayer;

namespace WebApplication.Publisher
{
    public class ChartNotifications : Hub
    {
        PieChart pieChart = new PieChart();
        
        public ChartNotifications() {}    
        
     
        public void InitChartData()    
        {
            pieChart.SetPieChartData();
        }       
        
    }
}