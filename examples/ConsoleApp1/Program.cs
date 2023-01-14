using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using dnshosting.api;
using dnshosting.api.dtos.service.dns.domains.domain.Zone;
using System;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddCommandLine(args);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient<DnsHostingClient>();
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

ILogger<Program> _logger = host.Services.GetRequiredService<ILogger<Program>>();
IConfiguration _configuration = host.Services.GetRequiredService<IConfiguration>();
DnsHostingClient client = host.Services.GetRequiredService<DnsHostingClient>();

try
{
    // Get options 
    string username = _configuration.GetValue<string>("user") ?? throw new ArgumentException("user is null or empty");
    string password = _configuration.GetValue<string>("password") ?? throw new ArgumentException("password is null or empty");
    string domain = _configuration.GetValue<string>("domain") ?? throw new ArgumentException("domain is null or empty");

    //   
    await client.LoginAsync(username, password);
    try
    {
        var testSubdomain = "DnsHostingClient";
        var ip1 = "8.8.8.8";
        var ip2 = "8.8.4.4";

        // create
        {
            var response = await client.GetDomainZoneAsync(domain);
            if (response.records != null)
            {
                int num = 1;
                foreach (var record in response.records!)
                {
                    record._num = num++;
                }

                // add record DnsHostingClient.{domain}. 3600 IN A 8.8.8.8
                response.records.Add(new Record()
                {
                    subdomain_name = testSubdomain,
                    data = ip1,
                    type = "A",
                    priority = 0,
                    _num = response.records.Count + 1
                });

                await client.UpdateDomainZoneAsync(domain, response);
            }
        }

        // validate create
        {
            var response = await client.GetDomainRecordAsync(domain, testSubdomain);
            string? data = response.list?.Where(
                x => (x.type ?? string.Empty).Equals("A", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()?.data;
            var validate = (data ?? string.Empty).Equals(ip1, StringComparison.CurrentCultureIgnoreCase);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Create ");
            Console.ForegroundColor = validate ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"{(validate ? "OK" : "FAILED")}");
            Console.ResetColor();
        }

        // update
        {
            var response = await client.GetDomainZoneAsync(domain);
            if (response.records != null)
            {
                int num = 1;
                foreach (var record in response.records!)
                {
                    record._num = num++;

                    // update record DnsHostingClient.{domain}. 3600 IN A 8.8.4.4
                    if ((record.subdomain_name ?? string.Empty).Equals(testSubdomain, StringComparison.CurrentCultureIgnoreCase) && (record.type ?? string.Empty).Equals("A", StringComparison.CurrentCultureIgnoreCase))
                    {
                        record.data = ip2;
                    }
                }

                await client.UpdateDomainZoneAsync(domain, response);
            }
        }

        // validate update
        {
            var response = await client.GetDomainRecordAsync(domain, testSubdomain);
            string? data = response.list?.Where(
                x => (x.type ?? string.Empty).Equals("A", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()?.data;

            var validate = (data ?? string.Empty).Equals(ip2, StringComparison.CurrentCultureIgnoreCase);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Update ");
            Console.ForegroundColor = validate ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"{(validate ? "OK" : "FAILED")}");
            Console.ResetColor();
        }

        // delete
        {
            var response = await client.GetDomainZoneAsync(domain);
            if (response.records != null)
            {
                // delete record
                response.records.RemoveAll(x => x.subdomain_name.Equals(testSubdomain, StringComparison.CurrentCultureIgnoreCase));

                int num = 1;
                foreach (var record in response.records!)
                {
                    record._num = num++;
                }

                await client.UpdateDomainZoneAsync(domain, response);
            }
        }

        // validate delete
        {
            var response = await client.GetDomainRecordAsync(domain, testSubdomain);
            string? data = response.list?.Where(
                x => (x.type ?? string.Empty).Equals("A", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()?.data;
            var validate = string.IsNullOrEmpty(data);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Delete ");
            Console.ForegroundColor = validate ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"{(validate ? "OK" : "FAILED")}");
            Console.ResetColor();
        }

        // GetDomainZoneAsync
        {
            var response = await client.GetDomainZoneAsync(domain);
            foreach (var record in response.records!)
            {
                Console.WriteLine($"{domain} {record.type}/{record.subdomain_name}= {record.data}");
            }
        }
    
    }
    finally
    {
        await client.LogoutAsync();
    }

}
catch (Exception ex)
{
    _logger.LogError(ex, "DnsHostingClient error.");
}





