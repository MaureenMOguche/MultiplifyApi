using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Multiplify.Domain.User;
using System.Security.Cryptography;

namespace Multiplify.Application.ServiceImplementations.Helpers;
public static class TokenProviders
{
    private static IMemoryCache? _memoryCache;
    
    public static void Initialize(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;

        
    }
    public static string GenerateSixDigitTokenAsync(string purpose, AppUser user)
    {
        if (_memoryCache == null)
            throw new Exception("Pass memory cache instance to Token providers initialize class");
        var cacheKey = $"{purpose}_{user.Id}";
        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

        return otp;
    }

    public static bool ValidateSixDigitTokenAsync(string purpose, string token, AppUser user)
    {
        if (_memoryCache == null)
            throw new Exception("Pass memory cache instance to Token providers initialize class");

        var cacheKey = $"{purpose}_{user.Id}";
        if(_memoryCache.TryGetValue(cacheKey, out string? cachedOtp))
            return token == cachedOtp;

        return false;
    }

    public static string GenerateEmailCodeToken(string purpose, string email)
    {
        if (_memoryCache == null)
            throw new Exception("Pass memory cache instance to Token providers initialize class");

        var cacheKey = $"{purpose}_{email}";
        var otp = $"{Guid.NewGuid()}_{RandomNumberGenerator.GetInt32(100000, 999999)}_{Guid.NewGuid()}";

        _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(7));

        return otp;
    }

    public static bool ValidateEmailCodeToken(string purpose, string token, string email)
    {
        if (_memoryCache == null)
            throw new Exception("Pass memory cache instance to Token providers initialize class");

        var cacheKey = $"{purpose}_{email}";
        if (_memoryCache.TryGetValue(cacheKey, out string? cachedOtp))
            return token == cachedOtp;

        return false;
    }
}


