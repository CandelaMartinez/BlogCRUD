using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Utilidades
{
    public class EmailSender : IEmailSender
    {
        //clase 72: instalar Microsoft.Asp.NetCore.Identity.UI version 3.1.0
        //implementar interface
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
