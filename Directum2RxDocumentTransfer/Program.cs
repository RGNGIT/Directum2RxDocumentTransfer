using Directum2RxDocumentTransfer.DTO;
using Directum2RxDocumentTransfer.Reports;
using Directum2RxDocumentTransfer.Utils;
using System.Data.SqlTypes;
using System.Xml;

namespace Directum2RxDocumentTransfer
{
    internal static class Program
    {
        static void Initialize() 
        {
            Console.WriteLine("Инициализация...");

            var xmlDoc = new XmlDocument();
            var xmlString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml"));
            xmlDoc.LoadXml(xmlString);

            Console.WriteLine("Установка строки подключения к БД...");
            var connectionStringNode = xmlDoc.SelectSingleNode("/Config/ConnectionString");
            SQL.SqlHandler.connectionString = connectionStringNode?.InnerText;

            Console.WriteLine("Установка базового URL...");
            var baseUrlNode = xmlDoc.SelectSingleNode("/Config/BaseUrl");
            Networking.baseUrl = baseUrlNode?.InnerText;

            Console.WriteLine("Установка эндпоинтов...");
            var visasEndpointNode = xmlDoc.SelectSingleNode("/Config/VisasEndpoint");
            Networking.endpointMap.Add(Networking.Endpoint.Visas, visasEndpointNode?.InnerText);
            var remarksEndpointNode = xmlDoc.SelectSingleNode("/Config/RemarksEndpoint");
            Networking.endpointMap.Add(Networking.Endpoint.Remarks, remarksEndpointNode?.InnerText);

            Console.WriteLine("Установка кредов...");
            var credentialsNode = xmlDoc.SelectSingleNode("/Config/Credentials");
            Networking.credentials = credentialsNode?.InnerText;

            Console.WriteLine("Создание системной таблицы...");
            SQL.SqlHandler.CreateSystemTableIfNotExists();

            Console.WriteLine("Инициализация завершена.");
        }

        static void FormVisasListReport(DataListEntity item) 
        {
            //if (SQL.SqlHandler.ExistsInSystemTable((int)item.TaskID!, "Visas"))
            //    return;

            var visasHandler = new VisasListReport();
            visasHandler.GetReportDataAndSendToDirectumRX(item.TaskID, item.DocumentId, 288, item.Subject);

            //SQL.SqlHandler.InsertIntoSystemTable((int)item.TaskID!, "Visas");
        }

        static void FormRemarksListReport(DataListEntity item) 
        {
            //if (SQL.SqlHandler.ExistsInSystemTable((int)item.TaskID!, "Remarks"))
            //    return;

            var remarksHandler = new RemarksListReport();
            remarksHandler.GetReportDataAndSendToDirectumRX(item.TaskID, item.DocumentId, item.Subject);

            //SQL.SqlHandler.InsertIntoSystemTable((int)item.TaskID!, "Remarks");
        }

        static void FormReports() 
        {
            // Данные всякие
            var formDataUtil = new FormDataList(1, 1);
            var data = formDataUtil.GetDataList();
            foreach (var item in data)
            {
                // FormVisasListReport(item);
                FormRemarksListReport(item);
            }
        }

        static void Main()
        {
            Initialize();

            Misc.Timer.Start();
            FormReports();
            var timeElapsed = Misc.Timer.StopAndGetValue();

            Logger.Debug($"Process ended. Time elapsed: {timeElapsed}");
            Console.ReadKey();
        }
    }
}