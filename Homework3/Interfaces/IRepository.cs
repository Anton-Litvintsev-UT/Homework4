namespace Homework3.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T item);
        void Remove(T item);
        IEnumerable<T> GetAll();
        T? Find(Func<T, bool> predicate);

        event Action<T>? ItemAdded;
        event Action<T>? ItemRemoved;
    }
}
