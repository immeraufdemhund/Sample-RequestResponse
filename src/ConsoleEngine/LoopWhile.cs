using System;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public static class LoopWhile
    {
        public static async Task NotQuit<TResult>(ConsoleEngineArguments<TResult> args) where TResult : class
        {
            args.Verify();
            string line = null;
            while(true)
            {
                Console.Clear();
                Console.Write("(quit exits)" + args.OptionalHeaderToShowOnEachLoop ?? ":");
                line = Console.ReadLine();
                if (line == "quit") break;

                try
                {
                    var result = await args.ResultFromUserInput(line);
                    if(result != null)
                        args.UseNonNullResponse(result);
                }
                catch (Exception ex)
                {
                    WriteError(ex);
                    Console.WriteLine("Press Enter to Continue");
                    Console.ReadLine();
                }
            }
        }

        private static void WriteError(Exception ex, int level = 0)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(new string('-', level) + ex.Message);
            if (ex.InnerException != null)
                WriteError(ex.InnerException);

            Console.ForegroundColor = currentColor;
        }
    }
}
