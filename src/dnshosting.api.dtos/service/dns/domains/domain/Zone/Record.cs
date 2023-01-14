using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnshosting.api.dtos.service.dns.domains.domain.Zone
{
    public class Record
    {
        public string? updated { get; set; }
        public int? weight { get; set; }
        public string? created { get; set; }
        public string? type { get; set; }
        public string? subdomain_name { get; set; }
        public int? priority { get; set; }
        public int? record_id { get; set; }
        public string? data { get; set; }
        public int? port { get; set; }
        public string? type_extended { get; set; }
        public string? mail_redirect_type { get; set; }
        public List<MailRedirectRule>? mail_redirect_rules { get; set; }
        public int? _num { get; set; }   
    }
}
