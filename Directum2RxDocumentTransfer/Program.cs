using Directum2RxDocumentTransfer.DTO;
using Directum2RxDocumentTransfer.Reports;
using Directum2RxDocumentTransfer.Utils;
using System.Xml;

namespace Directum2RxDocumentTransfer
{
    internal static class Program
    {
        static void Initialize() 
        {
            Console.WriteLine("�������������...");

            var xmlDoc = new XmlDocument();
            var xmlString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml"));
            xmlDoc.LoadXml(xmlString);

            Console.WriteLine("��������� ������ ����������� � �� DIRECTUM...");
            var connectionStringNode = xmlDoc.SelectSingleNode("/Config/ConnectionString");
            SQL.SqlHandler.d5ConnectionString = connectionStringNode?.InnerText;

            Console.WriteLine("��������� ������ ����������� � �� DirectumRX...");
            var connectionStringDirectumRXNode = xmlDoc.SelectSingleNode("/Config/ConnectionStringRX");
            SQL.SqlHandler.directumrxConnectionString = connectionStringDirectumRXNode?.InnerText;

            Console.WriteLine("��������� �������� URL...");
            var baseUrlNode = xmlDoc.SelectSingleNode("/Config/BaseUrl");
            Networking.baseUrl = baseUrlNode?.InnerText;

            Console.WriteLine("��������� ����������...");
            var visasEndpointNode = xmlDoc.SelectSingleNode("/Config/VisasEndpoint");
            Networking.endpointMap.Add(Networking.Endpoint.Visas, visasEndpointNode?.InnerText);
            var remarksEndpointNode = xmlDoc.SelectSingleNode("/Config/RemarksEndpoint");
            Networking.endpointMap.Add(Networking.Endpoint.Remarks, remarksEndpointNode?.InnerText);

            Console.WriteLine("��������� ������...");
            var credentialsNode = xmlDoc.SelectSingleNode("/Config/Credentials");
            Networking.credentials = credentialsNode?.InnerText;

            Console.WriteLine("�������� ��������� �������...");
            SQL.SqlHandler.CreateSystemTableIfNotExists();

            Console.WriteLine("������������� ���������.");
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
            // ������ ������
            var formDataUtil = new FormDataList(1, 1);
            var data = formDataUtil.GetDataList();
            foreach (var item in data)
            {
                try
                {
                    FormVisasListReport(item);
                    FormRemarksListReport(item);
                }
                catch (Exception ex) 
                {
                    Logger.Debug(ex.ToString());
                }
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