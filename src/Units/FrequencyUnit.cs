#nullable enable
using System;

namespace CircuitTool.Units
{
    /// <summary>
    /// Represents frequency measurement units
    /// </summary>
    public enum FrequencyUnit
    {
        /// <summary>Hertz (Hz)</summary>
        Hertz,
        /// <summary>Kilohertz (kHz)</summary>
        Kilohertz,
        /// <summary>Megahertz (MHz)</summary>
        Megahertz,
        /// <summary>Gigahertz (GHz)</summary>
        Gigahertz,
        /// <summary>Terahertz (THz)</summary>
        Terahertz
    }

    /// <summary>
    /// Represents a strongly-typed frequency measurement with automatic unit conversion
    /// </summary>
    public readonly struct Frequency : IEquatable<Frequency>, IComparable<Frequency>
    {
        private readonly double _hertz;

        /// <summary>
        /// Creates a new frequency measurement
        /// </summary>
        /// <param name="value">The value in the specified unit</param>
        /// <param name="unit">The unit of measurement</param>
        public Frequency(double value, FrequencyUnit unit)
        {
            _hertz = ConvertToHertz(value, unit);
        }

        /// <summary>
        /// Gets the frequency value in hertz
        /// </summary>
        public double Hertz => _hertz;

        /// <summary>
        /// Gets the frequency value in kilohertz
        /// </summary>
        public double Kilohertz => _hertz / 1000;

        /// <summary>
        /// Gets the frequency value in megahertz
        /// </summary>
        public double Megahertz => _hertz / 1_000_000;

        /// <summary>
        /// Gets the frequency value in gigahertz
        /// </summary>
        public double Gigahertz => _hertz / 1_000_000_000;

        /// <summary>
        /// Gets the frequency value in terahertz
        /// </summary>
        public double Terahertz => _hertz / 1_000_000_000_000;

        /// <summary>
        /// Gets the angular frequency (ω = 2πf) in radians per second
        /// </summary>
        public double AngularFrequency => 2 * Math.PI * _hertz;

        /// <summary>
        /// Gets the period (T = 1/f) in seconds
        /// </summary>
        public double Period => 1.0 / _hertz;

        private static double ConvertToHertz(double value, FrequencyUnit unit)
        {
            return unit switch
            {
                FrequencyUnit.Hertz => value,
                FrequencyUnit.Kilohertz => value * 1000,
                FrequencyUnit.Megahertz => value * 1_000_000,
                FrequencyUnit.Gigahertz => value * 1_000_000_000,
                FrequencyUnit.Terahertz => value * 1_000_000_000_000,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }

        public double ConvertTo(FrequencyUnit unit)
        {
            return unit switch
            {
                FrequencyUnit.Hertz => Hertz,
                FrequencyUnit.Kilohertz => Kilohertz,
                FrequencyUnit.Megahertz => Megahertz,
                FrequencyUnit.Gigahertz => Gigahertz,
                FrequencyUnit.Terahertz => Terahertz,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }

        public static Frequency operator +(Frequency left, Frequency right) => new(left._hertz + right._hertz, FrequencyUnit.Hertz);
        public static Frequency operator -(Frequency left, Frequency right) => new(left._hertz - right._hertz, FrequencyUnit.Hertz);
        public static Frequency operator *(Frequency frequency, double scalar) => new(frequency._hertz * scalar, FrequencyUnit.Hertz);
        public static Frequency operator *(double scalar, Frequency frequency) => new(scalar * frequency._hertz, FrequencyUnit.Hertz);
        public static Frequency operator /(Frequency frequency, double scalar) => new(frequency._hertz / scalar, FrequencyUnit.Hertz);

        public static bool operator ==(Frequency left, Frequency right) => Math.Abs(left._hertz - right._hertz) < 1e-6;
        public static bool operator !=(Frequency left, Frequency right) => !(left == right);
        public static bool operator <(Frequency left, Frequency right) => left._hertz < right._hertz;
        public static bool operator >(Frequency left, Frequency right) => left._hertz > right._hertz;
        public static bool operator <=(Frequency left, Frequency right) => left._hertz <= right._hertz;
        public static bool operator >=(Frequency left, Frequency right) => left._hertz >= right._hertz;

        public bool Equals(Frequency other) => this == other;
        public override bool Equals(object? obj) => obj is Frequency other && Equals(other);
        public override int GetHashCode() => _hertz.GetHashCode();
        public int CompareTo(Frequency other) => _hertz.CompareTo(other._hertz);

        public override string ToString() => $"{_hertz:G} Hz";
        public string ToString(FrequencyUnit unit) => $"{ConvertTo(unit):G} {GetUnitSymbol(unit)}";

        private static string GetUnitSymbol(FrequencyUnit unit)
        {
            return unit switch
            {
                FrequencyUnit.Hertz => "Hz",
                FrequencyUnit.Kilohertz => "kHz",
                FrequencyUnit.Megahertz => "MHz",
                FrequencyUnit.Gigahertz => "GHz",
                FrequencyUnit.Terahertz => "THz",
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }
    }
}
