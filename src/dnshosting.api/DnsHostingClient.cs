using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using dnshosting.api.dtos.auth;
using System.Net;
using dnshosting.api.dtos.service.dns.domains.domain.Zone;

namespace dnshosting.api
{
    public class DnsHostingClient
    {
        private readonly ILogger<DnsHostingClient>? _logger;

        public string? APIToken 
        { 
            get => this.Client.DefaultRequestHeaders.Authorization?.Parameter; 
            set => this.Client.DefaultRequestHeaders.Authorization = (value != null ?  new AuthenticationHeaderValue("APIToken", value) : null); 
        }    
        public HttpClient Client
        {
            get;
            set;
        }

        public Uri? BaseAddress
        {
            get
            {
                return this.Client?.BaseAddress;
            }
            set
            {
                this.Client.BaseAddress = value;
            }
        }

        public DnsHostingClient(HttpClient httpClient, ILogger<DnsHostingClient>? logger = null)
        {
            this._logger = logger;
            this.Client = httpClient;
            this.Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue(typeof(DnsHostingClient).FullName ?? "HuaweiWS5200Client")));
            this.Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json;charset=UTF-8");
            this.Client.BaseAddress = new Uri("https://api.dnshosting.org");
        }


        public async Task<string?> LoginAsync(string username, string password)
        {

            AuthRequestDto authDTO = new AuthRequestDto() { login = username, password = password};

            var requestContent = new StringContent(JsonSerializer.Serialize<AuthRequestDto>(
                    authDTO,
                    new JsonSerializerOptions()
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    }
                ), Encoding.UTF8, "application/json");

            var response = await this.Client.PostAsync(@"/auth", requestContent);
            response.EnsureSuccessStatusCode();
            using var contentStream = await response.Content.ReadAsStreamAsync();

            AuthResponseDto? result = await JsonSerializer.DeserializeAsync<AuthResponseDto>(contentStream);
            this.APIToken = result?.token;

            return APIToken;
        }

        public async Task LogoutAsync()
        {
            var response = await this.Client.DeleteAsync(@"/auth");
            response.EnsureSuccessStatusCode();
            this.APIToken = null;
        }


        public async Task UpdateDomainZoneAsync(string domain, Zone zoneDTO)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize<Zone>(
                    zoneDTO,
                    new JsonSerializerOptions()
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    }
                ), Encoding.UTF8, "application/json");

            var response = await this.Client.PutAsync($"/service/dns/domains/{domain}/zone", requestContent);
            response.EnsureSuccessStatusCode();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Zone> GetDomainZoneAsync(string domain)
        {
            var response = await this.Client.GetFromJsonAsync<Zone>($"/service/dns/domains/{domain}/zone");
            return response ?? throw new Exception("GetDomainZoneAsync");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="subdomain"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<RecordsBySubdomain> GetDomainRecordAsync(string domain, string subdomain)
        {
            var response = await this.Client.GetFromJsonAsync<RecordsBySubdomain>($"/service/dns/domains/{domain}/zone/records_by_subdomain/{subdomain}");
            return response ?? throw new Exception("GetDomainRecordAsync");
        }
    }
}