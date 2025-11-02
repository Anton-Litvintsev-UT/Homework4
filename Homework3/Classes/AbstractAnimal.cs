namespace Homework3.Classes
{
    public abstract class AbstractAnimal(string name, int age, char favoriteChar)
    {

        public static bool IsTeasingLimit { get; private set; } = false;

        public static int TeasingCounter { get; private set; } = 0;

        private int consecutiveCorrectFeeds = 0;

        public static void Increment()
        {
            TeasingCounter++;

            if (TeasingCounter == 3)
                IsTeasingLimit = true;
        }

        public string Name { get; set; } = name;
        public int Age { get; set; } = age;
        public char FavoriteChar { get; set; } = favoriteChar;

        public string Type => $"{GetType().Name}";

        public virtual string Describe()
        {
            return $"This is {Name}, aged {Age}.";
        }
        public abstract string MakeSound();

        public bool Feed(string food)
        {
            if (!string.IsNullOrEmpty(food) && food.Length == 1 && food[0] == FavoriteChar)
            {
                consecutiveCorrectFeeds++;
                return true;
            }
            else
            {
                consecutiveCorrectFeeds = 0;
                return false;
            }
        }

        public bool ShouldActCrazy() => consecutiveCorrectFeeds >= 3;
        public virtual string ActCrazy() => $"{Name} the {Type} is acting crazy!";
    }
}
