namespace PracticalAPI.DIKeyedServices
{
    public class FormalGreeting : IGreeting
    {
        public string Hello(string name)
        {
            return $"Good morning {name}, it's our pleasure to have you here";
        }
    }
}
