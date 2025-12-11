using Homework3.Interfaces;
using Homework3.Models.Animals;

namespace Homework3.Services
{
    public class EnclosureManager<T>(IRepository<AbstractAnimal> repository) where T : AbstractAnimal
    {
        private readonly IRepository<AbstractAnimal> _repository = repository;
        private readonly Random _rand = new();

        public event Action<T>? AnimalJoined;
        public event Action<T>? FoodDropped;

        public void NotifyAnimalAdded(T animal)
        {
            AnimalJoined?.Invoke(animal);
        }

        public async Task DropFoodAsync(IEnumerable<T> animals, string food, ILogger logger)
        {
            if (!animals.Any()) return;

            foreach (var animal in animals)
            {
                int delay = _rand.Next(500, 2000);
                await Task.Delay(delay);

                FoodDropped?.Invoke(animal);
            }
        }

        public IEnumerable<T> GetEnclosureMembers()
        {
            return _repository.GetAll().OfType<T>();
        }

        public void LogStatistics(ILogger logger)
        {
            var members = GetEnclosureMembers().ToList();
            logger.Log($"Enclosure {typeof(T).Name}: {members.Count} member(s).");
        }
    }
}
