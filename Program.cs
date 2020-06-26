using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;



namespace Redistest
{


   public class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Employee(string EmployeeId, string Name, int Age)
        {
            this.Id = EmployeeId;
            this.Name = Name;
            this.Age = Age;
        }
    }

    class Program
    {

    private static IConfigurationRoot Configuration { get; set; }
    const string SecretName = "CacheConnection";

    private static void InitializeConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();

        Configuration = builder.Build();
    }

    private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
        string cacheConnection = Configuration[SecretName];
        return ConnectionMultiplexer.Connect(cacheConnection);
    });

    public static ConnectionMultiplexer Connection
    {
        get
        {
            return lazyConnection.Value;
        }
    }

        static void Main(string[] args)
        {
            InitializeConfiguration();

        // Connection refers to a property that returns a ConnectionMultiplexer
        // as shown in the previous example.
        IDatabase cache = lazyConnection.Value.GetDatabase();

        // Store .NET object to cache
        Employee e007 = new Employee("007", "Davide Columbo", 100);
        Console.WriteLine("Cache response from storing Employee .NET object : " + 
        cache.StringSet("e007", JsonConvert.SerializeObject(e007)));

        // Retrieve .NET object from cache
        Employee e007FromCache = JsonConvert.DeserializeObject<Employee>(cache.StringGet("e007"));
        Console.WriteLine("Deserialized Employee .NET object :\n");
        Console.WriteLine("\tEmployee.Name : " + e007FromCache.Name);
        Console.WriteLine("\tEmployee.Id   : " + e007FromCache.Id);
        Console.WriteLine("\tEmployee.Age  : " + e007FromCache.Age + "\n");

        

        lazyConnection.Value.Dispose();
            }
        }
}
