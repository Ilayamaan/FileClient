using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace FileClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const string serverAddress = "127.0.0.1";
            const int port = 5000;
            string saveDirectory = @"C:\FileClient";

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            Console.Write("Enter the file name to download: ");
            string fileName = Console.ReadLine();

            try
            {
                TcpClient client = new TcpClient(serverAddress, port);
                using (NetworkStream stream = client.GetStream())
                {
                    // Send file request
                    byte[] requestBytes = Encoding.UTF8.GetBytes(fileName);
                    stream.Write(requestBytes, 0, requestBytes.Length);

                    // Receive file data
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, bytesRead);
                        }

                        byte[] fileData = ms.ToArray();
                        string response = Encoding.UTF8.GetString(fileData);

                        if (response == "File not found")
                        {
                            Console.WriteLine("File not found on the server.");
                        }
                        else
                        {
                            string filePath = Path.Combine(saveDirectory, fileName);
                            File.WriteAllBytes(filePath, fileData);
                            Console.WriteLine("File saved at " + filePath);
                        }
                    }
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message.ToString());
            }
        }
    }
}
