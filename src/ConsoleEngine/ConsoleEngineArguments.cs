using System;

namespace ConsoleEngine
{
    public class ConsoleEngineArguments<TResult> where TResult : class
    {
        public ResultFromUserInput<TResult> ResultFromUserInput { get; set; }
        public UseNonNullResponse<TResult> UseNonNullResponse { get; set; }
        public string OptionalHeaderToShowOnEachLoop { get; set; }

        internal void Verify()
        {
            if (ResultFromUserInput == null)
                throw new ArgumentNullException(string.Empty, "The delegate to get the user input cannot be null");

            if (UseNonNullResponse == null)
                throw new ArgumentNullException(string.Empty, "The delegate to use the (non-null) user input cannot be null");
        }
    }
}
