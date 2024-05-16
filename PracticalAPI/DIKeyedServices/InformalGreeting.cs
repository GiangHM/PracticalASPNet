namespace PracticalAPI.DIKeyedServices
{
    public class InformalGreeting : IGreeting
    {
        public string Hello(string name)
        {
            return $"Hello {name}, welcome to my site";
        }
    }
}
