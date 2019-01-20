using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FTP2
{
    class Program
    {
        static void Main(string[] args)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["hostname"]);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            string r;

            //connection options
            request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["password"]);
            request.KeepAlive = false;
            request.UseBinary = true;
            request.UsePassive = true;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            Console.WriteLine(reader.ReadToEnd());

            Console.WriteLine("Server response: {0}", response.StatusDescription);
            
            while(true)
            {
                Console.WriteLine("Enter directory path to list its contents. Enter 'Q' to quit.");
                r = Console.ReadLine();
                if (r == "Q" || r == "q")
                    break;
                request = (FtpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["hostname"]+r);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["password"]);
                response = (FtpWebResponse)request.GetResponse();

                responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
                Console.WriteLine(reader.ReadToEnd());

                Console.WriteLine("Server response: {0}", response.StatusDescription);
            }

            reader.Close();
            response.Close();

        }
    }
}
