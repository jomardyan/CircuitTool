#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace CircuitTool.Performance
{
    /// <summary>
    /// Performance monitoring and optimization utilities
    /// </summary>
    public static class PerformanceMonitor
    {
        private static readonly ConcurrentDictionary<string, (long count, long totalMs)> _metrics = new();

        /// <summary>
        /// Measures and records execution time for a function
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="operationName">Name of the operation for tracking</param>
        /// <param name="operation">Function to execute and measure</param>
        /// <returns>Result of the operation</returns>
        public static T MeasureOperation<T>(string operationName, Func<T> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = operation();
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _metrics.AddOrUpdate(operationName,
                (1, elapsedMs),
                (key, value) => (value.count + 1, value.totalMs + elapsedMs));

            return result;
        }

        /// <summary>
        /// Gets performance statistics for an operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Performance statistics</returns>
        public static (long callCount, double averageMs, long totalMs)? GetStats(string operationName)
        {
            if (_metrics.TryGetValue(operationName, out var stats))
            {
                return (stats.count, (double)stats.totalMs / stats.count, stats.totalMs);
            }
            return null;
        }

        /// <summary>
        /// Gets all performance statistics
        /// </summary>
        /// <returns>Dictionary of all performance metrics</returns>
        public static Dictionary<string, (long callCount, double averageMs, long totalMs)> GetAllStats()
        {
            var result = new Dictionary<string, (long callCount, double averageMs, long totalMs)>();
            
            foreach (var kvp in _metrics)
            {
                var stats = kvp.Value;
                result[kvp.Key] = (stats.count, (double)stats.totalMs / stats.count, stats.totalMs);
            }

            return result;
        }

        /// <summary>
        /// Clears all performance metrics
        /// </summary>
        public static void ClearStats()
        {
            _metrics.Clear();
        }
    }
}
