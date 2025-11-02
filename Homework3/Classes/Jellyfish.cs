using Homework3.Interfaces;

namespace Homework3.Classes
{
    class Jellyfish : AbstractAnimal, ICrazyAction
    {
        public Jellyfish(string name, int age) : base(name, age, 'j')
        {
            Name = name;
            Age = age;
            FavoriteChar = 'j';
        }

        public override string MakeSound() => "Blub-blub";
        public override string ActCrazy() => $"{Name} the Jellyfish is spinning!";
    }
}
