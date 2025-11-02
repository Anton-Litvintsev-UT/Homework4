using Homework3.Classes;

namespace Homework3.Generics
{

    class Enclosure<T> : Repository<T> where T : AbstractAnimal
    {
        private readonly Random _rand = new();
        public event Action<T>? AnimalJoinedInSameEnclosure;
        public event Action<T>? FoodDropped;

        public new void Add(T item)
        {
            base.Add(item);
            AnimalJoinedInSameEnclosure?.Invoke(item);
        }

        public new void Remove(T item)
        {
            base.Remove(item);
        }
        public async Task DropFoodAsync()
        {
            var animals = GetAll().ToList();
            if (animals.Count == 0) return;

            foreach (var animal in animals)
            {
                int delay = _rand.Next(1000, 4000); // random eating interval
                await Task.Delay(delay);

                FoodDropped?.Invoke(animal);
            }
        }
    }
}
