using System.Threading.Tasks;

namespace ConsoleEngine
{
    public delegate Task<TResult> ResultFromUserInput<TResult>(string userInput) where TResult : class;
    public delegate void UseNonNullResponse<TResult>(TResult result) where TResult : class;
}
