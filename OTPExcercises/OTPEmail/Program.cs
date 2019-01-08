using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;


namespace OTPEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new SmtpClient("smtp.mailgun.org", 587)
            {
                Credentials = new System.Net.NetworkCredential("postmaster@sandbox225443442a884472b81a9bdb12d6ac7f.mailgun.org", "2d0cd3a9ffbf26430a7febb9fb66e265-060550c6-6d976bb2"),
                EnableSsl = true
                
               
            };
            client.Send("postmaster@sandbox225443442a884472b81a9bdb12d6ac7f.mailgun.org", "marcchardonnens1@gmail.com", "test", "testbody");
            Console.WriteLine("Sent");
            Console.ReadLine();

        }
    }
}
