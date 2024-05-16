using PracticalAPI.DIKeyedServices;

namespace PracticalAPI.Services
{
    public class Welcoming : IWelcoming
    {
        private readonly IGreeting _greeting;
        public Welcoming([FromKeyedServices("FormalGreeting")]IGreeting greeting)
        {
            _greeting = greeting;
        }
        public string Welcome(string name, string message)
        {
            return $"{_greeting.Hello(name)}, {message}";
        }
    }
}
