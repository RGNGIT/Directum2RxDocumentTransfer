using Directum2RxDocumentTransfer.Reports;
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
            // ��������� ��������������
            var connectionStringNode = xmlDoc.SelectSingleNode("/Config/ConnectionString");
            Utils.SQL.SqlHandler.connectionString = connectionStringNode?.InnerText;

            Console.WriteLine("������������� ���������.");
        }

        static void FormReports() 
        {
            // ������ ������
            var formDataUtil = new Utils.FormDataList(1, 1);
            var data = formDataUtil.GetDataList();
            // ���� �����������
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