using Homework3.Models.Animals;
using System.Text;

namespace Homework3.Services
{
    public class ZooStatisticsService
    {
        public string GetStatistics(IEnumerable<AbstractAnimal> animals)
        {
            if (animals == null || !animals.Any()) return "No animals in the Zoo.";

            var sb = new StringBuilder();
            sb.AppendLine("=== ZOO LINQ STATISTICS ===");

            double avgAge = animals.Average(a => a.Age);
            sb.AppendLine($"Average Age: {avgAge:F1} years");

            var typeGroups = animals.GroupBy(a => a.GetType().Name);
            foreach (var group in typeGroups)
            {
                sb.AppendLine($" - {group.Key}: {group.Count()}");
            }

            var oldest = animals.OrderByDescending(a => a.Age).FirstOrDefault();
            if (oldest != null)
                sb.AppendLine($"Oldest Animal: {oldest.Name} ({oldest.Age})");

            var mostPopularChar = animals
                .SelectMany(a => a.Name.ToLower())
                .Where(char.IsLetter)
                .GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            if (mostPopularChar != null)
                sb.AppendLine($"Most common letter in names: '{mostPopularChar}'");

            return sb.ToString();
        }
    }
}