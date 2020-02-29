using CommandLine;
using Serilog;
using SnmpSharpNet;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web.Script.Serialization;

namespace SNMP.TrapSender.ConsoleApp
{
    public static class TrapSender
    {
        public static void SendTrap(string[] args)
        {
            try
            {
                var messageSent = false;

                while (messageSent == false)
                {
                    // Initialise logger
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.ColoredConsole()
                        .WriteTo.File("SNMPTrapSenderLogFile.txt", rollingInterval: RollingInterval.Day)
                        .WriteTo.EventLog("SNMP Trap Sender", manageEventSource: true)
                        .WriteTo.Udp("localhost", 7071, AddressFamily.InterNetwork)
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

                           SendTrap(json, o.IpToSendTrapTo, o.PortToSendTrapTo);

                           if (o.LogIt) Log.Information("SNMP Trap sent successfully!");

                           Thread.Sleep(2000);

                           messageSent = true;

                       })
                       .WithNotParsed<Options>(err =>
                       {
                           Log.Error("There was some errors. Please see below: ");

                           foreach (var error in err)
                           {
                               Log.Error(Environment.NewLine + error.ToString());
                           }
                       }
                       );

                }
            }
            catch (Exception ex)
            {
                Log.Error("Error happened: " + ex.Message);
            }
        }


        public static void SendTrap(string message, string ipAddressToSend, int portToSend)
        {
            TrapAgent agent = new TrapAgent();

            // Variable Binding collection to send with the trap
            VbCollection col = new VbCollection();
            col.Add(new Oid("1.3.6.1.2.1.1.1.0"), new OctetString(message));
            col.Add(new Oid("1.3.6.1.2.1.1.2.0"), new Oid("1.3.6.1.9.1.1.0"));
            col.Add(new Oid("1.3.6.1.2.1.1.3.0"), new TimeTicks(2324));
            col.Add(new Oid("1.3.6.1.2.1.1.4.0"), new OctetString(DateTime.Now.ToString()));
            col.Add(new Oid("1.3.6.1.2.1.1.5.0"), new OctetString("1"));

            // Send the trap to the localhost port 162
            agent.SendV1Trap(new IpAddress(ipAddressToSend), portToSend, "public",
                             new Oid("1.3.6.1.2.1.1.1.0"), new IpAddress(Utils.GetLocalIPAddress()),
                             SnmpConstants.LinkDown, 0, 13432, col);
        }

    }
}
