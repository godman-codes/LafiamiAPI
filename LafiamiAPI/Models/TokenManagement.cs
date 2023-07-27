using LafiamiAPI.Models.Internals;
using LafiamiAPI.Utilities.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LafiamiAPI.Models
{
    public class TokenManagement
    {
        private readonly JWTSettings jWTSettings;
        private readonly List<Claim> _claim;
        private DateTime IssuedDate;

        private void GenerateIssueDate()
        {
            IssuedDate = DateTime.UtcNow;
        }

        public TokenManagement(JWTSettings settings)
        {
            jWTSettings = settings;
            GenerateIssueDate();
        }

        public TokenManagement(JWTSettings settings, List<Claim> claims)
        {
            jWTSettings = settings;
            _claim = claims;
            GenerateIssueDate();
        }

        public SymmetricSecurityKey GetSigningKey()
        {
            // turn the secret key to byte
            // amd get its symmetric key
            byte[] key = Encoding.ASCII.GetBytes(jWTSettings.Secret);
            return new SymmetricSecurityKey(key);
        }

        public DateTime Expires()
        {
            // adds the normal datetime now which issued date is to the expiring days that is expected 
            // which is seven
            return IssuedDate.AddDays(jWTSettings.ExpiringDays);
        }

        public string GetJWTToken()
        {
            if (_claim == null)
            {
                throw new Exception("No claims Provided");
            }
            // handler used to create and validate jwt token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            
            // describe the token then pass it to the handler
            // to create a token
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                //Audience = queryResult.Code,
                Issuer = string.Join(Constants.Comma, jWTSettings.Issurer, jWTSettings.TestIssurer),
                IssuedAt = IssuedDate,
                Expires = Expires(),
                NotBefore = DateTime.UtcNow,
                Subject = new ClaimsIdentity(_claim),
                SigningCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public Action<JwtBearerOptions> ValidateToken()
        { 
            // returns the token validation configurations
            return x =>
            {
                // this is set to require https for all request
                x.RequireHttpsMetadata = true;
                // this is to save the token in the authentication ticket
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GetSigningKey(),
                    ValidIssuer = string.Join(Constants.Comma, jWTSettings.Issurer, jWTSettings.TestIssurer),
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };
            };
        }
    }
}
