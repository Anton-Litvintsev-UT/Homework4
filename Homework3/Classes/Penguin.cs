using Homework3.Interfaces;

namespace Homework3.Classes
{
    class Penguin : AbstractAnimal, IFlyable, ICrazyAction
    {
        public Penguin(string name, int age) : base(name, age, 'p')
        {
            Name = name;
            Age = age;
            FavoriteChar = 'p';
        }
        public override string MakeSound() => "Brrr-brrr-chirp";

        public string Fly() => $"Penguin {Name} uses jetpack to fly";

        public override string ActCrazy() => $"{Name} the Penguin is sliding on ice!";
    }
}
