using Homework3.Generics;
using Homework3.Interfaces;
using Homework3.Models;

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
            if (members.Count == 0) return;

            string animalTypeName = typeof(T).Name;
            logger.Log($"--- Statistics for {animalTypeName} Enclosure (Count: {members.Count}) ---");

            double avgAge = members.Average(a => a.Age);
            logger.Log($"Average age: {avgAge:F1}");

            double avgNameLen = members.Average(a => a.Name.Length);
            logger.Log($"Average name length: {avgNameLen:F1}");

            var allLetters = string.Concat(members.Select(a => a.Name.ToLowerInvariant()));
            var mostPopularLetter = allLetters
                                    .Where(char.IsLetter)
                                    .GroupBy(c => c)
                                    .OrderByDescending(g => g.Count())
                                    .FirstOrDefault()?.Key;

            if (mostPopularLetter != null)
                logger.Log($"Most popular letter in names: '{mostPopularLetter}'");
        }
    }
}
