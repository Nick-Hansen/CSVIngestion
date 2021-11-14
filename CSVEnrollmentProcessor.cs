using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVIngestion
{
    public class CSVEnrollmentProcessor {
        public void ProcessEnrollment(string path) {
            if(File.Exists(path))
            {
                try {
                    ProcessEnrollmentFile(path);
                    //move file to Processed folder
                }
                catch (Exception ex)
                {
                    //log error. Process file exception
                    var fileName = Path.GetFileName(path);
                    Console.WriteLine($"CSVIngestion\tError\tProcess File Exception: {fileName}\t{ex.Message}\t{DateTime.Now}");
                    //move file to Failures folder
                }
            }
            else if(Directory.Exists(path))
            {
                try {
                    ProcessEnrollmentDirectory(path);
                }
                catch (Exception ex)
                {
                    //log error. Process directory exception
                    Console.WriteLine($"CSVIngestion\tError\tProcess Directory Exception: {path}\t{ex.Message}\t{DateTime.Now}");
                }
            }
            else
            {
                //log error. Path not found
                Console.WriteLine($"CSVIngestion\tError\tEnrollment Input Path Not Valid: {path}\t{DateTime.Now}");
            }
        }

        public void ProcessEnrollmentDirectory(string directoryPath) {            
            //log information. Process directory started: fileName: DateTime.Now()
            Console.WriteLine($"CSVIngestion\tInformation\tProcess directory start: {directoryPath}\t{DateTime.Now}");
            
            //locate files to process in folder
            //this may be made more specific to only process files meeting naming requirements
            string [] fileEntries = Directory.GetFiles(directoryPath,"*.csv");

            //process each file
            foreach(string filePath in fileEntries) {
                try {
                    ProcessEnrollmentFile(filePath);
                    //move file to Processed folder
                }
                catch (Exception ex)
                {
                    //log error. Process file exception
                    var fileName = Path.GetFileName(filePath);
                    Console.WriteLine($"CSVIngestion\tError\tProcess File Exception: {fileName}\t{ex.Message}\t{DateTime.Now}");
                    //move file to Failures folder
                }
            }
            //log information. Process directory complete: fileName: DateTime.Now()
            Console.WriteLine($"CSVIngestion\tInformation\tProcess directory complete: {directoryPath}\t{DateTime.Now}");
        }

        public void ProcessEnrollmentFile(string filePath) {
            var fileName = Path.GetFileName(filePath);
            //log information. Process file started: fileName: DateTime.Now()
            Console.WriteLine($"CSVIngestion\tInformation\tProcess file start: {fileName}\t{DateTime.Now}");

            List<string[]> enrollmentValues = new List<string[]>();
            // Open the text file using a stream reader.
            using (var sr = new StreamReader(filePath)) {
                while (!sr.EndOfStream) {
                    var line = sr.ReadLine();
                    var values = line.Split(',');
                    enrollmentValues.Add(values);
                }
            }

            int intPlaceholder = 0;
            var enrollments = enrollmentValues.Select(e => new Enrollment() {
                UserId = int.TryParse(e[(int)Reference.Enrollment_Format.User_Id], out intPlaceholder) ? intPlaceholder : 0,
                FirstName = e[(int)Reference.Enrollment_Format.First_Name],
                LastName = e[(int)Reference.Enrollment_Format.Last_Name],
                Version = int.TryParse(e[(int)Reference.Enrollment_Format.Version], out intPlaceholder) ? intPlaceholder : 0,
                Company = e[(int)Reference.Enrollment_Format.Insurance_Company]
            })
            .GroupBy(e => new {
                Company = e.Company,
                UserId = e.UserId
            }).Select(a => new {
                UserId = a.Key.UserId,
                Company = a.Key.Company,
                Version = a.OrderByDescending(x => x.Version).Select(x => new Enrollment() {
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Version = x.Version,
                    Company = x.Company
                }).FirstOrDefault()
            }).Select(x => x.Version)
            .GroupBy(e => e.Company)
            .ToList();

            foreach(var company in enrollments) {
                var path = $"{Path.GetDirectoryName(filePath)}/{company.Key} {DateTime.Now.ToString("yyyy.MM.dd")}.csv";
                //log information. Company file started: fileName: DateTime.Now()
                Console.WriteLine($"CSVIngestion\tInformation\tCompany file start: {path}\t{DateTime.Now}");
                var sb = new StringBuilder();
                string line = string.Empty;
                // uncomment if headers are needed for company csv files
                // line = $"User Id,First Name,Last Name,Version,Company";
                // sb.AppendLine(line);
                foreach (var enrollee in company.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)) {
                    line = $"{enrollee.UserId},{enrollee.FirstName},{enrollee.LastName},{enrollee.Version},{enrollee.Company}";
                    sb.AppendLine(line);
                }
                File.WriteAllText(path, sb.ToString());
                //log information. Company file created: fileName: DateTime.Now()
                Console.WriteLine($"CSVIngestion\tInformation\tCompany file created: {path}\t{DateTime.Now}");
            }

            //log information. Process file complete: fileName: DateTime.Now()
            Console.WriteLine($"CSVIngestion\tInformation\tProcess file complete: {fileName}\t{DateTime.Now}");
        }
    }
}