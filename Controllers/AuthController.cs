using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using CognitoDemo.Model;
using Microsoft.AspNetCore.Mvc;

namespace CognitoDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration config;
        private RegionEndpoint region;
        private string? awsClientId;

        public AuthController(IConfiguration config)
        {
            this.config = config;
            region = RegionEndpoint.GetBySystemName(config["Cognito:Region"]);
            awsClientId = config["Cognito:ClientId"];
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var provider = new AmazonCognitoIdentityProviderClient(region);
            var request = new SignUpRequest
            {
                ClientId = awsClientId,
                Username = model.Username,
                Password = model.Password,

                //Other atributes
                UserAttributes ={
                    new AttributeType{
                        Name="email",
                        Value=model.Email
                    }
                }
            };

            try
            {
                var response = await provider.SignUpAsync(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Registration failed: {ex.Message}");
            }
        }
        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromBody] ConfirmModel model)
        {
            var provider = new AmazonCognitoIdentityProviderClient(region);
            var confirmRequest = new ConfirmSignUpRequest
            {
                ClientId = awsClientId,
                Username = model.Username,
                ConfirmationCode = model.ConfirmationCode
            };

            try
            {
                var confirmResponse = await provider.ConfirmSignUpAsync(confirmRequest);
                return Ok(confirmResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Confirmation failed: {ex.Message}");

            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var provider = new AmazonCognitoIdentityProviderClient(region);
            var request = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = awsClientId,
                AuthParameters = new Dictionary<string, string>
                {
                    {"USERNAME", model.Username},
                    {"PASSWORD", model.Password}
                }
            };

            try
            {
                var response = await provider.InitiateAuthAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Login failed: {ex.Message}");
            }
        }
    }
}