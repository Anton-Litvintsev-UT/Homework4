using Homework3.Interfaces;

namespace Homework3.Models
{
    public class Whale : AbstractAnimal, ICrazyAction
    {
        public Whale() : base() { }
        public Whale(string name, int age) : base(name, age, 'w') { }
        public override string MakeSound() => "Moooooaaaaannn";
        public override string ActCrazy() => $"{Name} is swimming wildly!";
    }
}