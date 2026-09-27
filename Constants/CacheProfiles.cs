
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Constants
{
    public static class CacheProfiles
    {
        public const string ProfileName10s = "CacheProfile10s";
        public const string ProfileName20s = "CacheProfile20s";

        public static readonly CacheProfile ProfileConf10s = new(){ Duration = 10 };
        public static readonly CacheProfile ProfileConf20s = new(){ Duration = 20 };
    }
}