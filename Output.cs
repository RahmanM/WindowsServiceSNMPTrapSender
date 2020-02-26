using System;

namespace SNMP.TrapSender.ConsoleApp
{
    public class Output
    {
        public string ServiceName { get; set; }

        public string Executable { get; set; }

        public string Hostname { get; set; }

        public string HostIpAddress { get; set; }

        public string Source { get; set; } = "Custom SNMP Trap V1";

        public string MessageKey { get; set; } = Guid.NewGuid().ToString();

        public int Severity { get; set; } = (int)ErrorSeverity.Critical;

        public string Type { get; set; } = "";
    }
}
