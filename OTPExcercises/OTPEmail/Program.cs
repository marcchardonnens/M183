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


            MailMessage mail = new MailMessage("debbie@isadork.com", "deborah.senn.89@outlook.ch");
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("debbie.isadork@gmail.com", "debdeb123+");
            mail.Subject = "this is a test email.";
            mail.Body = "this is my test email body";



            for (int i = 0; i < 100; i++)
            {


                client.Send(mail);
            }



        }
    }
}
