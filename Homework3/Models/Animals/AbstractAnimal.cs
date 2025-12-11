using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework3.Models.Animals
{
    public abstract class AbstractAnimal
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public char FavoriteChar { get; set; }
        public string Type => GetType().Name;

        public int? EnclosureId { get; set; }

        [ForeignKey("EnclosureId")]
        public virtual Enclosure? Enclosure { get; set; }

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

        public virtual string Describe()
        {
            string baseDescription = $"This is {Name}, aged {Age}.";

            if (Enclosure != null && !string.IsNullOrEmpty(Enclosure.Name))
            {
                baseDescription += $"\nCurrently residing in the {Enclosure.Name}.";
            }

            return baseDescription;
        }
        public abstract string MakeSound();

        public virtual string ActCrazy() => $"{Name} the {Type} is acting crazy!";
    }
}
