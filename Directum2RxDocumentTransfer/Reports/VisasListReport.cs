using Directum2RxDocumentTransfer.DTO;
using Directum2RxDocumentTransfer.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Directum2RxDocumentTransfer.Reports
{
    public class VisasListReport
    {
        public void GetReportDataAndSendToDirectumRX(int? taskId, int? documentId, int? rabCode)
        {
            var reportData = new VisasEntities.VisasListData();
            reportData.MainDocument = 1;
            // Найдем автора этого документа (инициатор задачи)
            using (var connection = SQL.SqlHandler.CreateNewConnection())
            {
                connection.Open();
                var commandText = string.Format(SQL.SqlCommands.TaskInitiatorScalarCommand, taskId);
                Logger.Debug($"GetReportDataAndSendToDirectumRX. Query: {commandText}");

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 86400;
                    var result = command.ExecuteScalar();
                    // Ожидается стринг - фулнейм автора
                    reportData.DocumentPerformer = result?.ToString();
                }
            }
            // Найдем нейм документа
            reportData.DocumentName = "Пока что тестовый";
            // Найдем все данные таблицы
            var visasCommonEntities = new List<VisasEntities.VisasItemCommonEntity>();
            using (var connection = SQL.SqlHandler.CreateNewConnection())
            {
                connection.Open();
                var commandText = string.Format(SQL.SqlCommands.VisasListDataCommand, taskId, rabCode);
                Logger.Debug($"GetReportDataAndSendToDirectumRX. Query: {commandText}");

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
                .Where(v => v.Result == "Согласовано")
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
                .Where(v => v.Result == "Подписано")
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

            reportData.Approvers = approvers;
            reportData.Signatories = signatures;

            var sendResult = Networking.SendRequest(new VisasEntities.VisasWebRequest() { data = reportData }, Networking.Endpoint.Visas).Result;
            Logger.Debug($"GetReportDataAndSendToDirectumRX. ResultSent. Status: {sendResult}");
        }
    }
}
