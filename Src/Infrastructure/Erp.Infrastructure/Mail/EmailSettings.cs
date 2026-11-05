using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Infrastructure.Mail
{
    public sealed class EmailSettings
    {
        public const string SectionName = "Email";
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 2525;
        public bool UseStartTls { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string FromAddress { get; set; } = "noreply@erp.local";
        public string FromName { get; set; } = "ERP Inventory & Orders";

    }
}
