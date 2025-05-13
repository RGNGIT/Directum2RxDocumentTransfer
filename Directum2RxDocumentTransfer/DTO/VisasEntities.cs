using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.DTO
{
    public static class VisasEntities
    {
        public class VisasItemCommonEntity
        {
            public string? Executor { get; set; }
            public string? Department { get; set; }
            public string? Substitute { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? WorkingTime { get; set; }
            public string? Result { get; set; }
        }

        public class VisasListData
        {
            public long MainDocument { get; set; }
            public string DocumentName { get; set; }
            public string DocumentPerformer { get; set; }
            public List<VisasListApprover> Approvers { get; set; }
            public List<VisasListSignatory> Signatories { get; set; }
        }

        public class VisasListApprover
        {
            public string Visioner { get; set; }
            public string Department { get; set; }
            public string Substitute { get; set; }
            public string AssignmentStartDate { get; set; }
            public string AssignmentCompleteDate { get; set; }
            public string TimeInWork { get; set; }
            public string Result { get; set; }
        }

        public class VisasListSignatory
        {
            public string Signatory { get; set; }
            public string Department { get; set; }
            public string Substitute { get; set; }
            public string AssignmentStartDate { get; set; }
            public string AssignmentCompleteDate { get; set; }
            public string TimeInWork { get; set; }
            public string Result { get; set; }
        }
    }
}
