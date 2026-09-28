using System;
using VfxWaitress.Cli;

namespace VfxWaitress
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.OutputEncoding = new System.Text.UTF8Encoding(false);
            Waitress.UnityProject.HintFromArguments(args);
            try
            {
                return CommandRouter.Run(args, Console.Out, Console.Error);
            }
            catch (VfxWaitressException e)
            {
                Console.Error.WriteLine("error: " + e.Message);
                return 2;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("internal error: " + e);
                return 3;
            }
        }
    }
}
