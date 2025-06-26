#nullable enable
using System;
using System.Numerics;

namespace CircuitTool.Mathematics
{
    /// <summary>
    /// Window function types for spectral analysis
    /// </summary>
    public enum WindowType
    {
        Rectangular,
        Hanning,
        Hamming,
        Blackman
    }

    /// <summary>
    /// Provides basic Fourier transform utilities for circuit analysis
    /// </summary>
    public static class FourierTransform
    {
        /// <summary>
        /// Computes the Discrete Fourier Transform (DFT) of a complex signal
        /// </summary>
        /// <param name="signal">Input signal</param>
        /// <returns>Frequency domain representation</returns>
        public static Complex[] DFT(Complex[] signal)
        {
            int N = signal.Length;
            var result = new Complex[N];
            
            for (int k = 0; k < N; k++)
            {
                Complex sum = Complex.Zero;
                for (int n = 0; n < N; n++)
                {
                    var angle = -2.0 * Math.PI * k * n / N;
                    var exponential = Complex.FromPolarCoordinates(1.0, angle);
                    sum += signal[n] * exponential;
                }
                result[k] = sum;
            }
            
            return result;
        }
        
        /// <summary>
        /// Computes the Inverse Discrete Fourier Transform (IDFT)
        /// </summary>
        /// <param name="spectrum">Frequency domain representation</param>
        /// <returns>Time domain signal</returns>
        public static Complex[] IDFT(Complex[] spectrum)
        {
            int N = spectrum.Length;
            var result = new Complex[N];
            
            for (int n = 0; n < N; n++)
            {
                Complex sum = Complex.Zero;
                for (int k = 0; k < N; k++)
                {
                    var angle = 2.0 * Math.PI * k * n / N;
                    var exponential = Complex.FromPolarCoordinates(1.0, angle);
                    sum += spectrum[k] * exponential;
                }
                result[n] = sum / N;
            }
            
            return result;
        }
        
        /// <summary>
        /// Computes the DFT of a real-valued signal
        /// </summary>
        /// <param name="signal">Real-valued input signal</param>
        /// <returns>Complex frequency domain representation</returns>
        public static Complex[] DFT(double[] signal)
        {
            var complexSignal = new Complex[signal.Length];
            for (int i = 0; i < signal.Length; i++)
            {
                complexSignal[i] = new Complex(signal[i], 0);
            }
            return DFT(complexSignal);
        }
        
        /// <summary>
        /// Computes power spectral density from a signal
        /// </summary>
        /// <param name="signal">Input signal</param>
        /// <returns>Power spectral density</returns>
        public static double[] PowerSpectralDensity(double[] signal)
        {
            var spectrum = DFT(signal);
            var psd = new double[spectrum.Length];
            
            for (int i = 0; i < spectrum.Length; i++)
            {
                psd[i] = spectrum[i].Magnitude * spectrum[i].Magnitude / signal.Length;
            }
            
            return psd;
        }
        
        /// <summary>
        /// Extracts harmonic content from a periodic signal
        /// </summary>
        /// <param name="signal">Periodic signal samples</param>
        /// <param name="fundamentalFrequency">Fundamental frequency in Hz</param>
        /// <param name="sampleRate">Sample rate in Hz</param>
        /// <param name="maxHarmonics">Maximum number of harmonics to extract</param>
        /// <returns>Harmonic magnitudes and phases</returns>
        public static (double[] magnitudes, double[] phases) ExtractHarmonics(
            double[] signal, 
            double fundamentalFrequency, 
            double sampleRate, 
            int maxHarmonics = 10)
        {
            var spectrum = DFT(signal);
            var magnitudes = new double[maxHarmonics + 1]; // Include DC component
            var phases = new double[maxHarmonics + 1];
            
            double frequencyResolution = sampleRate / signal.Length;
            
            // DC component
            magnitudes[0] = spectrum[0].Magnitude / signal.Length;
            phases[0] = spectrum[0].Phase;
            
            // Harmonics
            for (int h = 1; h <= maxHarmonics; h++)
            {
                double harmonicFrequency = h * fundamentalFrequency;
                int binIndex = (int)Math.Round(harmonicFrequency / frequencyResolution);
                
                if (binIndex < spectrum.Length)
                {
                    magnitudes[h] = spectrum[binIndex].Magnitude * 2.0 / signal.Length;
                    phases[h] = spectrum[binIndex].Phase;
                }
            }
            
            return (magnitudes, phases);
        }
        
        /// <summary>
        /// Calculates Total Harmonic Distortion (THD) from harmonic content
        /// </summary>
        /// <param name="harmonicMagnitudes">Harmonic magnitudes (fundamental at index 1)</param>
        /// <returns>THD as a ratio (0-1)</returns>
        public static double CalculateTHD(double[] harmonicMagnitudes)
        {
            if (harmonicMagnitudes.Length < 2)
                throw new ArgumentException("At least fundamental and one harmonic required");
            
            double fundamental = harmonicMagnitudes[1]; // Index 1 is fundamental
            double harmonicSum = 0;
            
            for (int i = 2; i < harmonicMagnitudes.Length; i++)
            {
                harmonicSum += harmonicMagnitudes[i] * harmonicMagnitudes[i];
            }
            
            return Math.Sqrt(harmonicSum) / fundamental;
        }
        
        /// <summary>
        /// Applies a window function to reduce spectral leakage
        /// </summary>
        /// <param name="signal">Input signal</param>
        /// <param name="windowType">Type of window to apply</param>
        /// <returns>Windowed signal</returns>
        public static double[] ApplyWindow(double[] signal, WindowType windowType = WindowType.Hanning)
        {
            int N = signal.Length;
            var windowed = new double[N];
            
            for (int n = 0; n < N; n++)
            {
                double window = windowType switch
                {
                    WindowType.Rectangular => 1.0,
                    WindowType.Hanning => 0.5 * (1 - Math.Cos(2 * Math.PI * n / (N - 1))),
                    WindowType.Hamming => 0.54 - 0.46 * Math.Cos(2 * Math.PI * n / (N - 1)),
                    WindowType.Blackman => 0.42 - 0.5 * Math.Cos(2 * Math.PI * n / (N - 1)) + 
                                          0.08 * Math.Cos(4 * Math.PI * n / (N - 1)),
                    _ => 1.0
                };
                
                windowed[n] = signal[n] * window;
            }
            
            return windowed;
        }
    }
}
