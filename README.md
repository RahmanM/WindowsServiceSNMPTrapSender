# WindowsServiceSNMPTrapSender
Send snmp traps based on windows service failure

#example
- In MySql Windows Services, select the properties and then select the Recovery Tab
- In the First Failure, Second Failure and Subsequent select Run A Program
- In Run the Program select this executable e.g. D:\SNMP.TrapSender.Console.exe
- In the command line parameters put --service=MySql8.0 --exe=mysqld.exe --ip=192.168.1.168 --port=162
- Open windows task manager and kill the for example mysqld.exe
- Trap will be send to Ip and Port as per above values

# Other command line arguments available:

```
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
    
    
```
