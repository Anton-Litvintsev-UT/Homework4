using Homework3.Interfaces;

namespace Homework3.Classes
{
    class Whale : AbstractAnimal, ICrazyAction
    {
        public Whale(string name, int age) : base(name, age, 'w')
        {
            Name = name;
            Age = age;
            FavoriteChar = 'w';
        }

        public override string MakeSound() => "Ooo-aaah-ooo-ahh";
       
        public override string ActCrazy() => $"{Name} is swimming wildly!";
    }
}
