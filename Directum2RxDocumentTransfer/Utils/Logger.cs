using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.Utils
{
    public static class Logger
    {
        public static void Debug(string message) 
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: {message}");
        }
    }
}
