using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.Options
{
    public class MailSettingsOptions
    {
        public const string SectionName = "MailSettings";
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string DisplayName { get; set; } = "SmartCare";
    }
}
