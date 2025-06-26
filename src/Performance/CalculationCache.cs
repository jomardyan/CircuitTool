#nullable enable
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CircuitTool.Performance
{
    /// <summary>
    /// Provides caching for expensive electrical calculations
    /// </summary>
    public static class CalculationCache
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new();
        private static readonly object _lockObject = new();
        private static int _maxCacheSize = 1000;

        /// <summary>
        /// Gets or sets the maximum cache size
        /// </summary>
        public static int MaxCacheSize
        {
            get => _maxCacheSize;
            set
            {
                _maxCacheSize = value;
                TrimCache();
            }
        }

        /// <summary>
        /// Gets a cached result or computes and caches it
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="factory">Function to compute the result if not cached</param>
        /// <returns>Cached or computed result</returns>
        public static T GetOrCompute<T>(string key, Func<T> factory)
        {
            if (_cache.TryGetValue(key, out var cachedValue) && cachedValue is T result)
            {
                return result;
            }

            var computedValue = factory();
            
            lock (_lockObject)
            {
                _cache.TryAdd(key, computedValue!);
                TrimCache();
            }

            return computedValue;
        }

        /// <summary>
        /// Creates a cache key from multiple parameters
        /// </summary>
        /// <param name="parameters">Parameters to include in the key</param>
        /// <returns>Cache key string</returns>
        public static string CreateKey(params object[] parameters)
        {
            return string.Join("|", parameters.Select(p => p?.ToString() ?? "null"));
        }

        /// <summary>
        /// Clears the entire cache
        /// </summary>
        public static void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Removes old entries when cache size exceeds limit
        /// </summary>
        private static void TrimCache()
        {
            if (_cache.Count <= _maxCacheSize) return;

            // Simple LRU-like trimming - remove 25% of entries
            var entriesToRemove = _cache.Count / 4;
            var keysToRemove = _cache.Keys.Take(entriesToRemove).ToList();

            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }
        }
    }
}
