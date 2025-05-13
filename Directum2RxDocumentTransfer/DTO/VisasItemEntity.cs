using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.DTO
{
    public class VisasItemEntity
    {
        public string? Executor { get; set; }
        public string? Department { get; set; }
        public string? Substitute { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? WorkingTime { get; set; }
        public string? Result { get; set; }
    }
}
