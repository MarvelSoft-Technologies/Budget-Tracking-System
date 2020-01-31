using System;
using System.IO;
using System.Reflection;
using System.Text;
using NLog;
using NLog.Targets;
using NPoco;

namespace Budget_Tracking_System
{

    [TableName("Logger")]
    [PrimaryKey("Id")]
    public class Logger
    {
        private static readonly NLog.Logger _log_ = NLog.LogManager.GetCurrentClassLogger();
        public int Id { get; set; }
        public DateTime LogDate { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
        public string LogType { get; set; }

        public static void ConfigureNLog()
        {
            try
            {
                var config = new NLog.Config.LoggingConfiguration();
                var minLogLevel = LogLevel.FromString(Environment.GetEnvironmentVariable("LOGGING_MIN_LEVEL"));
                var maxLogLevel = LogLevel.FromString(Environment.GetEnvironmentVariable("LOGGING_MAX_LEVEL"));

                //console logging
                var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
                config.AddRule(minLogLevel, maxLogLevel, logconsole);

                if (Globals.EnableFileLogging)
                {
                    // file loggin
                    var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "logfile.log" };
                    config.AddRule(minLogLevel, maxLogLevel, logfile);
                }


                if (Globals.EnableDBLogging)
                {
                    //db loggin
                    var logdb = new NLog.Targets.DatabaseTarget("logdb");
                    logdb.CommandText = @" insert into [Log] (MachineName, Logged, Level, Message,Logger, Properties, Callsite, Exception) 
                    values (@MachineName, @Logged, @Level, @Message, @Logger, @Properties, @Callsite, @Exception);";
                    logdb.CommandType = System.Data.CommandType.Text;
                    logdb.ConnectionString = Globals.SkillbaseConnectionString;
                    logdb.Parameters.Add(new DatabaseParameterInfo() { Name = "@MachineName", Layout = "${machinename}" });
                    logdb.Parameters.Add(new DatabaseParameterInfo() { Name = "@Logged", Layout = "${date}" });
                    logdb.Parameters.Add(new DatabaseParameterInfo() { Name = "@Level", Layout = "${level}" });
                    logdb.Parameters.Add(new DatabaseParameterInfo() { Name = "@Message", Layout = "${message}" });
                    logdb.Parameters.Add(new DatabaseParameterInfo() { Name = "@Logger", Layout = "${logger}" });
                    logdb.Parameters.Add(new DatabaseParameterInfo() { Name = "@Properties", Layout = "${all-event-properties:separator=|}" });
                    logdb.Parameters.Add(new DatabaseParameterInfo() { Name = "@Callsite", Layout = "${callsite}" });
                    logdb.Parameters.Add(new DatabaseParameterInfo() { Name = "@Exception", Layout = "${exception:tostring}" });

                    config.AddRule(minLogLevel, maxLogLevel, logdb);
                }


                // Apply config           
                NLog.LogManager.Configuration = config;
            }
            catch (Exception)
            {

            }
        }
        public static void Log(Exception ex, string type)
        {
            try
            {
                if (Globals.EnableElasticSearchLogging)
                {
                    //ElasticSearch.Log(ex, type);
                }
                var logLevel = LogLevel.FromString(type);
                _log_.Log(logLevel, ex, ex.Message);
                    
            }
            catch (Exception)
            {
                
            }
        }


        public static void Error(Exception ex)
        {
            Log(ex, MethodBase.GetCurrentMethod().Name);
        }

        public static void Error(string exception)
        {
            Log(new Exception(exception), MethodBase.GetCurrentMethod().Name);
        }

        public static void Info(string InfoText)
        {
            Log(new Exception(InfoText), MethodBase.GetCurrentMethod().Name);
        }

        public static void Debug(string DebugText)
        {
            Log(new Exception(DebugText), MethodBase.GetCurrentMethod().Name);
        }

      
    }

}