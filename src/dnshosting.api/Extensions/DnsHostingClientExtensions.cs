using dnshosting.api.dtos.service.dns.domains.domain.Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnshosting.api.Extensions
{
    public static class DnsHostingClientExtensions
    {
        public static async Task AddOrUpdateRecord(this DnsHostingClient client, string domain, string type, List<string> subdomains, string data)
        {
            Zone zone = await client.GetDomainZoneAsync(domain);

            bool updated = false;
            if (zone.records != null)
            {
                int num = 1;
                foreach (var record in zone.records!)
                {
                    record._num = num++;
                    if (subdomains.Any(s => s.Equals(record.subdomain_name, StringComparison.CurrentCultureIgnoreCase)) && type.Equals(record.type, StringComparison.CurrentCultureIgnoreCase))
                    {
                        subdomains.Remove(record.subdomain_name!);
                        if (!(data).Equals(record.data, StringComparison.CurrentCultureIgnoreCase))
                        {
                            record.data = data;
                            updated = true;
                        }
                    }
                }

                foreach (string subdomain in subdomains)
                {
                    zone.records.Add(new Record()
                    {
                        subdomain_name = subdomain,
                        data = data,
                        type = type,
                        priority = 0,
                        _num = zone.records.Count + 1
                    });
                    updated = true;
                }


                if (updated)
                {
                    await client.UpdateDomainZoneAsync(domain, zone);
                }
            }
        }


        public static async Task UpdateRecord(this DnsHostingClient client, string domain, string type, List<string> subdomains, string data)
        {
            Zone zone = await client.GetDomainZoneAsync(domain);

            bool updated = false;

            if (zone.records != null)
            {
                int num = 1;
                foreach (var record in zone.records!)
                {
                    record._num = num++;

                    if (subdomains.Any(s => s.Equals(record.subdomain_name, StringComparison.CurrentCultureIgnoreCase)) && type.Equals(record.type, StringComparison.CurrentCultureIgnoreCase))
                    {
                        subdomains.Remove(record.subdomain_name!);
                        if (!(data).Equals(record.data, StringComparison.CurrentCultureIgnoreCase))
                        {
                            record.data = data;
                            updated = true;
                        }
                    }
                }

                if (updated)
                {
                    await client.UpdateDomainZoneAsync(domain, zone);
                }                
            }
        }
    }
}
