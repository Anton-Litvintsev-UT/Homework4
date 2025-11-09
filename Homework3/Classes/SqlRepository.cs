using Homework3.Generics;
using Microsoft.Data.SqlClient;

namespace Homework3.Classes
{
    class SqlRepository<T> : IRepository<T> where T : AbstractAnimal
    {
        private readonly string _connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=Homework3Db;Trusted_Connection=True;";

        public event Action<AbstractAnimal>? ItemAdded;
        public event Action<AbstractAnimal>? ItemRemoved;

        public SqlRepository()
        {
            InitDb();
        }

        private void InitDb()
        {
            using var masterConn =
                new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=master;Trusted_Connection=True;");
            masterConn.Open();

            using (var cmd = masterConn.CreateCommand())
            {
                cmd.CommandText = @"
IF DB_ID(N'Homework3Db') IS NULL
    CREATE DATABASE [Homework3Db];";
                cmd.ExecuteNonQuery();
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd2 = conn.CreateCommand();
            cmd2.CommandText = @"
IF OBJECT_ID(N'dbo.Animals', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Animals
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Age INT NOT NULL,
        TypeName NVARCHAR(200) NOT NULL,
        FavoriteChar NCHAR(1) NULL,
        CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
    );
END";
            cmd2.ExecuteNonQuery();
        }

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO dbo.Animals (Name, Age, TypeName, FavoriteChar) VALUES (@name, @age, @type, @fav)";
            cmd.Parameters.AddWithValue("@name", item.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@age", item.Age);
            cmd.Parameters.AddWithValue("@type", item.GetType().Name);
            cmd.Parameters.AddWithValue("@fav", (object?)item.FavoriteChar ?? DBNull.Value);

            cmd.ExecuteNonQuery();
            ItemAdded?.Invoke(item);
        }

        public void Remove(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM dbo.Animals WHERE Name = @name AND TypeName = @type";
            cmd.Parameters.AddWithValue("@name", item.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@type", item.GetType().Name);

            cmd.ExecuteNonQuery();
            ItemRemoved?.Invoke(item);
        }

        public IEnumerable<T> GetAll()
        {
            var list = new List<T>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Name, Age, TypeName, FavoriteChar FROM dbo.Animals ORDER BY CreatedAt ASC";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var name = reader.GetString(0);
                var age = reader.GetInt32(1);
                var typeName = reader.GetString(2);
                var favCharObj = reader.IsDBNull(3) ? null : reader.GetString(3);
                char favChar = favCharObj != null && favCharObj.Length > 0 ? favCharObj[0] : default;

                var animal = CreateAnimalInstance(typeName, name, age);
                if (animal is T typedAnimal)
                {
                    typedAnimal.FavoriteChar = favChar;
                    list.Add(typedAnimal);
                }
            }

            return list;
        }

        public T? Find(Func<T, bool> predicate)
        {
            foreach (var a in GetAll())
            {
                if (predicate(a)) return a;
            }
            return null;
        }

        private AbstractAnimal? CreateAnimalInstance(string typeName, string name, int age)
        {
            return typeName switch
            {
                nameof(Penguin) => new Penguin(name, age),
                nameof(Whale) => new Whale(name, age),
                nameof(Jellyfish) => new Jellyfish(name, age),
                _ => null
            };
        }
    }
}
