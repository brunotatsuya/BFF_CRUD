﻿using BFF_CRUD.Models;
using BFF_CRUD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BFF_CRUD.Controllers
{

    [ApiController]
    public class Authentication : ControllerBase
    {
        private IConfiguration _configuration;

        public Authentication(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        [Route("auth/resource_owner")]
        [HttpPost]
        public IActionResult RequestResourceOwnerToken(ResourceOwner credentials)
        {
            string stsAccessToken = STSAuthService.TrySTSAuthentication(credentials);
            if (string.IsNullOrEmpty(stsAccessToken))
                return Unauthorized("As credenciais informadas não são válidas para autenticação STS.");
            if (!STSAuthService.ValidateGSIGroup(stsAccessToken, _configuration))
                return Unauthorized("As credenciais informadas não têm autorização para utilizar este serviço.");
            var token = TokenService.GenerateToken(_configuration);
            return Ok(token);
        }

        [Route("auth/client_credentials")]
        [HttpPost]
        public IActionResult RequestClientCredentialsToken(ClientCredentials credentials)
        {
            if (!STSAuthService.TrySTSAuthentication(credentials))
                return Unauthorized("As credenciais informadas não são válidas para autenticação STS.");
            var token = TokenService.GenerateToken(_configuration);
            return Ok(token);
        }
    }
}
