using PracticalAPI.DIKeyedServices;

namespace PracticalAPI.Services
{

    /// <summary>
    /// Using primary constructor of C# 12
    /// </summary>
    /// <param name="formalGreeting"></param>
    public class Welcoming([FromKeyedServices("FormalGreeting")] IGreeting formalGreeting
        , [FromKeyedServices("InformalGreeting")] IGreeting informalGreeting)
        : IWelcoming
    {
        
        public string WelcomeFormalWay(string name, string message)
        {
            return $"{formalGreeting.Hello(name)}, {message}";
        }

        public string WelcomeInFormalWay(string name)
        {
            return informalGreeting.Hello(name);
        }
    }


}
