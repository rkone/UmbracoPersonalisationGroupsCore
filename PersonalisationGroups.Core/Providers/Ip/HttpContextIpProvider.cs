﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Our.Umbraco.PersonalisationGroups.Core.Configuration;

namespace Our.Umbraco.PersonalisationGroups.Core.Providers.Ip
{
    public class HttpContextIpProvider : IIpProvider
    {
        private readonly PersonalisationGroupsConfig _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientIpParser _clientIpParser;

        public HttpContextIpProvider(IOptions<PersonalisationGroupsConfig> config, IHttpContextAccessor httpContextAccessor, ClientIpParser clientIpParser)
        {
            _config = config.Value;
            _httpContextAccessor = httpContextAccessor;
            _clientIpParser = clientIpParser;
        }

        public string GetIp()
        {
            var ip = GetIpFromHttpContext();
            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }

            return ip;
        }

        private string GetIpFromHttpContext()
        {
            // Return a test Ip if we've configured one
            var testIp = _config.TestFixedIp; 
            if (!string.IsNullOrEmpty(testIp))
            {
                return testIp;
            }

            var aspIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(aspIp))
            {
                return aspIp;
            }

            // Otherwise retrieve from the HTTP context
            return _clientIpParser.ParseClientIp(_httpContextAccessor.HttpContext);
        }
    }
}
