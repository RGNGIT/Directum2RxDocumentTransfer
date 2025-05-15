using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.Utils
{
    public static class Misc
    {
        public static string GetStringInEncoding(string rawString, string encoding) 
        {
            byte[] encodedBytes = Encoding.GetEncoding(encoding).GetBytes(rawString);
            return Encoding.GetEncoding(encoding).GetString(encodedBytes);
        }
    }
}
