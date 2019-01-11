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
                Credentials = new System.Net.NetworkCredential("postmaster@m183.tk", "a0dd02505f71a90ea50aff0d7f60546a-060550c6-0d2b2677"),
                EnableSsl = true,
               
                
               
            };
            client.Send("postmaster@m183.tk", "marcchardonnens1@gmail.com", "test", "testbody");
            Console.WriteLine("Sent");
            Console.ReadLine();

        }
    }
}
