using Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Services
{
    public class CsvWriterService
    {
        private readonly StreamWriter _streamWriter;

        public CsvWriterService(StreamWriter writer)
        {
            _streamWriter = writer;
        }

        public void WriteToCsv(IEnumerable<Message> data)
        {
            // Create a new CsvConfiguration with the InvariantCulture
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Set HasHeaderRecord to true if the CSV file is empty, false otherwise
                HasHeaderRecord = _streamWriter.BaseStream.Length == 0
            };

            // Create a new CsvWriter instance with the StreamWriter and configuration
            var csv = new CsvWriter(_streamWriter, config);

            // Write each Message object to the CSV file
            foreach (var message in data)
            {
                csv.WriteRecord(message);
                csv.NextRecord();
            }

            // Flush the StreamWriter to ensure all data is written to the file immediately
            _streamWriter.Flush();

            // Print a success message to the console
            Console.WriteLine("Data written to CSV file successfully");
        }
    }
}
