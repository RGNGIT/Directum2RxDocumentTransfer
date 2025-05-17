using Directum2RxDocumentTransfer.DTO;
using Directum2RxDocumentTransfer.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directum2RxDocumentTransfer.Reports
{
    public class RemarksListReport
    {
        public void GetReportDataAndSendToDirectumRX(int? taskId, int? documentId) 
        {
            var reportData = new RemarksEntities.RemarksListData();
            reportData.MainDocument = documentId ?? -1;
            var remarksListLinesEntities = new List<RemarksEntities.RemarksListLine>();
            using (var connection = SQL.SqlHandler.CreateNewConnection())
            {
                connection.Open();
                var commandText = string.Format(SQL.SqlCommands.RemarksListDataCommand, taskId);
                Logger.Debug($"GetReportDataAndSendToDirectumRX. Query: {commandText}");

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 86400;
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            remarksListLinesEntities.Add(new RemarksEntities.RemarksListLine()
                            {
                                Performer = reader.GetValue(0) as string,
                                Result = reader.GetValue(1) as string,
                                MarkDocId = reader.GetInt32(2),
                                MarkDocName = reader.GetValue(3) as string,
                                Remark = reader.GetValue(4) != null ? Misc.GetStringInEncoding(reader.GetValue(4).ToString(), "Windows-1251") : null
                            });
                }
            }

            if (!remarksListLinesEntities.Any())
                return;

            // Найдем нейм документа
            reportData.DocumentName = $"Пока что тестовый Ремарк {taskId}";

            reportData.Lines = remarksListLinesEntities;
            var sendResult = Networking.SendRequest(new RemarksEntities.RemarksWebRequest() { data = reportData }, Networking.Endpoint.Remarks).Result;
            Logger.Debug($"GetReportDataAndSendToDirectumRX. ResultSent. Status: {sendResult}");
        }
    }
}
