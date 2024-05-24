using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Domain.User;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;


namespace Multiplify.Application.ServiceImplementations.Helpers;
public static class TokenProviders
{
    private static IMemoryCache? _memoryCache;
    private static IUnitOfWork? _db;
    
    public static void Initialize(IMemoryCache memoryCache, IUnitOfWork db)
    {
        _memoryCache = memoryCache;
        _db = db;
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

    public static string GeneratePasswordResetToken(string purpose, string userId)
    {
        if (_memoryCache == null)
            throw new Exception("Pass memory cache instance to Token providers initialize class");

        var cacheKey = $"{purpose}:{userId}";
        var otp = $"{purpose}:{userId}:{Guid.NewGuid()}_{RandomNumberGenerator.GetInt32(100000, 999999)}_{Guid.NewGuid()}";

        _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(7));

        return otp;
    }

    public static async Task<(bool, string)> ValidatePasswordResetToken(string purpose, string token, string password)
    {
        if (_memoryCache == null)
            throw new Exception("Pass memory cache instance to Token providers initialize class");

        if (_db == null)
            throw new Exception("Pass db instance");

        var otpParts = token.Split(':');
        if (otpParts.Length != 3)
            return (false, "Invalid Token");

        if (otpParts[0] != purpose)
            return (false, "Invalid Token");

        _ = _memoryCache.TryGetValue(otpParts[1], out List<string>? passwordList);

        if (passwordList != null)
        {
            if (passwordList.Contains(password))
                return (false, "You cannot use a password you have previously used");

            passwordList.Add(password);
            _memoryCache.Set(otpParts[1], passwordList);
        }



        var cacheKey = $"{purpose}:{otpParts[1]}";

        if (_memoryCache.TryGetValue(cacheKey, out string? cachedOtp))
        {
            if (token != cachedOtp)
                return (false, "Invalid Token");

            var user = await _db.GetRepository<AppUser>().GetAsync(x => x.Id == otpParts[1], true).FirstOrDefaultAsync();
            
            if (user == null)
                return (false, "Invalid Token");

            passwordList ??= [];
            passwordList?.Add(password);
            passwordList = passwordList?.Distinct().ToList();
            _memoryCache.Set(otpParts[1], passwordList);

            user.PasswordHash = BC.EnhancedHashPassword(password);

            

            _db.GetRepository<AppUser>().Update(user);

            if (await _db.SaveChangesAsync())
                return (true, "Valid");

            return (false, "Invalid Token");
        }

        return (false, "Invalid Token");
    }
}


