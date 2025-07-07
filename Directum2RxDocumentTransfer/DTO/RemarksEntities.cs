namespace Directum2RxDocumentTransfer.DTO
{
    public static class RemarksEntities
    {
        public class RemarksListData
        {
            public long? MainDocument { get; set; }
            public string DocumentName { get; set; }
            public List<RemarksListLine> Lines { get; set; }
        }

        public class RemarksListLine 
        {
            public string? Performer {  get; set; }
            public int? MarkDocId { get; set; }
            public string? MarkDocName { get; set; }
            public string? Result { get; set; }
            public string? Remark { get; set; }
        }

        public class RemarksWebRequest
        {
            public RemarksListData data { get; set; }
        }
    }
}
