using PracticalAPI.DIKeyedServices;

namespace PracticalAPI.Services
{

    /// <summary>
    /// Using primary constructor of C# 12
    /// </summary>
    /// <param name="greeting"></param>
    public class Welcoming([FromKeyedServices("FormalGreeting")] IGreeting greeting
        , IKeyedServiceProvider keyedServiceProvider)
        : IWelcoming
    {
        
        public string Welcome(string name, string message)
        {
            return $"{greeting.Hello(name)}, {message}";
        }
        public string Welcome2(string name, string type)
        {
            var service = keyedServiceProvider.GetRequiredKeyedService<IGreeting>(type);
            return service.Hello(name);
        }
    }
}
