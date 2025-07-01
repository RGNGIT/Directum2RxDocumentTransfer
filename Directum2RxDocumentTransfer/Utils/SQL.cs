using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Directum2RxDocumentTransfer.Utils
{
    public static class SQL
    {
        public static class SqlHandler
        {
            public static string? d5ConnectionString { get; set; } = string.Empty;
            public static string? directumrxConnectionString { get; set; } = string.Empty;

            public static SqlConnection CreateNewDirectumConnection() => new SqlConnection(d5ConnectionString);
            public static SqlConnection CreateNewDirectumRxConnection() => new SqlConnection(directumrxConnectionString);

            public static void CreateSystemTableIfNotExists()
            {
                using (var connection = CreateNewDirectumConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SqlCommands.CreateSystemTable, connection))
                        command.ExecuteNonQuery();
                }
            }

            public static void InsertIntoSystemTable(int taskId, string reportType)
            {
                using (var connection = CreateNewDirectumConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(string.Format(SqlCommands.InsertIntoSystemTable, taskId, reportType), connection))
                        command.ExecuteNonQuery();
                }
            }

            public static string RunDirectumRxCommand(string sqlCommand) 
            {
                using (var connection = CreateNewDirectumRxConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlCommand, connection))
                        command.ExecuteNonQuery();
                }

                return "OK";
            }

            public static bool ExistsInSystemTable(int taskId, string reportType) 
            {
                using (var connection = CreateNewDirectumConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(string.Format(SqlCommands.CountFromSystemTable, taskId, reportType), connection))
                    {
                        var result = command.ExecuteScalar();
                        return (int)result > 0;
                    }
                }
            }
        }

        public static class SqlCommands
        {
            public static string CountFromSystemTable = @"SELECT COUNT(*) FROM CentrVD_DocTransfer_ProcessedTasks WHERE TaskId = '{0}' AND ReportType = '{1}';";

            public static string InsertIntoSystemTable = @"INSERT INTO CentrVD_DocTransfer_ProcessedTasks (TaskId, ReportType) VALUES ('{0}', '{1}');";

            public static string CreateSystemTable = @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CentrVD_DocTransfer_ProcessedTasks')
BEGIN
    CREATE TABLE CentrVD_DocTransfer_ProcessedTasks
    (
        [TaskId] int,
        [ReportType] nvarchar(max)
    )
END;";

            public static string FormListCommand = @"
                SELECT DISTINCT d.XRecID, a.TaskID, l.id, l.mes, d.Name 
                FROM SBEDoc d
                LEFT JOIN SBTaskAttach a ON d.XRecID = a.AttachID
                LEFT JOIN SBTask t ON t.XRecID = a.TaskID
                LEFT JOIN lists l ON l.docid = d.XRecID
                WHERE d.Kind IN (1354042, 101731, 101733, 1046615, 102804, 101730, 1016457,
                                101727, 378269, 116955, 102811, 102786, 832434, 215802, 101739)
                AND t.StandardRoute IN (3793114, 1946646, 3726428, 385179, 814183, 385180, 2854901,
                                      3753319, 113534, 2783244, 352255, 2361561, 104380, 2170042, 3000837,
                                      2652158, 2652157, 113526, 2388218, 113533, 2997213, 2663968, 622583,
                                      113532, 1969563, 206596, 2399164)
                AND t.State = 'D'
                AND l.id IS NULL
                ORDER BY d.XRecID DESC;";
            public static string TaskInitiatorScalarCommand = @"SELECT TOP(1) a.NameAn FROM MBAnalit a, SBTask b WHERE a.XRecID = b.Author AND b.XRecID = {0};";

            public static string RemarksListDataCommand = @"select u.Dop3, 
                         j.ResultTitle, 
                         r.MarksDocID,
                         d.Name,
                         t.Text,
                         j.StartDate 
                  from RSNReportInfo r
                       left join SBEDoc d on (d.XRecID = r.MarksDocID), 
                       SBTaskJob j, 
                       MBAnalit u,
                       SBTaskText t
                  where r.TaskID in (select XRecID from SBTask where XRecID = {0} or MainTaskID = {0}) and
                        j.XRecID = r.JobID and
                        u.Analit = j.Executor and
                        t.JobID = r.JobID and
                        j.ResultTitle in ('Согласовано с замечаниями', 'Замечания сняты', 'Согласовано с замечаниями в документе', 'Не согласовано', 'За', 'Против', 'Воздержался', 'На доработку')
                  order by j.StartDate asc, t.TextLastUpd asc;";

            public static string VisasListDataCommand = @"SELECT u.Dop3 AS ExecutorName,
       (SELECT unit.Prim
        FROM MBAnalit unit
        WHERE unit.Analit = (SELECT TOP 1 w.Podr
                             FROM MBAnalit w
                             WHERE w.Polzovatel = u.Analit AND w.Vid = {1})) AS Unit,
       CASE
           WHEN EXISTS(SELECT 1 FROM SBTaskProtocol tp
                        WHERE tp.ActionType = 'X' AND
                              tp.UserID = j.Executor AND
                              tp.JobID = r.JobID) THEN ''
           ELSE (SELECT TOP 1 su.Dop3
                 FROM SBTaskProtocol tp,
                      MBAnalit su
                 WHERE tp.ActionType = 'X' AND
                       tp.JobID = r.JobID AND
                       su.Analit = tp.UserID)
       END AS SubsName,
       j.StartDate AS StartD,
       j.EndDate AS EndD,
	   RIGHT('0' + CAST(DATEDIFF(SECOND, j.StartDate, j.EndDate) / 3600 AS VARCHAR), 2) + ':' +
       RIGHT('0' + CAST((DATEDIFF(SECOND, j.StartDate, j.EndDate) % 3600) / 60 AS VARCHAR), 2) AS WorkingTime,
       CASE
           WHEN j.ResultTitle IS NULL AND j.EndDate IS NOT NULL THEN 'Выполнено'
           ELSE j.ResultTitle
       END AS Res,
       j.Executor
FROM RSNReportInfo r,
     SBTaskJob j,
     MBAnalit u
WHERE r.TaskID IN (SELECT XRecID
                   FROM SBTask
                   WHERE XRecID = {0} OR MainTaskID = {0}) AND
      j.XRecID = r.JobID AND
      u.Analit = j.Executor AND
      (j.ResultTitle IS NULL OR j.ResultTitle <> 'На доработку')
ORDER BY j.EndDate DESC;";
        }
    }
}
