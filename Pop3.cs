using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace POP3_CS
{
    class Klient
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Press 'Q' to quit program.");
            List<String> uidls = FirstConnection();
            Pyntla petla = new Pyntla();
            petla.Pentla(uidls);
            return;
        }

        public static List<String> FirstConnection()
        {
            Pop3 obj = new Pop3();
            //SslStream mySsl;
            obj.Connect(ConfigurationManager.AppSettings["website"],
                ConfigurationManager.AppSettings["username"],
                ConfigurationManager.AppSettings["password"]);

            List<String> uidls = obj.Unique();

            foreach (String uidl in uidls)
            {
                Console.WriteLine(uidl);
            }

            Console.WriteLine("Press Q to quit. Press any other key to check for new messages!");
            obj.Disconnect();
            return uidls;
        }

    }

    public class Pyntla
    {

        public void Pentla(List<String> uidls)
        {
            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                Pop3 obj = new Pop3();
                //SslStream mySsl;
                obj.Connect(ConfigurationManager.AppSettings["website"],
                    ConfigurationManager.AppSettings["username"],
                    ConfigurationManager.AppSettings["password"]);

                /*ArrayList list = obj.List();
                foreach (Pop3Message msg in list)
                {
                    Pop3Message msg2 = obj.Retrieve(msg);
                    Console.WriteLine("Message {0}: {1}",
                        msg2.number, msg2.message);
                }*/

                List<String> uidls2 = obj.Unique().Except(uidls).ToList();

                Console.WriteLine("\nYou received {0} new messages", uidls2.Count);

                //Console.WriteLine("Press Q to quit. Press any other key to check for new messages!");
                obj.Disconnect();
            }
        }
    }

    public class Pop3Message
    {
        public long number;
        public long bytes;
        public bool retrieved;
        public string message;
    }

    public class Pop3 : TcpClient
    {
        public void Connect(string server, string username, string password)
        {
            string message;
            string response;

            Connect(server, Int32.Parse(ConfigurationManager.AppSettings["port"]));
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }

            message = "USER " + username + "\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }

            message = "PASS " + password + "\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }
        }

        public void Disconnect()
        {
            string message;
            string response;
            message = "QUIT\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }
        }

        public Pop3Message Retrieve(Pop3Message rhs)
        {
            string message;
            string response;

            Pop3Message msg = new Pop3Message();
            msg.bytes = rhs.bytes;
            msg.number = rhs.number;

            message = "RETR " + rhs.number + "\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }

            msg.retrieved = true;
            while (true)
            {
                response = Response();
                if (response == ".\r\n")
                {
                    break;
                }
                else
                {
                    msg.message += response;
                }
            }
            return msg;
        }

        public List<String> Unique()
        {
            string message;
            string response;
            
            List<string> uids = new List<string>();
            message = "UIDL\r\n";
            Write(message);
            response = Response();


            while (true)
            {
                response = Response();
                if (response == ".\r\n")
                {
                    break;
                }
                else
                {
                    uids.Add(response/*.Split(' ')[1]*/);
                }
            }
            return uids;
        }

        public void Delete(Pop3Message rhs)
        {
            string message;
            string response;

            message = "DELE " + rhs.number + "\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }


        }

        private void Write(string message)
        {
            System.Text.ASCIIEncoding en = new System.Text.ASCIIEncoding();

            byte[] WriteBuffer = new byte[1024];
            WriteBuffer = en.GetBytes(message);

            NetworkStream stream = GetStream();
            stream.Write(WriteBuffer, 0, WriteBuffer.Length);

            Debug.WriteLine("WRITE:" + message);
        }

        public ArrayList List()
        {
            string message;
            string response;

            ArrayList retval = new ArrayList();
            message = "LIST\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }

            while (true)
            {
                response = Response();
                if (response == ".\r\n")
                {
                    return retval;
                }
                else
                {
                    Pop3Message msg = new Pop3Message();
                    char[] seps = { ' ' };
                    string[] values = response.Split(seps);
                    msg.number = Int32.Parse(values[0]);
                    msg.bytes = Int32.Parse(values[1]);
                    msg.retrieved = false;
                    retval.Add(msg);
                    continue;
                }
            }
        }

        private string Response()
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] serverbuff = new Byte[1024];
            NetworkStream stream = GetStream();
            int count = 0;
            while (true)
            {
                byte[] buff = new Byte[2];
                int bytes = stream.Read(buff, 0, 1);
                if (bytes == 1)
                {
                    serverbuff[count] = buff[0];
                    count++;

                    if (buff[0] == '\n')
                    {
                        break;
                    }
                }
                else
                {
                    break;
                };
            };

            string retval = enc.GetString(serverbuff, 0, count);
            Debug.WriteLine("READ:" + retval);
            return retval;
        }

    }
    public class Pop3Exception : System.ApplicationException
    {
        public Pop3Exception(string str) : base(str)
        {
        }
    }
}