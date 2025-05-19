using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.DTO
{
    public class DataListEntity
    {
        public int DocumentId { get; set; }
        public int? TaskID { get; set; }
        public string? Subject { get; set; }
    }
}
