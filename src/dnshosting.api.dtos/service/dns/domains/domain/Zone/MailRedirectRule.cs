using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnshosting.api.dtos.service.dns.domains.domain.Zone
{

    public class MailRedirectRule
    {
        public bool? is_default { get; set; }
        public string? to { get; set; }
        public string? from { get; set; }
    }
}
