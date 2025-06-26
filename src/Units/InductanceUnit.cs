#nullable enable
using System;

namespace CircuitTool.Units
{
    /// <summary>
    /// Represents inductance measurement units
    /// </summary>
    public enum InductanceUnit
    {
        /// <summary>Nanohenries (nH)</summary>
        Nanohenries,
        /// <summary>Microhenries (μH)</summary>
        Microhenries,
        /// <summary>Millihenries (mH)</summary>
        Millihenries,
        /// <summary>Henries (H)</summary>
        Henries,
        /// <summary>Kilohenries (kH)</summary>
        Kilohenries
    }

    /// <summary>
    /// Represents a strongly-typed inductance measurement with automatic unit conversion
    /// </summary>
    public readonly struct Inductance : IEquatable<Inductance>, IComparable<Inductance>
    {
        private readonly double _henries;

        /// <summary>
        /// Creates a new inductance measurement
        /// </summary>
        /// <param name="value">The value in the specified unit</param>
        /// <param name="unit">The unit of measurement</param>
        public Inductance(double value, InductanceUnit unit)
        {
            _henries = ConvertToHenries(value, unit);
        }

        /// <summary>
        /// Gets the inductance value in henries
        /// </summary>
        public double Henries => _henries;

        /// <summary>
        /// Gets the inductance value in nanohenries
        /// </summary>
        public double Nanohenries => _henries * 1e9;

        /// <summary>
        /// Gets the inductance value in microhenries
        /// </summary>
        public double Microhenries => _henries * 1e6;

        /// <summary>
        /// Gets the inductance value in millihenries
        /// </summary>
        public double Millihenries => _henries * 1000;

        /// <summary>
        /// Gets the inductance value in kilohenries
        /// </summary>
        public double Kilohenries => _henries / 1000;

        private static double ConvertToHenries(double value, InductanceUnit unit)
        {
            return unit switch
            {
                InductanceUnit.Nanohenries => value / 1e9,
                InductanceUnit.Microhenries => value / 1e6,
                InductanceUnit.Millihenries => value / 1000,
                InductanceUnit.Henries => value,
                InductanceUnit.Kilohenries => value * 1000,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }

        public double ConvertTo(InductanceUnit unit)
        {
            return unit switch
            {
                InductanceUnit.Nanohenries => Nanohenries,
                InductanceUnit.Microhenries => Microhenries,
                InductanceUnit.Millihenries => Millihenries,
                InductanceUnit.Henries => Henries,
                InductanceUnit.Kilohenries => Kilohenries,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }

        public static Inductance operator +(Inductance left, Inductance right) => new(left._henries + right._henries, InductanceUnit.Henries);
        public static Inductance operator -(Inductance left, Inductance right) => new(left._henries - right._henries, InductanceUnit.Henries);
        public static Inductance operator *(Inductance inductance, double scalar) => new(inductance._henries * scalar, InductanceUnit.Henries);
        public static Inductance operator *(double scalar, Inductance inductance) => new(scalar * inductance._henries, InductanceUnit.Henries);
        public static Inductance operator /(Inductance inductance, double scalar) => new(inductance._henries / scalar, InductanceUnit.Henries);

        public static bool operator ==(Inductance left, Inductance right) => Math.Abs(left._henries - right._henries) < 1e-12;
        public static bool operator !=(Inductance left, Inductance right) => !(left == right);
        public static bool operator <(Inductance left, Inductance right) => left._henries < right._henries;
        public static bool operator >(Inductance left, Inductance right) => left._henries > right._henries;
        public static bool operator <=(Inductance left, Inductance right) => left._henries <= right._henries;
        public static bool operator >=(Inductance left, Inductance right) => left._henries >= right._henries;

        public bool Equals(Inductance other) => this == other;
        public override bool Equals(object? obj) => obj is Inductance other && Equals(other);
        public override int GetHashCode() => _henries.GetHashCode();
        public int CompareTo(Inductance other) => _henries.CompareTo(other._henries);

        public override string ToString() => $"{_henries:G} H";
        public string ToString(InductanceUnit unit) => $"{ConvertTo(unit):G} {GetUnitSymbol(unit)}";

        private static string GetUnitSymbol(InductanceUnit unit)
        {
            return unit switch
            {
                InductanceUnit.Nanohenries => "nH",
                InductanceUnit.Microhenries => "μH",
                InductanceUnit.Millihenries => "mH",
                InductanceUnit.Henries => "H",
                InductanceUnit.Kilohenries => "kH",
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }
    }
}
