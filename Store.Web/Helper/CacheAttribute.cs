﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Store.Service.Services.CacheService;
using System.Net.Mime;
using System.Text;

namespace Store.Web.Helper
{
    public class CacheAttribute:Attribute,IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CacheAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var _cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await _cacheService.GetCacheResponseAsync(cacheKey);
            if(!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode=200
                };

                context.Result = contentResult;
                return;
            }

            var exceutedContext = await next();
            if (exceutedContext.Result is OkObjectResult response)
                await _cacheService.SetCacheResponseAsync(cacheKey, response.Value,TimeSpan.FromSeconds(_timeToLiveInSeconds));
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            StringBuilder cacheKey = new StringBuilder();
            cacheKey.Append($"{request.Path}"); 
            foreach ( var (key,value) in request.Query.OrderBy(x=>x.Key))
            
                cacheKey.Append($"|{key}-{value}");
            
            return cacheKey.ToString();
           
        }
    }
}
