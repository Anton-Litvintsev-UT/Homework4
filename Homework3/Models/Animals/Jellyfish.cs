using Homework3.Interfaces;

namespace Homework3.Models.Animals
{
    public class Jellyfish : AbstractAnimal, ICrazyAction
    {
        public Jellyfish() : base() { }
        public Jellyfish(string name, int age) : base(name, age, 'j') { }
        public override string MakeSound() => "Bloop bloop";
        public override string ActCrazy() => $"{Name} the Jellyfish is spinning!";
    }
}