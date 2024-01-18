// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Helper;

namespace Helper.Identity
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            //If already a 401 Response is waiting do nothing
            if (context.Response.HasStarted)
            {
                return;
            }

            //// If no config present do nothing
            //if (ratelimitconfig is null)
            //{
            //    await _next(context);
            //    return;
            //}

            //GET BEARER TOKEN from Authorization Header
            var bearertoken = "";
           
            if (context.Request.Headers.ContainsKey("Authorization"))
                bearertoken = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(bearertoken) && bearertoken.StartsWith("Bearer"))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = bearertoken.Replace("Bearer", "").Trim();

                var authorizationtoken = RequestAuthorizationEndpoint(token);

                var jwttoken = ReadMyJWTSecurityToken(authorizationtoken, handler);

                if (jwttoken != null)
                {
                    //Read the assigned roles and add some access logic
                }
            }

            await _next(context);
        }

        private static JwtSecurityToken? ReadMyJWTSecurityToken(string token, JwtSecurityTokenHandler handler)
        {
            try
            {
                var jwttoken = handler.ReadJwtToken(token);

                return jwttoken;
            }
            catch
            {
                return null;
            }
        }

        private static string RequestAuthorizationEndpoint(string token)
        {
            //TODO REQUEST AUTHORIZATION ON KEYCLOAK with grant_type=urn:ietf:params:oauth:grant-type:uma-ticket AND audience=odh-tourism-api
            //and pass the bearer token

            return "";
        }
    }
}

