using System;

namespace CSVIngestion
{
    class Program
    {
        static void Main(string[] args)
        {
            //log information. CSVIngestion started: DateTime.Now()
            Console.WriteLine($"CSVIngestion\tInformation\tCSVIngestion started\t{DateTime.Now}");
            //get path from args or Refernce
            var path = args.Length > 0 ? args[0] : Reference.inputFolderPath;

            //create instance of fileProcessor
            var processor = new CSVEnrollmentProcessor();
            //start fileProcessor.process
            processor.ProcessEnrollment(path);
            //log information. CSVIngestion completed: DateTime.Now()
            Console.WriteLine($"CSVIngestion\tInformation\tCSVIngestion completed\t{DateTime.Now}");
        }
    }
}
