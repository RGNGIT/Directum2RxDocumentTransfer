namespace Directum2RxDocumentTransfer.Utils
{
    public static class Logger
    {
        private static string logFilePath = $"session_{DateTime.Now.ToString("dd.MM.yyyy")}_log.txt";

        public static void Debug(string message) 
        {
            var messageToLog = $"{DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss")} Thread {Thread.CurrentThread.ManagedThreadId}: {message}";

            using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                writer.WriteLine(messageToLog);

            Console.WriteLine(messageToLog);
        }
    }
}
