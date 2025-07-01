using Directum2RxDocumentTransfer.DTO;
using Directum2RxDocumentTransfer.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Directum2RxDocumentTransfer.Reports
{
    public class VisasListReport
    {
        public void GetReportDataAndSendToDirectumRX(int? taskId, int? documentId, int? rabCode, string? subject)
        {
            Logger.Debug($"VisasList. GetReportDataAndSendToDirectumRX. Processing document: {taskId}");

            var reportData = new VisasEntities.VisasListData();
            reportData.MainDocument = documentId ?? -1;
            // Найдем автора этого документа (инициатор задачи)
            using (var connection = SQL.SqlHandler.CreateNewDirectumConnection())
            {
                connection.Open();

                var commandText = string.Format(SQL.SqlCommands.TaskInitiatorScalarCommand, taskId);
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 86400;
                    var result = command.ExecuteScalar();
                    // Ожидается стринг - фулнейм автора
                    reportData.DocumentPerformer = result?.ToString();
                }
            }
            // Найдем нейм документа
            if (!string.IsNullOrEmpty(subject))
            {
                var splitName = subject.Split("Согласование");
                var header = splitName.Length >= 2 ? splitName[1].Trim() : subject;
                reportData.DocumentName = header;

                Logger.Debug($"VisasList. GetReportDataAndSendToDirectumRX. Subject (header): {header}");
            }
            else
            {
                reportData.DocumentName = "";
                Logger.Debug($"VisasList. GetReportDataAndSendToDirectumRX. Subject was not defined.");
            }

            // Найдем все данные таблицы
            var visasCommonEntities = new List<VisasEntities.VisasItemCommonEntity>();
            using (var connection = SQL.SqlHandler.CreateNewDirectumConnection())
            {
                connection.Open();
                var commandText = string.Format(SQL.SqlCommands.VisasListDataCommand, taskId, rabCode);

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 86400;
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            visasCommonEntities.Add(new VisasEntities.VisasItemCommonEntity()
                            {
                                Executor = reader.GetValue(0) as string,
                                Department = reader.GetValue(1) as string,
                                Substitute = reader.GetValue(2) as string,
                                StartDate = reader.GetDateTime(3).ToString("dd.MM.yyyy HH:mm.ss"),
                                EndDate = reader.GetDateTime(4).ToString("dd.MM.yyyy HH:mm.ss"),
                                WorkingTime = reader.GetValue(5) as string,
                                Result = reader.GetValue(6) as string
                            });
                }
            }
            // Выделяем аппруверов и подписантов
            var approvers = visasCommonEntities
                .Where(v => v.Result != "Подписано" && v.Result != "Отказать" && v.Result != "Утверждено")
                .Select(v => new VisasEntities.VisasListApprover()
                {
                    Visioner = v.Executor,
                    Department = v.Department,
                    Substitute = v.Substitute,
                    AssignmentStartDate = v.StartDate,
                    AssignmentCompleteDate = v.EndDate,
                    TimeInWork = v.WorkingTime,
                    Result = v.Result
                })
                .ToList();
            var signatures = visasCommonEntities
                .Where(v => v.Result == "Подписано" || v.Result == "Отказать" || v.Result == "Утверждено")
                .Select(v => new VisasEntities.VisasListSignatory()
                {
                    Signatory = v.Executor,
                    Department = v.Department,
                    Substitute = v.Substitute,
                    AssignmentStartDate = v.StartDate,
                    AssignmentCompleteDate = v.EndDate,
                    TimeInWork = v.WorkingTime,
                    Result = v.Result
                })
                .ToList();

            if (!approvers.Any() && !signatures.Any())
            {
                Logger.Debug($"VisasList. GetReportDataAndSendToDirectumRX. Processing document {taskId} skipped. No approvers and signatures found.");
                return;
            }

            reportData.Approvers = approvers;
            reportData.Signatories = signatures;

            // var sendResult = Networking.SendRequest(new VisasEntities.VisasWebRequest() { data = reportData }, Networking.Endpoint.Visas).Result;
            var sendResult = reportData.MainDocument != -1 ? SQL.SqlHandler.RunDirectumRxCommand(string.Format("UPDATE sungero_content_edoc SET visaslistjsonc_russnef_centrvd = '{0}' WHERE xrecid = '{1}';", JsonConvert.SerializeObject(reportData), reportData.MainDocument)) : "No main document sent. Skip.";
            Logger.Debug($"VisasList. GetReportDataAndSendToDirectumRX. Result sent. Status: {sendResult}");
        }
    }
}
