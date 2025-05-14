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
                visasHandler.GetReportDataAndSendToDirectumRX(item.TaskID, item.DocumentId, 288);
        }

        static void Test()
        {
            var objectToSend = new VisasEntities.VisasListData()
            {
                MainDocument = 123,
                DocumentName = "Sas",
                DocumentPerformer = "Sos",
                Approvers = new List<VisasEntities.VisasListApprover>() 
                { 
                    new VisasEntities.VisasListApprover() 
                    {
                        Visioner = "Согласовович Согласуй Согласуев",
                        Department = "Подразделение не ваших собачих дел",
                        Substitute = "Пупов Пупа Пупович",
                        AssignmentStartDate = "04.05.2001 00:00",
                        AssignmentCompleteDate = "04.05.2001 00:00",
                        TimeInWork = "00:00",
                        Result = "Согласовано"
                    },
                    new VisasEntities.VisasListApprover()
                    {
                        Visioner = "Согласовович Согласуй Согласуев",
                        Department = "Подразделение не ваших собачих дел",
                        Substitute = "Пупов Пупа Пупович",
                        AssignmentStartDate = "04.05.2001 00:00",
                        AssignmentCompleteDate = "04.05.2001 00:00",
                        TimeInWork = "00:00",
                        Result = "Согласовано"
                    }
                },
                Signatories = new List<VisasEntities.VisasListSignatory>() 
                {
                    new VisasEntities.VisasListSignatory()
                    {
                        Signatory = "Согласовович Согласуй Согласуев",
                        Department = "Подразделение не ваших собачих дел",
                        Substitute = "Пупов Пупа Пупович",
                        AssignmentStartDate = "04.05.2001 00:00",
                        AssignmentCompleteDate = "04.05.2001 00:00",
                        TimeInWork = "00:00",
                        Result = "Согласовано"
                    },
                    new VisasEntities.VisasListSignatory()
                    {
                        Signatory = "Согласовович Согласуй Согласуев",
                        Department = "Подразделение не ваших собачих дел",
                        Substitute = "Пупов Пупа Пупович",
                        AssignmentStartDate = "04.05.2001 00:00",
                        AssignmentCompleteDate = "04.05.2001 00:00",
                        TimeInWork = "00:00",
                        Result = "Согласовано"
                    }
                }
            };

            var result = Networking.SendRequest(new VisasEntities.VisasWebRequest() { data = objectToSend }, Networking.Endpoint.Visas).Result;
            Console.WriteLine(result);
        }

        static void Main()
        {
            Initialize();
            // Test();
            FormReports();

            Console.ReadKey();
        }
    }
}