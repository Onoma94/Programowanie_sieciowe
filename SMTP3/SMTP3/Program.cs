using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace SMTP3
{
    class Program
    {
        static void Main(string[] args)
        {
            var message = new MimeMessage();
            string name, address;

            //From info
            Console.WriteLine("Give the sender's name:\n");
            name = Console.ReadLine();
            Console.WriteLine("Give the sender's correct e-mail address:\n");
            address = Console.ReadLine();
            message.From.Add(new MailboxAddress(name, address));

            //To info
            Console.WriteLine("Give the receiver's name:\n");
            name = Console.ReadLine();
            Console.WriteLine("Give the receiver's correct e-mail address:\n");
            address = Console.ReadLine();
            message.To.Add(new MailboxAddress(name, address));

            //Console.WriteLine(name + "\n" + address);
            message.Subject = "PS LAB LATO 2017 (nr grupy laboratoryjnej)";
            message.Body = new TextPart () { Text = @"Artur Gierach" };
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.wp.pl", 465, true);
                client.Authenticate("niathal91", "ulicaszyca1");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
