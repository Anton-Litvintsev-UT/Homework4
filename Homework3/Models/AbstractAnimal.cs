using System.ComponentModel.DataAnnotations;
using Homework3.Interfaces;

namespace Homework3.Models
{
    public abstract class AbstractAnimal
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public char FavoriteChar { get; set; }
        public string Type => GetType().Name;

        private int consecutiveCorrectFeeds = 0;

        protected AbstractAnimal() { }

        protected AbstractAnimal(string name, int age, char favoriteChar)
        {
            Name = name;
            Age = age;
            FavoriteChar = favoriteChar;
        }
        public bool Feed(string food)
        {
            if (string.IsNullOrWhiteSpace(food))
            {
                consecutiveCorrectFeeds = 0;
                return false;
            }

            char c = char.ToLower(food.Trim()[0]);
            if (c == char.ToLower(FavoriteChar))
            {
                consecutiveCorrectFeeds++;
                return true;
            }

            consecutiveCorrectFeeds = 0;
            return false;
        }
        public int ConsecutiveCorrectFeeds => consecutiveCorrectFeeds;

        // Animal acts crazy if fed correctly 3 times in a row
        public bool ShouldActCrazy() => consecutiveCorrectFeeds >= 3;

        // Reset counter after acting crazy
        public void ResetConsecutiveCorrectFeeds() => consecutiveCorrectFeeds = 0;

        public virtual string Describe() => $"This is {Name}, aged {Age}.";
        public abstract string MakeSound();

        public virtual string ActCrazy() => $"{Name} the {Type} is acting crazy!";
    }
}
