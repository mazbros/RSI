using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RSI.Helpers
{
    public class CsvDownloader<T> : FileResult
    {
        private readonly IList<T> _list;
        private readonly char _separator;

        public CsvDownloader(IList<T> list,
            string fileDownloadName,
            char separator = ',')
            : base("text/csv")
        {
            _list = list;
            FileDownloadName = fileDownloadName;
            _separator = separator;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            var outputStream = response.OutputStream;
            using (var memoryStream = new MemoryStream())
            {
                writeList(memoryStream);
                outputStream.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
            }
        }

        private void writeList(Stream stream)
        {
            var streamWriter = new StreamWriter(stream, Encoding.Default);

            writeHeaderLine(streamWriter);
            streamWriter.WriteLine();
            writeDataLines(streamWriter);

            streamWriter.Flush();
        }

        private void writeHeaderLine(StreamWriter streamWriter)
        {
            foreach (var member in typeof(T).GetProperties())
                writeValue(streamWriter, member.Name);
        }

        private void writeDataLines(StreamWriter streamWriter)
        {
            foreach (var line in _list)
            {
                foreach (var member in typeof(T).GetProperties())
                    writeValue(streamWriter, GetPropertyValue(line, member.Name));
                streamWriter.WriteLine();
            }
        }


        private void writeValue(TextWriter writer, string value)
        {
            writer.Write("\"");
            writer.Write(value.Replace("\"", "\"\""));
            writer.Write("\"" + _separator);
        }

        public static string GetPropertyValue(object src, string propName)
        {
            var res = src.GetType().GetProperty(propName).GetValue(src, null);
            return res?.ToString() ?? string.Empty;
        }
    }
}