using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.DTO
{
    public class DataListEntity
    {
        public int XRecID { get; set; }
        public int? TaskID { get; set; }
        public int? DocumentId { get; set; }
        public string? Mes { get; set; }
        public string? Subject { get; set; }
    }
}
