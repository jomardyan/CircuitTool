#nullable enable
using System;

namespace CircuitTool.Units
{
    /// <summary>
    /// Represents capacitance measurement units
    /// </summary>
    public enum CapacitanceUnit
    {
        /// <summary>Picofarads (pF)</summary>
        Picofarads,
        /// <summary>Nanofarads (nF)</summary>
        Nanofarads,
        /// <summary>Microfarads (μF)</summary>
        Microfarads,
        /// <summary>Millifarads (mF)</summary>
        Millifarads,
        /// <summary>Farads (F)</summary>
        Farads,
        /// <summary>Kilofarads (kF)</summary>
        Kilofarads
    }

    /// <summary>
    /// Represents a strongly-typed capacitance measurement with automatic unit conversion
    /// </summary>
    public readonly struct Capacitance : IEquatable<Capacitance>, IComparable<Capacitance>
    {
        private readonly double _farads;

        /// <summary>
        /// Creates a new capacitance measurement
        /// </summary>
        /// <param name="value">The value in the specified unit</param>
        /// <param name="unit">The unit of measurement</param>
        public Capacitance(double value, CapacitanceUnit unit)
        {
            _farads = ConvertToFarads(value, unit);
        }

        /// <summary>
        /// Gets the capacitance value in farads
        /// </summary>
        public double Farads => _farads;

        /// <summary>
        /// Gets the capacitance value in picofarads
        /// </summary>
        public double Picofarads => _farads * 1e12;

        /// <summary>
        /// Gets the capacitance value in nanofarads
        /// </summary>
        public double Nanofarads => _farads * 1e9;

        /// <summary>
        /// Gets the capacitance value in microfarads
        /// </summary>
        public double Microfarads => _farads * 1e6;

        /// <summary>
        /// Gets the capacitance value in millifarads
        /// </summary>
        public double Millifarads => _farads * 1000;

        /// <summary>
        /// Gets the capacitance value in kilofarads
        /// </summary>
        public double Kilofarads => _farads / 1000;

        private static double ConvertToFarads(double value, CapacitanceUnit unit)
        {
            return unit switch
            {
                CapacitanceUnit.Picofarads => value / 1e12,
                CapacitanceUnit.Nanofarads => value / 1e9,
                CapacitanceUnit.Microfarads => value / 1e6,
                CapacitanceUnit.Millifarads => value / 1000,
                CapacitanceUnit.Farads => value,
                CapacitanceUnit.Kilofarads => value * 1000,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }

        public double ConvertTo(CapacitanceUnit unit)
        {
            return unit switch
            {
                CapacitanceUnit.Picofarads => Picofarads,
                CapacitanceUnit.Nanofarads => Nanofarads,
                CapacitanceUnit.Microfarads => Microfarads,
                CapacitanceUnit.Millifarads => Millifarads,
                CapacitanceUnit.Farads => Farads,
                CapacitanceUnit.Kilofarads => Kilofarads,
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }

        public static Capacitance operator +(Capacitance left, Capacitance right) => new(left._farads + right._farads, CapacitanceUnit.Farads);
        public static Capacitance operator -(Capacitance left, Capacitance right) => new(left._farads - right._farads, CapacitanceUnit.Farads);
        public static Capacitance operator *(Capacitance capacitance, double scalar) => new(capacitance._farads * scalar, CapacitanceUnit.Farads);
        public static Capacitance operator *(double scalar, Capacitance capacitance) => new(scalar * capacitance._farads, CapacitanceUnit.Farads);
        public static Capacitance operator /(Capacitance capacitance, double scalar) => new(capacitance._farads / scalar, CapacitanceUnit.Farads);

        public static bool operator ==(Capacitance left, Capacitance right) => Math.Abs(left._farads - right._farads) < 1e-15;
        public static bool operator !=(Capacitance left, Capacitance right) => !(left == right);
        public static bool operator <(Capacitance left, Capacitance right) => left._farads < right._farads;
        public static bool operator >(Capacitance left, Capacitance right) => left._farads > right._farads;
        public static bool operator <=(Capacitance left, Capacitance right) => left._farads <= right._farads;
        public static bool operator >=(Capacitance left, Capacitance right) => left._farads >= right._farads;

        public bool Equals(Capacitance other) => this == other;
        public override bool Equals(object? obj) => obj is Capacitance other && Equals(other);
        public override int GetHashCode() => _farads.GetHashCode();
        public int CompareTo(Capacitance other) => _farads.CompareTo(other._farads);

        public override string ToString() => $"{_farads:G} F";
        public string ToString(CapacitanceUnit unit) => $"{ConvertTo(unit):G} {GetUnitSymbol(unit)}";

        private static string GetUnitSymbol(CapacitanceUnit unit)
        {
            return unit switch
            {
                CapacitanceUnit.Picofarads => "pF",
                CapacitanceUnit.Nanofarads => "nF",
                CapacitanceUnit.Microfarads => "μF",
                CapacitanceUnit.Millifarads => "mF",
                CapacitanceUnit.Farads => "F",
                CapacitanceUnit.Kilofarads => "kF",
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }
    }
}
