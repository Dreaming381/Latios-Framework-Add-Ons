using System;
using ModelWaitress.Cli;

namespace ModelWaitress
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.OutputEncoding = new System.Text.UTF8Encoding(false);
            var output = new System.IO.StreamWriter(Console.OpenStandardOutput(), new System.Text.UTF8Encoding(false), 1 << 16) { AutoFlush = true };
            try
            {
                return CommandRouter.Run(args, output, Console.Error);
            }
            catch (Waitress.WaitressException e)
            {
                output.Flush();
                Console.Error.WriteLine("error: " + e.Message);
                return 2;
            }
            catch (Exception e)
            {
                output.Flush();
                Console.Error.WriteLine("internal error: " + e);
                return 3;
            }
            finally
            {
                output.Flush();
            }
        }
    }
}
