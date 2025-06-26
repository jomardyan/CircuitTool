#nullable enable
using System;

namespace CircuitTool
{
    /// <summary>
    /// Provides calculations for signal integrity analysis
    /// </summary>
    public static class SignalIntegrityCalculator
    {
        /// <summary>
        /// Calculates the characteristic impedance of a microstrip transmission line
        /// </summary>
        /// <param name="traceWidth">Trace width in meters</param>
        /// <param name="dielectricThickness">Dielectric thickness in meters</param>
        /// <param name="dielectricConstant">Relative dielectric constant</param>
        /// <returns>Characteristic impedance in ohms</returns>
        public static double MicrostripImpedance(double traceWidth, double dielectricThickness, double dielectricConstant)
        {
            if (traceWidth <= 0) throw new ArgumentException("Trace width must be positive", nameof(traceWidth));
            if (dielectricThickness <= 0) throw new ArgumentException("Dielectric thickness must be positive", nameof(dielectricThickness));
            if (dielectricConstant <= 1) throw new ArgumentException("Dielectric constant must be greater than 1", nameof(dielectricConstant));
            
            double widthToHeightRatio = traceWidth / dielectricThickness;
            double effectiveDielectric = (dielectricConstant + 1) / 2 + 
                                       (dielectricConstant - 1) / 2 * Math.Pow(1 + 12 / widthToHeightRatio, -0.5);
            
            double z0;
            if (widthToHeightRatio < 1)
            {
                z0 = 60 / Math.Sqrt(effectiveDielectric) * Math.Log(8 / widthToHeightRatio + widthToHeightRatio / 4);
            }
            else
            {
                z0 = 120 * Math.PI / (Math.Sqrt(effectiveDielectric) * 
                     (widthToHeightRatio + 1.393 + 0.667 * Math.Log(widthToHeightRatio + 1.444)));
            }
            
            return z0;
        }

        /// <summary>
        /// Calculates the propagation delay of a transmission line
        /// </summary>
        /// <param name="length">Line length in meters</param>
        /// <param name="effectiveDielectric">Effective dielectric constant</param>
        /// <returns>Propagation delay in seconds</returns>
        public static double PropagationDelay(double length, double effectiveDielectric)
        {
            if (length <= 0) throw new ArgumentException("Length must be positive", nameof(length));
            if (effectiveDielectric <= 1) throw new ArgumentException("Effective dielectric must be greater than 1", nameof(effectiveDielectric));
            
            const double speedOfLight = 299792458; // m/s
            double velocity = speedOfLight / Math.Sqrt(effectiveDielectric);
            return length / velocity;
        }

        /// <summary>
        /// Calculates crosstalk coupling coefficient between parallel traces
        /// </summary>
        /// <param name="traceSpacing">Spacing between traces in meters</param>
        /// <param name="traceWidth">Trace width in meters</param>
        /// <param name="dielectricHeight">Height above ground plane in meters</param>
        /// <returns>Coupling coefficient (0-1)</returns>
        public static double CrosstalkCoupling(double traceSpacing, double traceWidth, double dielectricHeight)
        {
            if (traceSpacing <= 0) throw new ArgumentException("Trace spacing must be positive", nameof(traceSpacing));
            if (traceWidth <= 0) throw new ArgumentException("Trace width must be positive", nameof(traceWidth));
            if (dielectricHeight <= 0) throw new ArgumentException("Dielectric height must be positive", nameof(dielectricHeight));
            
            // Simplified coupling model
            double spacingRatio = traceSpacing / dielectricHeight;
            double widthRatio = traceWidth / dielectricHeight;
            
            // Empirical formula for edge-coupled microstrip
            return 0.1 * Math.Exp(-1.5 * spacingRatio) * (1 + 0.5 * widthRatio);
        }

        /// <summary>
        /// Calculates the rise time degradation due to transmission line effects
        /// </summary>
        /// <param name="originalRiseTime">Original rise time in seconds</param>
        /// <param name="propagationDelay">Propagation delay in seconds</param>
        /// <returns>Degraded rise time in seconds</returns>
        public static double RiseTimeDegradation(double originalRiseTime, double propagationDelay)
        {
            if (originalRiseTime <= 0) throw new ArgumentException("Rise time must be positive", nameof(originalRiseTime));
            if (propagationDelay <= 0) throw new ArgumentException("Propagation delay must be positive", nameof(propagationDelay));
            
            // Rule of thumb: if propagation delay > rise_time/6, significant degradation occurs
            if (propagationDelay > originalRiseTime / 6)
            {
                return Math.Sqrt(originalRiseTime * originalRiseTime + (2 * propagationDelay) * (2 * propagationDelay));
            }
            return originalRiseTime;
        }
    }
}
