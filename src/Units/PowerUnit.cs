#nullable enable
using System;

namespace CircuitTool.Units
{
    /// <summary>
    /// Represents power measurement units
    /// </summary>
    public enum PowerUnit
    {
        /// <summary>Nanowatts (nW)</summary>
        Nanowatts,
        /// <summary>Microwatts (μW)</summary>
        Microwatts,
        /// <summary>Milliwatts (mW)</summary>
        Milliwatts,
        /// <summary>Watts (W)</summary>
        Watts,
        /// <summary>Kilowatts (kW)</summary>
        Kilowatts,
        /// <summary>Megawatts (MW)</summary>
        Megawatts,
        /// <summary>Gigawatts (GW)</summary>
        Gigawatts
    }

    /// <summary>
    /// Represents a strongly-typed power measurement with automatic unit conversion
    /// </summary>
    public readonly struct Power : IEquatable<Power>, IComparable<Power>
    {
        private readonly double _watts;

        /// <summary>
        /// Creates a new power measurement
        /// </summary>
        /// <param name="value">The value in the specified unit</param>
        /// <param name="unit">The unit of measurement</param>
        public Power(double value, PowerUnit unit)
        {
            _watts = ConvertToWatts(value, unit);
        }

        /// <summary>
        /// Gets the power value in watts
        /// </summary>
        public double Watts => _watts;

        /// <summary>
        /// Gets the power value in milliwatts
        /// </summary>
        public double Milliwatts => _watts * 1000;

        /// <summary>
        /// Gets the power value in kilowatts
        /// </summary>
        public double Kilowatts => _watts / 1000;

        /// <summary>
        /// Gets the power value in megawatts
        /// </summary>
        public double Megawatts => _watts / 1_000_000;

        /// <summary>
        /// Gets the power value in gigawatts
        /// </summary>
        public double Gigawatts => _watts / 1_000_000_000;

        /// <summary>
        /// Gets the power value in microwatts
        /// </summary>
        public double Microwatts => _watts * 1_000_000;

        /// <summary>
        /// Gets the power value in nanowatts
        /// </summary>
        public double Nanowatts => _watts * 1_000_000_000;

        private static double ConvertToWatts(double value, PowerUnit unit)
        {
            return unit switch
            {
                PowerUnit.Nanowatts => value / 1_000_000_000,
                PowerUnit.Microwatts => value / 1_000_000,
                PowerUnit.Milliwatts => value / 1000,
                PowerUnit.Watts => value,
                PowerUnit.Kilowatts => value * 1000,
                PowerUnit.Megawatts => value * 1_000_000,
                PowerUnit.Gigawatts => value * 1_000_000_000,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }

        public double ConvertTo(PowerUnit unit)
        {
            return unit switch
            {
                PowerUnit.Nanowatts => Nanowatts,
                PowerUnit.Microwatts => Microwatts,
                PowerUnit.Milliwatts => Milliwatts,
                PowerUnit.Watts => Watts,
                PowerUnit.Kilowatts => Kilowatts,
                PowerUnit.Megawatts => Megawatts,
                PowerUnit.Gigawatts => Gigawatts,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }

        public static Power operator +(Power left, Power right) => new(left._watts + right._watts, PowerUnit.Watts);
        public static Power operator -(Power left, Power right) => new(left._watts - right._watts, PowerUnit.Watts);
        public static Power operator *(Power power, double scalar) => new(power._watts * scalar, PowerUnit.Watts);
        public static Power operator *(double scalar, Power power) => new(scalar * power._watts, PowerUnit.Watts);
        public static Power operator /(Power power, double scalar) => new(power._watts / scalar, PowerUnit.Watts);

        public static bool operator ==(Power left, Power right) => Math.Abs(left._watts - right._watts) < 1e-10;
        public static bool operator !=(Power left, Power right) => !(left == right);
        public static bool operator <(Power left, Power right) => left._watts < right._watts;
        public static bool operator >(Power left, Power right) => left._watts > right._watts;
        public static bool operator <=(Power left, Power right) => left._watts <= right._watts;
        public static bool operator >=(Power left, Power right) => left._watts >= right._watts;

        public bool Equals(Power other) => this == other;
        public override bool Equals(object? obj) => obj is Power other && Equals(other);
        public override int GetHashCode() => _watts.GetHashCode();
        public int CompareTo(Power other) => _watts.CompareTo(other._watts);

        public override string ToString() => $"{_watts:G} W";
        public string ToString(PowerUnit unit) => $"{ConvertTo(unit):G} {GetUnitSymbol(unit)}";

        private static string GetUnitSymbol(PowerUnit unit)
        {
            return unit switch
            {
                PowerUnit.Nanowatts => "nW",
                PowerUnit.Microwatts => "μW",
                PowerUnit.Milliwatts => "mW",
                PowerUnit.Watts => "W",
                PowerUnit.Kilowatts => "kW",
                PowerUnit.Megawatts => "MW",
                PowerUnit.Gigawatts => "GW",
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }
    }
}
