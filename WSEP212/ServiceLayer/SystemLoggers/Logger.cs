using System;
using System.IO;
using Serilog;

namespace WSEP212.DomainLayer.SystemLoggers
{
    public class Logger
    {
        private static readonly Lazy<Logger> lazy
            = new Lazy<Logger>(() => new Logger());

        public static Logger Instance
            => lazy.Value;

        private Logger() { }

        public void writeInformationEventToLog(String info)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile("LogFileInformation.txt", shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}{NewLine}") //writing the info to the same file
                .CreateLogger();

            Log.Information(info);
            Log.CloseAndFlush();
        }
        
        public void writeWarningEventToLog(String info)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile("LogFileWarnings.txt", shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}{NewLine}") //writing the error to the same file
                .CreateLogger();

            Log.Warning(info);
            Log.CloseAndFlush();
        }
        
        public void writeErrorEventToLog(String info)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile("LogFileErrors.txt", shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}{NewLine}") //writing the error to the same file
                .CreateLogger();

            Log.Error(info);
            Log.CloseAndFlush();
        }
    }
}