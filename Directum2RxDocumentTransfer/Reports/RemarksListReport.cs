using Directum2RxDocumentTransfer.DTO;
using Directum2RxDocumentTransfer.Utils;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Directum2RxDocumentTransfer.Reports
{
    public class RemarksListReport
    {
        public void GetReportDataAndSendToDirectumRX(int? taskId, int? documentId, string? subject) 
        {
            Logger.Debug($"RemarksList. GetReportDataAndSendToDirectumRX. Processing document: {taskId}");

            var reportData = new RemarksEntities.RemarksListData();
            reportData.MainDocument = documentId ?? -1;

            // Найдем нейм документа
            if (!string.IsNullOrEmpty(subject))
            {
                var splitName = subject.Split("Согласование");
                var header = splitName.Length >= 2 ? splitName[1].Trim() : subject;
                reportData.DocumentName = header;

                Logger.Debug($"RemarksList. GetReportDataAndSendToDirectumRX. Subject (header): {header}");
            }
            else
            {
                reportData.DocumentName = "";
                Logger.Debug($"RemarksList. GetReportDataAndSendToDirectumRX. Subject was not defined.");
            }

            var remarksListLinesEntities = new List<RemarksEntities.RemarksListLine>();
            using (var connection = SQL.SqlHandler.CreateNewDirectumConnection())
            {
                connection.Open();

                var commandText = string.Format(SQL.SqlCommands.RemarksListDataCommand, taskId);
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 86400;
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            remarksListLinesEntities.Add(new RemarksEntities.RemarksListLine()
                            {
                                Performer = reader.GetValue(0) as string,
                                Result = reader.GetValue(1) as string,
                                MarkDocId = !reader.IsDBNull(2) ? reader.GetInt32(2) : -1,
                                MarkDocName = reader.GetValue(3) as string,
                                Remark = reader.GetValue(4) != null ? Misc.GetStringInEncoding(reader.GetValue(4) as byte[], "Windows-1251") : null
                            });
                }
            }

            if (!remarksListLinesEntities.Any())
            {
                Logger.Debug($"RemarksList. GetReportDataAndSendToDirectumRX. Processing document {taskId} skipped. No lines found (perhaps no remarks).");
                return;
            }

            reportData.Lines = remarksListLinesEntities;
            // var sendResult = Networking.SendRequest(new RemarksEntities.RemarksWebRequest() { data = reportData }, Networking.Endpoint.Remarks).Result;
            var sql = "UPDATE sungero_content_edoc SET remarkslistjso_russnef_centrvd = @json WHERE xrecid = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@json", JsonConvert.SerializeObject(reportData) },
                { "@id", reportData.MainDocument }
            };
            var sendResult = reportData.MainDocument != -1 ? SQL.SqlHandler.RunDirectumRxCommand(sql, parameters) : "No main document sent. Skip.";
            Logger.Debug($"RemarksList. GetReportDataAndSendToDirectumRX. Result sent. Status: {sendResult}");
        }
    }
}
