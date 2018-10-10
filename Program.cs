using System;

namespace CultureLiteDbProblem
{
    using System.Globalization;
    using System.Threading;

    using LiteDB;

    class Program
    {
        static void Main(string[] args)
        {
           var cultureInfo = new CultureInfo("en-gb"); //breaks
             //var cultureInfo =new CultureInfo("fi"); //works

            Thread.CurrentThread.CurrentCulture = cultureInfo;

            Console.WriteLine($"Running with {cultureInfo.Name} culture");

            using (var repo = new LiteRepository($"Filename = Bug66245_min.db;Journal = false;async=true;Cache Size=0;Flush= true;Timeout=0:10:00;mode=Exclusive;"))
            {
                repo.Database.Log.Level = Logger.FULL;
                repo.Database.Log.Logging += (s) => Console.WriteLine(s);
                var collection =
                    repo.Database.GetCollection<MongoDbDataWrapper<string>>("DataObjects_1");

                //repo.Database.Shrink(); //This fixes the problem?

                var o = collection.FindById("qyeyeW.1oMJK5");

                Console.WriteLine();
                Console.WriteLine(o == null ? "Nothing found" : "Found");
                
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            }
        }

        public class MongoDbDataWrapper<T>
        {
#pragma warning disable IDE1006 // Naming Styles

            /// <summary>
            /// Gets and sets the data object
            /// </summary>
            public T d { get; set; }


            /// <summary>
            /// Gets and sets the id of the data object (This is a primary key in MongoDB)
            /// </summary>
            public string Id { get; set; }


            /// <summary>
            /// Gets and sets the if of the meta data node that is associated with the data object
            /// </summary>
            public string m { get; set; }

#pragma warning restore IDE1006 // Naming Styles
        }
    }
