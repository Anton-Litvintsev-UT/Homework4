using Homework3.Generics;


namespace Homework3.Classes
{
    class Repository<T> : IRepository<T>
    {
        private readonly List<T> _items = new List<T>();

        public event Action<T>? ItemAdded;
        public event Action<T>? ItemRemoved;

        public void Add(T item)
        {
            _items.Add(item);
            ItemAdded?.Invoke(item);
        }

        public void Remove(T item)
        {
            _items.Remove(item);
            ItemRemoved?.Invoke(item);
        }

        public IEnumerable<T> GetAll()
        {
            return _items;
        }

        public T? Find(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }
    }
}
