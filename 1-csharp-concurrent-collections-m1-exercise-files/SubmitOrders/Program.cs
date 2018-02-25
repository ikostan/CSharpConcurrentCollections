using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;

namespace Pluralsight.ConcurrentCollections.SubmitOrders
{
	class Program
	{
		static void Main(string[] args)
		{
            // Each of these methods shows a different state in the program
            // as it was edited through module 1 of the course.

            // Uncomment any method and comment out the others to see
            // how the program worked at that point.

            // initially, with one thread:

            RunProgramOneThread();

            // multi-threaded but NOT thread-safe:
            // This method will give unpredictable results and may crash
            // Note that the stability of this function depends to some extent on 
            // your hardware. I put a Thread.Sleep(1) in the code used in the module
            // because that seemed to time the threads to maximize contention and so maximuze
            // the likelihood of showing errors on my computer.
            // You may need to adjust this time to get similar results on your hardware.

            RunProgramMultithreaded();

            // multi-threaded and thread-safe
            // because it uses a concurrent queue

            RunProgramConcurrent();

            // multi-threaded and thread-safe
            // because it uses locks to synchronize threads

            RunProgramWithLock();

		}

        //Single thread
		static void RunProgramOneThread()
		{
			var orders = new Queue<string>();
			PlaceOrders(orders, "Mark");
			PlaceOrders(orders, "Ramdevi");

			foreach (string order in orders)
				Console.WriteLine("ORDER: " + order);

            Console.WriteLine("Please press any...");
            Console.ReadKey();
		}

        //Multi-threaded BUT still using a regular Queue
		static void RunProgramMultithreaded()
		{
			var orders = new Queue<string>();
			Task task1 = Task.Run(() => PlaceOrders(orders, "Mark"));
			Task task2 = Task.Run(() => PlaceOrders(orders, "Ramdevi"));
			Task.WaitAll(task1, task2);

			foreach (string order in orders)
				Console.WriteLine("ORDER: " + order);

            Console.WriteLine("Please press any...");
            Console.ReadKey();
		}

        //Multi-threaded + using a concurrent Queue
        static void RunProgramConcurrent()
		{
			var orders = new ConcurrentQueue<string>();
			Task task1 = Task.Run(() => PlaceOrders2(orders, "Mark"));
			Task task2 = Task.Run(() => PlaceOrders2(orders, "Ramdevi"));
			Task.WaitAll(task1, task2);

			foreach (string order in orders)
				Console.WriteLine("ORDER: " + order);

            Console.WriteLine("Please press any...");
            Console.ReadKey();
        }

        //
		static void RunProgramWithLock()
		{
			var orders = new Queue<string>();
			Task task1 = Task.Run(() => PlaceOrders3(orders, "Mark"));
			Task task2 = Task.Run(() => PlaceOrders3(orders, "Ramdevi"));
			Task.WaitAll(task1, task2);

			foreach (string order in orders)
				Console.WriteLine("ORDER: " + order);

            Console.WriteLine("Please press any...");
            Console.ReadKey();
        }

        // Single threaded method - often produces bugs if used in multi-threaded flow
        static void PlaceOrders(Queue<string> orders, string customerName)
		{
            // Add 5 orders
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(1);
				string orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1); // Order description
				orders.Enqueue(orderName); //Add a new order in the queue
			}
		}

        //This method is using concurrent collection, much better for multi-threading flow
		static void PlaceOrders2(ConcurrentQueue<string> orders, string customerName)
		{
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(1);
				string orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1);
				orders.Enqueue(orderName);
			}
		}

        //Object that used for LOCKING purposes
		static object _lockObj = new object();

        //Using LOCK in order to get thread-safety
		static void PlaceOrders3(Queue<string> orders, string customerName)
		{
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(1);
				string orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1);
				lock (_lockObj)
				{
					orders.Enqueue(orderName);
				}
			}

            Console.WriteLine("Please press any...");
            Console.ReadKey();
        }
	}

    //End of class
}