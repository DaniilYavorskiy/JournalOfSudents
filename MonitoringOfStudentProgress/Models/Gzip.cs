using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace JournalOfStudents.Models {
    internal class Gzip {
        public static byte[] Compress(byte[] input) {
            using var result = new MemoryStream();
            var lengthBytes = BitConverter.GetBytes(input.Length);
            result.Write(lengthBytes, 0, 4);
            using (var compressionStream = new GZipStream(result, CompressionMode.Compress)) {
                compressionStream.Write(input, 0, input.Length);
                compressionStream.Flush();
            }
            return result.ToArray();
        }
        public static byte[] Decompress(byte[] input) {
            using var source = new MemoryStream(input);
            byte[] lengthBytes = new byte[4];
            source.Read(lengthBytes, 0, 4);
            var length = BitConverter.ToInt32(lengthBytes, 0);
            using var decompressionStream = new GZipStream(source, CompressionMode.Decompress);
            var result = new byte[length];
            decompressionStream.Read(result, 0, length);
            return result;
        }
        public static byte[] PackAllStudents(ObservableCollection<Student> studentList) {
            StringBuilder sb = new();
            bool first = true;
            foreach (Student student in studentList) {
                if (first) first = false;
                else sb.Append('\n');
                sb.Append(student.Pack());
            }
            byte[] encoded = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] compressed = Gzip.Compress(encoded);
            return compressed;
        }
        public static void UnpackAllStudents(ObservableCollection<Student> studentList, byte[] compressed) {
            byte[] decompressed = Gzip.Decompress(compressed);
            String data = Encoding.UTF8.GetString(decompressed);
            studentList.Clear();
            foreach (String pack in data.Split("\n")) studentList.Add(new Student(pack));
        }
    }
}
