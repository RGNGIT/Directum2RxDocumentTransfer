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
            public static string? connectionString { get; set; } = string.Empty;

            public static SqlConnection CreateNewConnection() => new SqlConnection(connectionString);
        }

        public static class SqlCommands
        {
            public static string FormListCommand = @"
                SELECT DISTINCT d.XRecID, a.TaskID, l.id, l.mes
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
                ORDER BY d.XRecID DESC
                OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;";
            public static string TaskInitiatorScalarCommand = @"SELECT TOP(1) a.NameAn FROM MBAnalit a, SBTask b WHERE a.XRecID = b.Author AND b.XRecID = {0};";

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
ORDER BY j.EndDate DESC";
        }
    }
}
