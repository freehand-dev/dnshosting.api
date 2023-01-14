using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnshosting.api.dtos.service.dns.domains.domain.Zone
{
    public class RecordsBySubdomain
    {
        public int? total_count { get; set; }
        public List<Record>? list { get; set; }
    }
}
