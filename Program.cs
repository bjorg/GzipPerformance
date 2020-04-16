using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace GzipPerformance {

    public class AddVsCopyTo {

        //--- Constants ---
        private const string DATA = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

        //--- Fields ---
        private readonly string EncodedData;

        //--- Constructors ---
        public AddVsCopyTo() {

            // prepare test data
            var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(DATA));
            var compressedStream = new MemoryStream();
            using(var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal)) {
                uncompressedStream.CopyTo(gzipStream);
                gzipStream.Flush();
                compressedStream.Position = 0;
                this.EncodedData = Convert.ToBase64String(compressedStream.ToArray());
            }

            // validate implementations
            if(DecodeDataAdd() != DATA) {
                throw new Exception("DecodeData() failed");
            }
            if(DecodeDataStream() != DATA) {
                throw new Exception("DecodeData() failed");
            }
        }

        //--- Methods ---

        [Benchmark]
        public string DecodeDataAdd() {
            if(string.IsNullOrEmpty(this.EncodedData))
                return this.EncodedData;

            var bytes = Convert.FromBase64String(this.EncodedData);
            var uncompressedBytes = new List<byte>();

            using(var stream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress)) {
                int b;
                while((b = stream.ReadByte()) != -1) {
                    uncompressedBytes.Add((byte)b);
                }
            }

            var decodedString = Encoding.UTF8.GetString(uncompressedBytes.ToArray());
            return decodedString;
        }

        [Benchmark]
        public string DecodeDataStream() {
            if(string.IsNullOrEmpty(this.EncodedData)) {
                return this.EncodedData;
            }
            var bytes = Convert.FromBase64String(this.EncodedData);
            var uncompressedStream = new MemoryStream();
            using(var stream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress)) {
                stream.CopyTo(uncompressedStream);
                uncompressedStream.Position = 0;
            }
            var decodedString = Encoding.UTF8.GetString(uncompressedStream.ToArray());
            return decodedString;
        }
    }

    public class Program {

        //--- Class Methods ---
        public static void Main(string[] args) {
            BenchmarkRunner.Run<AddVsCopyTo>();
        }
    }
}
