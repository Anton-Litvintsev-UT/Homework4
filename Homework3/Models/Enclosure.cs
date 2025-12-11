using Homework3.Models.Animals;
using System.ComponentModel.DataAnnotations;

namespace Homework3.Models
{
    public class Enclosure
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<AbstractAnimal> Animals { get; set; } = [];

        public Enclosure() { }
        public Enclosure(string name)
        {
            Name = name;
        }
    }
}