namespace SNMP.TrapSender.ConsoleApp
{

    class Program
    {
        static void Main(string[] args)
        {
            TrapSender.SendTrap(args);

            // Console.ReadLine();
        }

    }
}
