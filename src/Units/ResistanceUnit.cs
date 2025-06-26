using System;

namespace CircuitTool.Units
{
    /// <summary>
    /// Represents resistance measurement units
    /// </summary>
    public enum ResistanceUnit
    {
        /// <summary>Milliohms (mΩ)</summary>
        Milliohms,
        /// <summary>Ohms (Ω)</summary>
        Ohms,
        /// <summary>Kiloohms (kΩ)</summary>
        Kiloohms,
        /// <summary>Megaohms (MΩ)</summary>
        Megaohms,
        /// <summary>Gigaohms (GΩ)</summary>
        Gigaohms
    }

    /// <summary>
    /// Represents a strongly-typed resistance measurement with automatic unit conversion
    /// </summary>
    public readonly struct Resistance : IEquatable<Resistance>, IComparable<Resistance>
    {
        private readonly double _ohms;

        /// <summary>
        /// Creates a new resistance measurement
        /// </summary>
        /// <param name="value">The value in the specified unit</param>
        /// <param name="unit">The unit of measurement</param>
        public Resistance(double value, ResistanceUnit unit)
        {
            if (value < 0)
                throw new ArgumentException("Resistance cannot be negative");
            
            _ohms = ConvertToOhms(value, unit);
        }

        /// <summary>
        /// Gets the resistance value in ohms
        /// </summary>
        public double Ohms => _ohms;

        /// <summary>
        /// Gets the resistance value in kiloohms
        /// </summary>
        public double Kiloohms => _ohms / 1000;

        /// <summary>
        /// Gets the resistance value in megaohms
        /// </summary>
        public double Megaohms => _ohms / 1e6;

        /// <summary>
        /// Gets the resistance value in the specified unit
        /// </summary>
        /// <param name="unit">The desired unit</param>
        /// <returns>The resistance value in the specified unit</returns>
        public double GetValue(ResistanceUnit unit)
        {
            return unit switch
            {
                ResistanceUnit.Milliohms => _ohms * 1000,
                ResistanceUnit.Ohms => _ohms,
                ResistanceUnit.Kiloohms => _ohms / 1000,
                ResistanceUnit.Megaohms => _ohms / 1e6,
                ResistanceUnit.Gigaohms => _ohms / 1e9,
                _ => throw new ArgumentException("Invalid resistance unit")
            };
        }

        private static double ConvertToOhms(double value, ResistanceUnit unit)
        {
            return unit switch
            {
                ResistanceUnit.Milliohms => value / 1000,
                ResistanceUnit.Ohms => value,
                ResistanceUnit.Kiloohms => value * 1000,
                ResistanceUnit.Megaohms => value * 1e6,
                ResistanceUnit.Gigaohms => value * 1e9,
                _ => throw new ArgumentException("Invalid resistance unit")
            };
        }

        // Operators
        public static Resistance operator +(Resistance left, Resistance right) => new(left._ohms + right._ohms, ResistanceUnit.Ohms);
        public static Resistance operator -(Resistance left, Resistance right) => new(Math.Max(0, left._ohms - right._ohms), ResistanceUnit.Ohms);
        public static Resistance operator *(Resistance resistance, double multiplier) => new(resistance._ohms * Math.Max(0, multiplier), ResistanceUnit.Ohms);
        public static Resistance operator /(Resistance resistance, double divisor) 
        {
            if (divisor <= 0) throw new ArgumentException("Divisor must be positive");
            return new(resistance._ohms / divisor, ResistanceUnit.Ohms);
        }

        // Parallel resistance calculation using & operator
        public static Resistance operator &(Resistance r1, Resistance r2) => new(1.0 / (1.0 / r1._ohms + 1.0 / r2._ohms), ResistanceUnit.Ohms);

        // Comparison operators
        public static bool operator ==(Resistance left, Resistance right) => Math.Abs(left._ohms - right._ohms) < 1e-10;
        public static bool operator !=(Resistance left, Resistance right) => !(left == right);
        public static bool operator <(Resistance left, Resistance right) => left._ohms < right._ohms;
        public static bool operator >(Resistance left, Resistance right) => left._ohms > right._ohms;
        public static bool operator <=(Resistance left, Resistance right) => left._ohms <= right._ohms;
        public static bool operator >=(Resistance left, Resistance right) => left._ohms >= right._ohms;

        // Implicit conversion from double (assumes ohms)
        public static implicit operator Resistance(double ohms) => new(ohms, ResistanceUnit.Ohms);
        public static implicit operator double(Resistance resistance) => resistance._ohms;

        public bool Equals(Resistance other) => this == other;
        public override bool Equals(object obj) => obj is Resistance resistance && Equals(resistance);
        public override int GetHashCode() => _ohms.GetHashCode();
        public int CompareTo(Resistance other) => _ohms.CompareTo(other._ohms);

        public override string ToString() => _ohms switch
        {
            >= 1e9 => $"{_ohms / 1e9:F3} GΩ",
            >= 1e6 => $"{_ohms / 1e6:F3} MΩ",
            >= 1000 => $"{_ohms / 1000:F3} kΩ",
            >= 1 => $"{_ohms:F3} Ω",
            _ => $"{_ohms * 1000:F3} mΩ"
        };

        public string ToString(ResistanceUnit unit) => $"{GetValue(unit):F3} {GetUnitSymbol(unit)}";

        private static string GetUnitSymbol(ResistanceUnit unit) => unit switch
        {
            ResistanceUnit.Milliohms => "mΩ",
            ResistanceUnit.Ohms => "Ω",
            ResistanceUnit.Kiloohms => "kΩ",
            ResistanceUnit.Megaohms => "MΩ",
            ResistanceUnit.Gigaohms => "GΩ",
            _ => "Ω"
        };
    }
}
