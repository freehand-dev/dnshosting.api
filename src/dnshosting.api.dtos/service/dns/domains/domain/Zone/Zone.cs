using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnshosting.api.dtos.service.dns.domains.domain.Zone
{
    public class Zone
    {
        public string? updated { get; set; }
        public int? retry { get; set; }
        public ZoneHash? hash { get; set; }
        public string? created { get; set; }
        public int? negative_ttl { get; set; }
        public bool? hasHistory { get; set; }
        public int? refresh { get; set; }
        public List<Record>? records { get; set; }
        public int? expire { get; set; }
        public string? link { get; set; }
        public int? ttl { get; set; }
    }

}
