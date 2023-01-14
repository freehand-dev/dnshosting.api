using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace dnshosting.api.dtos.auth
{
    public class AuthRequestDto
    {
        public string? login { get; set; }
        public string? password { get; set; }
    }

}
