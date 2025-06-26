#nullable enable
using System;
using System.Collections.Generic;
using System.Numerics;
using CircuitTool.Units;

namespace CircuitTool.CircuitBuilder
{
    /// <summary>
    /// Represents a circuit component
    /// </summary>
    public abstract class Component
    {
        public string Id { get; }
        public string Name { get; set; }

        protected Component(string id, string name)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the impedance of this component at the specified frequency
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        /// <returns>Complex impedance</returns>
        public abstract Impedance GetImpedance(double frequency);

        /// <summary>
        /// Gets component tolerance for Monte Carlo analysis
        /// </summary>
        public virtual double Tolerance { get; set; } = 0.05; // 5% default
    }

    /// <summary>
    /// Represents a resistor component
    /// </summary>
    public class Resistor : Component
    {
        public double Resistance { get; }

        public Resistor(string id, double resistance, string name = "") 
            : base(id, string.IsNullOrEmpty(name) ? $"R{id}" : name)
        {
            if (resistance <= 0) throw new ArgumentOutOfRangeException(nameof(resistance));
            Resistance = resistance;
        }

        public override Impedance GetImpedance(double frequency) => Impedance.Resistor(Resistance);

        public override string ToString() => $"{Name}: {Resistance:G} Ω";
    }

    /// <summary>
    /// Represents a capacitor component
    /// </summary>
    public class Capacitor : Component
    {
        public double Capacitance { get; }

        public Capacitor(string id, double capacitance, string name = "") 
            : base(id, string.IsNullOrEmpty(name) ? $"C{id}" : name)
        {
            if (capacitance <= 0) throw new ArgumentOutOfRangeException(nameof(capacitance));
            Capacitance = capacitance;
        }

        public override Impedance GetImpedance(double frequency) => Impedance.Capacitor(Capacitance, frequency);

        public override string ToString() => $"{Name}: {Capacitance:G} F";
    }

    /// <summary>
    /// Represents an inductor component
    /// </summary>
    public class Inductor : Component
    {
        public double Inductance { get; }

        public Inductor(string id, double inductance, string name = "") 
            : base(id, string.IsNullOrEmpty(name) ? $"L{id}" : name)
        {
            if (inductance <= 0) throw new ArgumentOutOfRangeException(nameof(inductance));
            Inductance = inductance;
        }

        public override Impedance GetImpedance(double frequency) => Impedance.Inductor(Inductance, frequency);

        public override string ToString() => $"{Name}: {Inductance:G} H";
    }

    /// <summary>
    /// Represents how components are connected
    /// </summary>
    public enum ConnectionType
    {
        Series,
        Parallel
    }

    /// <summary>
    /// Represents a connection between components
    /// </summary>
    public class ComponentConnection
    {
        public Component Component { get; }
        public ConnectionType ConnectionType { get; }

        public ComponentConnection(Component component, ConnectionType connectionType)
        {
            Component = component ?? throw new ArgumentNullException(nameof(component));
            ConnectionType = connectionType;
        }
    }

    /// <summary>
    /// Fluent API for building circuits
    /// </summary>
    public class CircuitBuilder
    {
        private readonly List<ComponentConnection> _components = new();
        private int _nextId = 1;

        /// <summary>
        /// Starts building a new circuit
        /// </summary>
        public static CircuitBuilder New() => new();

        /// <summary>
        /// Adds a resistor to the circuit
        /// </summary>
        /// <param name="resistance">Resistance in ohms</param>
        /// <param name="name">Optional component name</param>
        public CircuitBuilder AddResistor(double resistance, string? name = null)
        {
            var resistor = new Resistor(_nextId++.ToString(), resistance, name ?? "");
            _components.Add(new ComponentConnection(resistor, _components.Count == 0 ? ConnectionType.Series : ConnectionType.Series));
            return this;
        }

        /// <summary>
        /// Adds a capacitor to the circuit
        /// </summary>
        /// <param name="capacitance">Capacitance in farads</param>
        /// <param name="name">Optional component name</param>
        public CircuitBuilder AddCapacitor(double capacitance, string? name = null)
        {
            var capacitor = new Capacitor(_nextId++.ToString(), capacitance, name ?? "");
            _components.Add(new ComponentConnection(capacitor, _components.Count == 0 ? ConnectionType.Series : ConnectionType.Series));
            return this;
        }

        /// <summary>
        /// Adds an inductor to the circuit
        /// </summary>
        /// <param name="inductance">Inductance in henries</param>
        /// <param name="name">Optional component name</param>
        public CircuitBuilder AddInductor(double inductance, string? name = null)
        {
            var inductor = new Inductor(_nextId++.ToString(), inductance, name ?? "");
            _components.Add(new ComponentConnection(inductor, _components.Count == 0 ? ConnectionType.Series : ConnectionType.Series));
            return this;
        }

        /// <summary>
        /// Sets the last added component to be in series with the next component
        /// </summary>
        public CircuitBuilder InSeriesWith()
        {
            if (_components.Count > 0)
            {
                var lastComponent = _components[_components.Count - 1];
                _components[_components.Count - 1] = new ComponentConnection(lastComponent.Component, ConnectionType.Series);
            }
            return this;
        }

        /// <summary>
        /// Sets the last added component to be in parallel with the next component
        /// </summary>
        public CircuitBuilder InParallelWith()
        {
            if (_components.Count > 0)
            {
                var lastComponent = _components[_components.Count - 1];
                _components[_components.Count - 1] = new ComponentConnection(lastComponent.Component, ConnectionType.Parallel);
            }
            return this;
        }

        /// <summary>
        /// Sets component tolerance for all components
        /// </summary>
        /// <param name="tolerance">Tolerance as a fraction (e.g., 0.05 for 5%)</param>
        public CircuitBuilder WithTolerance(double tolerance)
        {
            foreach (var connection in _components)
            {
                connection.Component.Tolerance = tolerance;
            }
            return this;
        }

        /// <summary>
        /// Sets tolerance for the last added component
        /// </summary>
        /// <param name="tolerance">Tolerance as a fraction (e.g., 0.05 for 5%)</param>
        public CircuitBuilder WithComponentTolerance(double tolerance)
        {
            if (_components.Count > 0)
            {
                _components[_components.Count - 1].Component.Tolerance = tolerance;
            }
            return this;
        }

        /// <summary>
        /// Builds the circuit and returns the total impedance calculation
        /// </summary>
        public Circuit Build() => new(_components);
    }

    /// <summary>
    /// Represents a complete circuit with analysis capabilities
    /// </summary>
    public class Circuit
    {
        private readonly List<ComponentConnection> _components;

        internal Circuit(List<ComponentConnection> components)
        {
            _components = new List<ComponentConnection>(components);
            if (_components.Count == 0)
                throw new InvalidOperationException("Circuit must have at least one component");
        }

        /// <summary>
        /// Gets all components in the circuit
        /// </summary>
        public IReadOnlyList<Component> Components => _components.ConvertAll(c => c.Component);

        /// <summary>
        /// Calculates the total impedance of the circuit at the specified frequency
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        /// <returns>Total circuit impedance</returns>
        public Impedance CalculateImpedance(double frequency)
        {
            if (_components.Count == 0)
                throw new InvalidOperationException("Circuit has no components");

            var result = _components[0].Component.GetImpedance(frequency);

            for (int i = 1; i < _components.Count; i++)
            {
                var component = _components[i];
                var impedance = component.Component.GetImpedance(frequency);

                result = component.ConnectionType switch
                {
                    ConnectionType.Series => Impedance.Series(result, impedance),
                    ConnectionType.Parallel => Impedance.Parallel(result, impedance),
                    _ => throw new InvalidOperationException($"Unknown connection type: {component.ConnectionType}")
                };
            }

            return result;
        }

        /// <summary>
        /// Calculates circuit response for a given input voltage
        /// </summary>
        /// <param name="inputVoltage">Input voltage (AC)</param>
        /// <param name="frequency">Frequency in Hz</param>
        /// <returns>Circuit response</returns>
        public CircuitResponse AnalyzeResponse(ACVoltage inputVoltage, double frequency)
        {
            var totalImpedance = CalculateImpedance(frequency);
            var current = new ACCurrent(inputVoltage.Complex / totalImpedance.Complex);
            var power = CalculatePower(inputVoltage, current);

            return new CircuitResponse(inputVoltage, current, totalImpedance, power, frequency);
        }

        /// <summary>
        /// Performs frequency sweep analysis
        /// </summary>
        /// <param name="startFreq">Start frequency in Hz</param>
        /// <param name="stopFreq">Stop frequency in Hz</param>
        /// <param name="points">Number of points</param>
        /// <returns>Frequency response</returns>
        public FrequencyResponse GetFrequencyResponse(double startFreq, double stopFreq, int points)
        {
            var frequencies = new double[points];
            var impedances = new Complex[points];

            for (int i = 0; i < points; i++)
            {
                var frequency = startFreq * Math.Pow(stopFreq / startFreq, (double)i / (points - 1));
                frequencies[i] = frequency;
                impedances[i] = CalculateImpedance(frequency).Complex;
            }

            return new FrequencyResponse(frequencies, impedances);
        }

        /// <summary>
        /// Calculates resonant frequency for RLC circuits
        /// </summary>
        /// <returns>Resonant frequency in Hz, or null if not applicable</returns>
        public double? CalculateResonantFrequency()
        {
            var inductors = _components.Where(c => c.Component is Inductor).ToArray();
            var capacitors = _components.Where(c => c.Component is Capacitor).ToArray();

            if (inductors.Length == 1 && capacitors.Length == 1)
            {
                var L = ((Inductor)inductors[0].Component).Inductance;
                var C = ((Capacitor)capacitors[0].Component).Capacitance;
                return 1.0 / (2 * Math.PI * Math.Sqrt(L * C));
            }

            return null;
        }

        /// <summary>
        /// Gets component nominal values for Monte Carlo analysis
        /// </summary>
        public double[] GetNominalValues()
        {
            return _components.Select(c => c.Component switch
            {
                Resistor r => r.Resistance,
                Capacitor cap => cap.Capacitance,
                Inductor ind => ind.Inductance,
                _ => 0.0
            }).ToArray();
        }

        /// <summary>
        /// Gets component tolerances for Monte Carlo analysis
        /// </summary>
        public double[] GetTolerances()
        {
            return _components.Select(c => c.Component.Tolerance).ToArray();
        }

        private static ComplexPower CalculatePower(ACVoltage voltage, ACCurrent current)
        {
            var complexPower = voltage.Complex * Complex.Conjugate(current.Complex);
            return new ComplexPower(complexPower);
        }

        public override string ToString()
        {
            var components = string.Join(" -> ", _components.Select(c => 
                $"{c.Component} ({c.ConnectionType})"));
            return $"Circuit: {components}";
        }
    }

    /// <summary>
    /// Represents the response of a circuit to an input signal
    /// </summary>
    public readonly struct CircuitResponse
    {
        public ACVoltage InputVoltage { get; }
        public ACCurrent Current { get; }
        public Impedance TotalImpedance { get; }
        public ComplexPower Power { get; }
        public double Frequency { get; }

        public CircuitResponse(ACVoltage inputVoltage, ACCurrent current, Impedance totalImpedance, 
                             ComplexPower power, double frequency)
        {
            InputVoltage = inputVoltage;
            Current = current;
            TotalImpedance = totalImpedance;
            Power = power;
            Frequency = frequency;
        }

        public override string ToString() => 
            $"f={Frequency:F1} Hz: V={InputVoltage}, I={Current}, Z={TotalImpedance}, P={Power}";
    }

    /// <summary>
    /// Represents complex power (apparent, real, and reactive power)
    /// </summary>
    public readonly struct ComplexPower
    {
        private readonly Complex _complex;

        public ComplexPower(Complex complex)
        {
            _complex = complex;
        }

        /// <summary>
        /// Real power in watts
        /// </summary>
        public double RealPower => _complex.Real;

        /// <summary>
        /// Reactive power in VARs
        /// </summary>
        public double ReactivePower => _complex.Imaginary;

        /// <summary>
        /// Apparent power in VA
        /// </summary>
        public double ApparentPower => _complex.Magnitude;

        /// <summary>
        /// Power factor
        /// </summary>
        public double PowerFactor => RealPower / ApparentPower;

        public override string ToString() => 
            $"{RealPower:F3} W + j{ReactivePower:F3} VAR (PF={PowerFactor:F3})";
    }
}

// Extension methods for older framework compatibility
internal static class ComponentExtensions
{
    public static T[] Where<T>(this ComponentConnection[] source, Func<ComponentConnection, bool> predicate)
    {
        var result = new List<T>();
        foreach (var item in source)
        {
            if (predicate(item))
            {
                result.Add((T)(object)item);
            }
        }
        return result.ToArray();
    }

    public static TResult[] Select<T, TResult>(this ComponentConnection[] source, Func<ComponentConnection, TResult> selector)
    {
        var result = new TResult[source.Length];
        for (int i = 0; i < source.Length; i++)
        {
            result[i] = selector(source[i]);
        }
        return result;
    }

    public static TResult[] ToArray<TResult>(this List<TResult> source)
    {
        return source.ToArray();
    }
}
