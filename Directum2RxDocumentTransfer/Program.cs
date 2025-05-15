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
            Console.WriteLine("�������������...");

            var xmlDoc = new XmlDocument();
            var xmlString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml"));
            xmlDoc.LoadXml(xmlString);

            Console.WriteLine("��������� ������ ����������� � ��...");
            var connectionStringNode = xmlDoc.SelectSingleNode("/Config/ConnectionString");
            SQL.SqlHandler.connectionString = connectionStringNode?.InnerText;

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
            visasHandler.GetReportDataAndSendToDirectumRX(item.TaskID, item.DocumentId, 288);

            //SQL.SqlHandler.InsertIntoSystemTable((int)item.TaskID!, "Visas");
        }

        static void FormRemarksListReport(DataListEntity item) 
        {
            //if (SQL.SqlHandler.ExistsInSystemTable((int)item.TaskID!, "Remarks"))
            //    return;

            var remarksHandler = new RemarksListReport();
            remarksHandler.GetReportDataAndSendToDirectumRX(item.TaskID, item.DocumentId);

            //SQL.SqlHandler.InsertIntoSystemTable((int)item.TaskID!, "Remarks");
        }

        static void FormReports() 
        {
            // ������ ������
            var formDataUtil = new FormDataList(1, 1);
            var data = formDataUtil.GetDataList();
            foreach (var item in data)
            {
                FormVisasListReport(item);
                FormRemarksListReport(item);
            }
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
                        Visioner = "������������ �������� ���������",
                        Department = "������������� �� ����� ������� ���",
                        Substitute = "����� ���� �������",
                        AssignmentStartDate = "04.05.2001 00:00",
                        AssignmentCompleteDate = "04.05.2001 00:00",
                        TimeInWork = "00:00",
                        Result = "�����������"
                    },
                    new VisasEntities.VisasListApprover()
                    {
                        Visioner = "������������ �������� ���������",
                        Department = "������������� �� ����� ������� ���",
                        Substitute = "����� ���� �������",
                        AssignmentStartDate = "04.05.2001 00:00",
                        AssignmentCompleteDate = "04.05.2001 00:00",
                        TimeInWork = "00:00",
                        Result = "�����������"
                    }
                },
                Signatories = new List<VisasEntities.VisasListSignatory>() 
                {
                    new VisasEntities.VisasListSignatory()
                    {
                        Signatory = "������������ �������� ���������",
                        Department = "������������� �� ����� ������� ���",
                        Substitute = "����� ���� �������",
                        AssignmentStartDate = "04.05.2001 00:00",
                        AssignmentCompleteDate = "04.05.2001 00:00",
                        TimeInWork = "00:00",
                        Result = "�����������"
                    },
                    new VisasEntities.VisasListSignatory()
                    {
                        Signatory = "������������ �������� ���������",
                        Department = "������������� �� ����� ������� ���",
                        Substitute = "����� ���� �������",
                        AssignmentStartDate = "04.05.2001 00:00",
                        AssignmentCompleteDate = "04.05.2001 00:00",
                        TimeInWork = "00:00",
                        Result = "�����������"
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