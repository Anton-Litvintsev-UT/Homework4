using Homework3.Interfaces;

namespace Homework3.Models.Animals
{
    public class Penguin : AbstractAnimal, IFlyable, ICrazyAction
    {
        public Penguin() : base() { }
        public Penguin(string name, int age) : base(name, age, 'p') { }
        public override string MakeSound() => "Squawk";
        public string Fly() => "Penguin flaps wings but can't fly high!";
        public override string ActCrazy() => $"{Name} the Penguin is sliding on ice!";
    }
}