#nullable enable
using System;

namespace CircuitTool.Units
{
    /// <summary>
    /// Represents current measurement units
    /// </summary>
    public enum CurrentUnit
    {
        /// <summary>Nanoamperes (nA)</summary>
        Nanoamperes,
        /// <summary>Microamperes (μA)</summary>
        Microamperes,
        /// <summary>Milliamperes (mA)</summary>
        Milliamperes,
        /// <summary>Amperes (A)</summary>
        Amperes,
        /// <summary>Kiloamperes (kA)</summary>
        Kiloamperes
    }

    /// <summary>
    /// Represents a strongly-typed current measurement with automatic unit conversion
    /// </summary>
    public readonly struct Current : IEquatable<Current>, IComparable<Current>
    {
        private readonly double _amperes;

        /// <summary>
        /// Creates a new current measurement
        /// </summary>
        /// <param name="value">The value in the specified unit</param>
        /// <param name="unit">The unit of measurement</param>
        public Current(double value, CurrentUnit unit)
        {
            _amperes = ConvertToAmperes(value, unit);
        }

        /// <summary>
        /// Gets the current value in amperes
        /// </summary>
        public double Amperes => _amperes;

        /// <summary>
        /// Gets the current value in milliamperes
        /// </summary>
        public double Milliamperes => _amperes * 1000;

        /// <summary>
        /// Gets the current value in microamperes
        /// </summary>
        public double Microamperes => _amperes * 1e6;

        /// <summary>
        /// Gets the current value in the specified unit
        /// </summary>
        /// <param name="unit">The desired unit</param>
        /// <returns>The current value in the specified unit</returns>
        public double GetValue(CurrentUnit unit)
        {
            return unit switch
            {
                CurrentUnit.Nanoamperes => _amperes * 1e9,
                CurrentUnit.Microamperes => _amperes * 1e6,
                CurrentUnit.Milliamperes => _amperes * 1000,
                CurrentUnit.Amperes => _amperes,
                CurrentUnit.Kiloamperes => _amperes / 1000,
                _ => throw new ArgumentException("Invalid current unit")
            };
        }

        private static double ConvertToAmperes(double value, CurrentUnit unit)
        {
            return unit switch
            {
                CurrentUnit.Nanoamperes => value / 1e9,
                CurrentUnit.Microamperes => value / 1e6,
                CurrentUnit.Milliamperes => value / 1000,
                CurrentUnit.Amperes => value,
                CurrentUnit.Kiloamperes => value * 1000,
                _ => throw new ArgumentException("Invalid current unit")
            };
        }

        // Operators
        public static Current operator +(Current left, Current right) => new(left._amperes + right._amperes, CurrentUnit.Amperes);
        public static Current operator -(Current left, Current right) => new(left._amperes - right._amperes, CurrentUnit.Amperes);
        public static Current operator *(Current current, double multiplier) => new(current._amperes * multiplier, CurrentUnit.Amperes);
        public static Current operator /(Current current, double divisor) => new(current._amperes / divisor, CurrentUnit.Amperes);

        // Comparison operators
        public static bool operator ==(Current left, Current right) => Math.Abs(left._amperes - right._amperes) < 1e-10;
        public static bool operator !=(Current left, Current right) => !(left == right);
        public static bool operator <(Current left, Current right) => left._amperes < right._amperes;
        public static bool operator >(Current left, Current right) => left._amperes > right._amperes;
        public static bool operator <=(Current left, Current right) => left._amperes <= right._amperes;
        public static bool operator >=(Current left, Current right) => left._amperes >= right._amperes;

        // Implicit conversion from double (assumes amperes)
        public static implicit operator Current(double amperes) => new(amperes, CurrentUnit.Amperes);
        public static implicit operator double(Current current) => current._amperes;

        public bool Equals(Current other) => this == other;
        public override bool Equals(object? obj) => obj is Current current && Equals(current);
        public override int GetHashCode() => _amperes.GetHashCode();
        public int CompareTo(Current other) => _amperes.CompareTo(other._amperes);

        public override string ToString() => $"{_amperes:F6} A";
        public string ToString(CurrentUnit unit) => $"{GetValue(unit):F3} {GetUnitSymbol(unit)}";

        private static string GetUnitSymbol(CurrentUnit unit) => unit switch
        {
            CurrentUnit.Nanoamperes => "nA",
            CurrentUnit.Microamperes => "μA",
            CurrentUnit.Milliamperes => "mA",
            CurrentUnit.Amperes => "A",
            CurrentUnit.Kiloamperes => "kA",
            _ => "A"
        };
    }
}
