using SnmpSharpNet;
using System;
using System.Net;
using System.Net.Sockets;

namespace SNMP.TrapSender.ConsoleApp
{
    public class Utils
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
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
                             new Oid("1.3.6.1.2.1.1.1.0"), new IpAddress(GetLocalIPAddress()),
                             SnmpConstants.LinkDown, 0, 13432, col);
        }
    }
}
