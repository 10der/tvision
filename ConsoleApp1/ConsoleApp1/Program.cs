using TurboVision.App;

namespace ConsoleApp1
{
    class Program
    {
        public static object Desktop { get; internal set; }

        static void Main(string[] args)
        {
            Application D = new TVDemo();
            D.Run();
            D.Done();
        }
    }
}
