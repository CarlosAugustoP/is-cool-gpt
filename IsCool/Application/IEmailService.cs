using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsCool.Application
{
    public interface IEmailService
    {
        Task SendEmail(string to, string body, string subject);
    }
}