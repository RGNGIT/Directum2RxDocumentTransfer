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
            Networking.visasEndpoint = visasEndpointNode?.InnerText;

            Console.WriteLine("Установка кредов...");
            var credentialsNode = xmlDoc.SelectSingleNode("/Config/Credentials");
            Networking.credentials = credentialsNode?.InnerText;

            Console.WriteLine("Инициализация завершена.");
        }

        static void FormReports() 
        {
            // Данные всякие
            var formDataUtil = new FormDataList(1, 1);
            var data = formDataUtil.GetDataList();
            // Лист визирования
            var visasHandler = new VisasListReport();
            foreach (var item in data)
                visasHandler.GetReportDataAndSendToDirectumRX((int)item.TaskID!, (int)item.DocumentId!, 288);
        }

        static void Main()
        {
            Initialize();
            FormReports();
        }
    }
}