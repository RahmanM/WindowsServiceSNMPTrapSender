using CommandLine;
using Serilog;
using System;
using System.Net;
using System.Web.Script.Serialization;

namespace SNMP.TrapSender.ConsoleApp
{
    /// <summary>
    /// Command line arguments
    /// 
    /// Example:
    /// 
    /// --service=MySql8.0 --exe=mysqld.exe --ip=192.168.1.168 --port=162 --log=true --type=manual --severity=1 (critical) 2=(major) 3=(minor) etc
    /// </summary>
    public class Options
    {

        [Option("service", Required = true, HelpText = "Please provide Service name e.g. service=MySql.")]
        public string ServiceName { get; set; }

        [Option("exe", Required = true, HelpText = "Please provide exeutable name e.g. exe=MySqld.exe")]
        public string Executable { get; set; }

        [Option("ip", Required = true, HelpText = "IP address to send the trap to.")]
        public string IpToSendTrapTo { get; set; }

        [Option("port", Required = true, HelpText = "Port number to send the trap to.")]
        public int PortToSendTrapTo { get; set; }

        [Option("source", Required = false, HelpText = "Please provide event source e.g. source=SNMP Trap V1")]
        public string Source { get; set; }

        [Option("severity", Required = false, HelpText = "Please provide event severity e.g. severity=1")]
        public int Severity { get; set; }

        [Option("type", Required = false, HelpText = "Please provide event type")]
        public string Type { get; set; }

        [Option("log", Required = false, HelpText = "Please provide log=true for detailed processing log")]
        public bool LogIt { get; set; } = true;
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Initialise logger
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.ColoredConsole()
                    .WriteTo.File("SNMPTrapSenderLogFile.txt", rollingInterval: RollingInterval.Month)
                    .CreateLogger();

                // Parse the parameters and assign to output and then send the SNMP Trap 
                Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (o.LogIt) Log.Information("Starting to send SNMP message!");

                       var output = new Output();
                       output.ServiceName = o.ServiceName;
                       output.Executable = o.Executable;
                       output.HostIpAddress = Utils.GetLocalIPAddress();
                       output.Hostname = Dns.GetHostName();
                       output.Source = o.Source != null ? o.Source : output.Source;
                       output.Severity = o.Severity > 0 ? o.Severity : output.Severity;
                       output.Type = o.Type != null ? o.Type : output.Type;

                       var json = new JavaScriptSerializer().Serialize(output);

                       if (o.LogIt) Log.Information($"Message to send:  {json}");

                       Utils.SendTrap(json, o.IpToSendTrapTo, o.PortToSendTrapTo);

                       if (o.LogIt) Log.Information("SNMP Trap sent successfully!");

                   }).WithNotParsed<Options>(err =>
                   {
                       Log.Error("There was some errors. Please see below: ");

                       foreach (var error in err)
                       {
                           Console.WriteLine(Environment.NewLine + error.ToString());
                       }
                   }
                   );
            }
            catch (Exception ex)
            {
                Log.Error("Error happened: " + ex.Message);
            }


            Console.ReadLine();
        }


    }
}
