using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.Utils
{
    public static class Misc
    {
        public static class Timer 
        {
            private static Stopwatch? timer;

            public static void Start() 
            {
                timer = new Stopwatch();
                timer.Start();
            }

            public static string StopAndGetValue() 
            {
                if (timer == null)
                    return "No time elapsed";

                timer!.Stop();
                return timer.Elapsed.TotalSeconds.ToString();
            }
        }

        public static string GetStringInEncoding(byte[] encodedBytes, string encoding) 
        {
            return Encoding.GetEncoding(encoding).GetString(encodedBytes);
        }
    }
}
