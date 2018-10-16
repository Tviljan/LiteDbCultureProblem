using System;

namespace CultureLiteDbProblem
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using LiteDB;

    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fi"); 

            Console.WriteLine($"Running with {Thread.CurrentThread.CurrentCulture.Name} culture");

            var file = "AddItem.txt";

            var dbName = Guid.NewGuid().ToString();

            var listOfIds = new List<string>();

            var notFoundId = new List<string>();

            int itemCount = 0;
            
            Console.WriteLine($"Create/Open db and add items from txt file");
            using (var repo = new LiteRepository($"Filename = {dbName}.db;Journal = false;async=true;Cache Size=0;Flush= true;Timeout=0:10:00;mode=Exclusive;"))
            {
                using (var reader = new System.IO.StreamReader(file))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        var id = line.Trim();
                        listOfIds.Add(id);
                        var wrapper = new MongoDbDataWrapper<string>()
                        {
                            Id = id
                        };

                        var wrappedData = BsonMapper.Global.ToDocument(wrapper);
                        
                        var col = repo.Database.GetCollection("DataObjects");
                        col.Insert(wrappedData);
                        itemCount++;
                    }
                }
            }
            
            Console.WriteLine($"Item Count { itemCount}");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-gb");
            Console.WriteLine($"Switch to with { Thread.CurrentThread.CurrentCulture.Name} culture");
            
            Console.WriteLine($"Open db");
            using (var repo = new LiteRepository($"Filename = {dbName}.db;Journal = false;async=true;Cache Size=0;Flush= true;Timeout=0:10:00;mode=Exclusive;"))
            {
                
                var c = repo.Database.GetCollection("DataObjects");
                var items = c.FindAll();
                
                Console.WriteLine($"Item Count { items.Count()}");

                foreach (var id in listOfIds)
                {
                    var col = repo.Database.GetCollection("DataObjects");
                    var item = col.FindById(id);
                    if (item == null)
                    {
                        notFoundId.Add(id);
                    }
                }
            }
            
            Console.WriteLine($"Missing item Count { notFoundId.Count}");
            foreach (var notfound in notFoundId)
            {
                Console.WriteLine($"Item with id { notfound}  not found");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    public class MongoDbDataWrapper<T>
    {
#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// Gets and sets the id of the data object (This is a primary key in MongoDB)
        /// </summary>
        public string Id { get; set; }
        

#pragma warning restore IDE1006 // Naming Styles
    }
}
