using Homework3.Generics;
using Homework3.Models;
using Microsoft.EntityFrameworkCore;

namespace Homework3.Data
{
    public class AnimalRepository(ZooDbContext context) : IRepository<AbstractAnimal>
    {
        private readonly ZooDbContext _context = context;

        public event Action<AbstractAnimal>? ItemAdded;
        public event Action<AbstractAnimal>? ItemRemoved;

        public void Add(AbstractAnimal item)
        {
            _context.Animals.Add(item);
            _context.SaveChanges();
            ItemAdded?.Invoke(item);
        }

        public void Remove(AbstractAnimal item)
        {
            _context.Animals.Remove(item);
            _context.SaveChanges();
            ItemRemoved?.Invoke(item);
        }

        public IEnumerable<AbstractAnimal> GetAll() => [.. _context.Animals];
        public AbstractAnimal? Find(Func<AbstractAnimal, bool> predicate)
        {
            return _context.Animals
                           .AsNoTracking()
                           .FirstOrDefault(predicate);
        }
    }
}