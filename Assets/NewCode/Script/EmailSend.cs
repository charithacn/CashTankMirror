using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
public class EmailSend : MonoBehaviour
{

    // Start is called before the first frame update
    public void Start()
    {
        SendEmail();
    }
    void SendEmail()
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("yasithsaminthaka@gmail.com");
        mail.To.Add("autoplaybox@gmail.com");
        mail.Subject = "Password Verification";
        mail.Body = "Password Number";

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.EnableSsl = true;
        smtpServer.Credentials = new System.Net.NetworkCredential("yasithsaminthaka@gmail.com", "vave arox dbts rbfn") as ICredentialsByHost;

        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object obj, X509Certificate cert, X509Chain chain, SslPolicyErrors sslerrors)
            { return true; };

        smtpServer.Send(mail);

#if UNITY_EDITOR
        Debug.Log("email sent");
#endif

    }
}
