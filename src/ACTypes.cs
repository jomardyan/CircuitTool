#nullable enable
using System;
using System.Numerics;
using CircuitTool.Units;

namespace CircuitTool
{
    /// <summary>
    /// Represents an AC voltage with magnitude and phase
    /// </summary>
    public readonly struct ACVoltage : IEquatable<ACVoltage>
    {
        private readonly Complex _complex;

        /// <summary>
        /// Creates a new AC voltage from magnitude and phase
        /// </summary>
        /// <param name="magnitude">RMS magnitude in volts</param>
        /// <param name="phase">Phase angle in radians</param>
        public ACVoltage(double magnitude, double phase)
        {
            _complex = Complex.FromPolarCoordinates(magnitude, phase);
        }

        /// <summary>
        /// Creates a new AC voltage from a complex number
        /// </summary>
        /// <param name="complex">Complex representation</param>
        public ACVoltage(Complex complex)
        {
            _complex = complex;
        }

        /// <summary>
        /// Gets the RMS magnitude in volts
        /// </summary>
        public double Magnitude => _complex.Magnitude;

        /// <summary>
        /// Gets the phase angle in radians
        /// </summary>
        public double Phase => _complex.Phase;

        /// <summary>
        /// Gets the phase angle in degrees
        /// </summary>
        public double PhaseDegrees => _complex.Phase * 180.0 / Math.PI;

        /// <summary>
        /// Gets the real component
        /// </summary>
        public double Real => _complex.Real;

        /// <summary>
        /// Gets the imaginary component
        /// </summary>
        public double Imaginary => _complex.Imaginary;

        /// <summary>
        /// Gets the complex number representation
        /// </summary>
        public Complex Complex => _complex;

        /// <summary>
        /// Gets the peak voltage (magnitude * √2)
        /// </summary>
        public double Peak => Magnitude * Math.Sqrt(2);

        /// <summary>
        /// Gets the peak-to-peak voltage (peak * 2)
        /// </summary>
        public double PeakToPeak => Peak * 2;

        public static ACVoltage operator +(ACVoltage left, ACVoltage right) => new(left._complex + right._complex);
        public static ACVoltage operator -(ACVoltage left, ACVoltage right) => new(left._complex - right._complex);
        public static ACVoltage operator *(ACVoltage voltage, Complex scalar) => new(voltage._complex * scalar);
        public static ACVoltage operator *(Complex scalar, ACVoltage voltage) => new(scalar * voltage._complex);
        public static ACVoltage operator /(ACVoltage voltage, Complex scalar) => new(voltage._complex / scalar);

        public static bool operator ==(ACVoltage left, ACVoltage right) => left._complex == right._complex;
        public static bool operator !=(ACVoltage left, ACVoltage right) => !(left == right);

        public bool Equals(ACVoltage other) => this == other;
        public override bool Equals(object? obj) => obj is ACVoltage other && Equals(other);
        public override int GetHashCode() => _complex.GetHashCode();

        public override string ToString() => $"{Magnitude:F3} V ∠ {PhaseDegrees:F1}°";
        public string ToString(string format) => $"{Magnitude.ToString(format)} V ∠ {PhaseDegrees.ToString(format)}°";
    }

    /// <summary>
    /// Represents an AC current with magnitude and phase
    /// </summary>
    public readonly struct ACCurrent : IEquatable<ACCurrent>
    {
        private readonly Complex _complex;

        /// <summary>
        /// Creates a new AC current from magnitude and phase
        /// </summary>
        /// <param name="magnitude">RMS magnitude in amperes</param>
        /// <param name="phase">Phase angle in radians</param>
        public ACCurrent(double magnitude, double phase)
        {
            _complex = Complex.FromPolarCoordinates(magnitude, phase);
        }

        /// <summary>
        /// Creates a new AC current from a complex number
        /// </summary>
        /// <param name="complex">Complex representation</param>
        public ACCurrent(Complex complex)
        {
            _complex = complex;
        }

        /// <summary>
        /// Gets the RMS magnitude in amperes
        /// </summary>
        public double Magnitude => _complex.Magnitude;

        /// <summary>
        /// Gets the phase angle in radians
        /// </summary>
        public double Phase => _complex.Phase;

        /// <summary>
        /// Gets the phase angle in degrees
        /// </summary>
        public double PhaseDegrees => _complex.Phase * 180.0 / Math.PI;

        /// <summary>
        /// Gets the real component
        /// </summary>
        public double Real => _complex.Real;

        /// <summary>
        /// Gets the imaginary component
        /// </summary>
        public double Imaginary => _complex.Imaginary;

        /// <summary>
        /// Gets the complex number representation
        /// </summary>
        public Complex Complex => _complex;

        /// <summary>
        /// Gets the peak current (magnitude * √2)
        /// </summary>
        public double Peak => Magnitude * Math.Sqrt(2);

        public static ACCurrent operator +(ACCurrent left, ACCurrent right) => new(left._complex + right._complex);
        public static ACCurrent operator -(ACCurrent left, ACCurrent right) => new(left._complex - right._complex);
        public static ACCurrent operator *(ACCurrent current, Complex scalar) => new(current._complex * scalar);
        public static ACCurrent operator *(Complex scalar, ACCurrent current) => new(scalar * current._complex);
        public static ACCurrent operator /(ACCurrent current, Complex scalar) => new(current._complex / scalar);

        public static bool operator ==(ACCurrent left, ACCurrent right) => left._complex == right._complex;
        public static bool operator !=(ACCurrent left, ACCurrent right) => !(left == right);

        public bool Equals(ACCurrent other) => this == other;
        public override bool Equals(object? obj) => obj is ACCurrent other && Equals(other);
        public override int GetHashCode() => _complex.GetHashCode();

        public override string ToString() => $"{Magnitude:F3} A ∠ {PhaseDegrees:F1}°";
        public string ToString(string format) => $"{Magnitude.ToString(format)} A ∠ {PhaseDegrees.ToString(format)}°";
    }

    /// <summary>
    /// Represents complex impedance for AC circuit analysis
    /// </summary>
    public readonly struct Impedance : IEquatable<Impedance>
    {
        private readonly Complex _complex;

        /// <summary>
        /// Creates a new impedance from magnitude and phase
        /// </summary>
        /// <param name="magnitude">Magnitude in ohms</param>
        /// <param name="phase">Phase angle in radians</param>
        public static Impedance FromPolar(double magnitude, double phase)
        {
            return new Impedance(Complex.FromPolarCoordinates(magnitude, phase));
        }

        /// <summary>
        /// Creates a new impedance from resistance and reactance
        /// </summary>
        /// <param name="resistance">Resistance in ohms</param>
        /// <param name="reactance">Reactance in ohms</param>
        public Impedance(double resistance, double reactance)
        {
            _complex = new Complex(resistance, reactance);
        }

        /// <summary>
        /// Creates a new impedance from a complex number
        /// </summary>
        /// <param name="complex">Complex representation</param>
        public Impedance(Complex complex)
        {
            _complex = complex;
        }

        /// <summary>
        /// Gets the magnitude in ohms
        /// </summary>
        public double Magnitude => _complex.Magnitude;

        /// <summary>
        /// Gets the phase angle in radians
        /// </summary>
        public double Phase => _complex.Phase;

        /// <summary>
        /// Gets the phase angle in degrees
        /// </summary>
        public double PhaseDegrees => _complex.Phase * 180.0 / Math.PI;

        /// <summary>
        /// Gets the resistance component in ohms
        /// </summary>
        public double Resistance => _complex.Real;

        /// <summary>
        /// Gets the reactance component in ohms
        /// </summary>
        public double Reactance => _complex.Imaginary;

        /// <summary>
        /// Gets the complex number representation
        /// </summary>
        public Complex Complex => _complex;

        /// <summary>
        /// Gets the admittance (1/Z)
        /// </summary>
        public Admittance Admittance => new(1.0 / _complex);

        /// <summary>
        /// Creates impedance for a resistor
        /// </summary>
        /// <param name="resistance">Resistance in ohms</param>
        public static Impedance Resistor(double resistance) => new(resistance, 0);

        /// <summary>
        /// Creates impedance for a capacitor at given frequency
        /// </summary>
        /// <param name="capacitance">Capacitance in farads</param>
        /// <param name="frequency">Frequency in hertz</param>
        public static Impedance Capacitor(double capacitance, double frequency)
        {
            var reactance = -1.0 / (2 * Math.PI * frequency * capacitance);
            return new Impedance(0, reactance);
        }

        /// <summary>
        /// Creates impedance for an inductor at given frequency
        /// </summary>
        /// <param name="inductance">Inductance in henries</param>
        /// <param name="frequency">Frequency in hertz</param>
        public static Impedance Inductor(double inductance, double frequency)
        {
            var reactance = 2 * Math.PI * frequency * inductance;
            return new Impedance(0, reactance);
        }

        public static Impedance operator +(Impedance left, Impedance right) => new(left._complex + right._complex);
        public static Impedance operator -(Impedance left, Impedance right) => new(left._complex - right._complex);
        public static Impedance operator *(Impedance impedance, double scalar) => new(impedance._complex * scalar);
        public static Impedance operator *(double scalar, Impedance impedance) => new(scalar * impedance._complex);
        public static Impedance operator /(Impedance impedance, double scalar) => new(impedance._complex / scalar);

        /// <summary>
        /// Parallel combination of impedances
        /// </summary>
        public static Impedance Parallel(Impedance z1, Impedance z2) => new(1.0 / (1.0 / z1._complex + 1.0 / z2._complex));

        /// <summary>
        /// Series combination of impedances
        /// </summary>
        public static Impedance Series(Impedance z1, Impedance z2) => z1 + z2;

        public static bool operator ==(Impedance left, Impedance right) => left._complex == right._complex;
        public static bool operator !=(Impedance left, Impedance right) => !(left == right);

        public bool Equals(Impedance other) => this == other;
        public override bool Equals(object? obj) => obj is Impedance other && Equals(other);
        public override int GetHashCode() => _complex.GetHashCode();

        public override string ToString() => $"{Magnitude:F3} Ω ∠ {PhaseDegrees:F1}°";
        public string ToString(string format) => $"{Magnitude.ToString(format)} Ω ∠ {PhaseDegrees.ToString(format)}°";
    }

    /// <summary>
    /// Represents complex admittance for AC circuit analysis
    /// </summary>
    public readonly struct Admittance : IEquatable<Admittance>
    {
        private readonly Complex _complex;

        /// <summary>
        /// Creates a new admittance from a complex number
        /// </summary>
        /// <param name="complex">Complex representation</param>
        public Admittance(Complex complex)
        {
            _complex = complex;
        }

        /// <summary>
        /// Gets the magnitude in siemens
        /// </summary>
        public double Magnitude => _complex.Magnitude;

        /// <summary>
        /// Gets the conductance component in siemens
        /// </summary>
        public double Conductance => _complex.Real;

        /// <summary>
        /// Gets the susceptance component in siemens
        /// </summary>
        public double Susceptance => _complex.Imaginary;

        /// <summary>
        /// Gets the complex number representation
        /// </summary>
        public Complex Complex => _complex;

        /// <summary>
        /// Gets the impedance (1/Y)
        /// </summary>
        public Impedance Impedance => new(1.0 / _complex);

        public static Admittance operator +(Admittance left, Admittance right) => new(left._complex + right._complex);
        public static Admittance operator -(Admittance left, Admittance right) => new(left._complex - right._complex);

        public static bool operator ==(Admittance left, Admittance right) => left._complex == right._complex;
        public static bool operator !=(Admittance left, Admittance right) => !(left == right);

        public bool Equals(Admittance other) => this == other;
        public override bool Equals(object? obj) => obj is Admittance other && Equals(other);
        public override int GetHashCode() => _complex.GetHashCode();

        public override string ToString() => $"{Magnitude:F6} S";
    }
}
