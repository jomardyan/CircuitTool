#nullable enable
using System;
using System.Collections.Generic;

namespace CircuitTool
{
    /// <summary>
    /// Provides specialized calculations for DC motor control and analysis
    /// </summary>
    public static class MotorControlCalculator
    {
        /// <summary>
        /// Motor types for different calculation methods
        /// </summary>
        public enum MotorType
        {
            BrushedDC,
            BrushlessDC,
            StepperMotor,
            ServoMotor
        }

        /// <summary>
        /// Calculates H-bridge current requirements for DC motor
        /// </summary>
        /// <param name="motorVoltage">Motor rated voltage</param>
        /// <param name="motorCurrent">Motor rated current</param>
        /// <param name="stallCurrent">Motor stall current</param>
        /// <param name="safetyFactor">Safety factor (typically 1.5-2.0)</param>
        /// <returns>Required H-bridge current rating</returns>
        public static double CalculateHBridgeCurrent(double motorVoltage, double motorCurrent, double stallCurrent, double safetyFactor = 1.8)
        {
            double maxCurrent = Math.Max(motorCurrent, stallCurrent);
            return maxCurrent * safetyFactor;
        }

        /// <summary>
        /// Calculates motor driver heat sink requirements
        /// </summary>
        /// <param name="motorCurrent">Motor current in amperes</param>
        /// <param name="driverRdsOn">Driver MOSFET RDS(on) in ohms</param>
        /// <param name="ambientTemp">Ambient temperature in °C</param>
        /// <param name="maxJunctionTemp">Maximum junction temperature in °C</param>
        /// <returns>Required thermal resistance in °C/W</returns>
        public static double CalculateMotorDriverThermal(double motorCurrent, double driverRdsOn, double ambientTemp = 25, double maxJunctionTemp = 150)
        {
            double powerDissipation = motorCurrent * motorCurrent * driverRdsOn * 2; // For H-bridge
            double tempRise = maxJunctionTemp - ambientTemp;
            return tempRise / powerDissipation;
        }

        /// <summary>
        /// Calculates stepper motor step resolution
        /// </summary>
        /// <param name="stepsPerRevolution">Motor steps per revolution</param>
        /// <param name="microsteps">Microstepping factor</param>
        /// <param name="gearRatio">Gear reduction ratio (1 = no gears)</param>
        /// <returns>Angular resolution in degrees per step</returns>
        public static double CalculateStepperResolution(int stepsPerRevolution, int microsteps = 1, double gearRatio = 1.0)
        {
            double totalSteps = stepsPerRevolution * microsteps * gearRatio;
            return 360.0 / totalSteps;
        }

        /// <summary>
        /// Calculates encoder resolution requirements
        /// </summary>
        /// <param name="desiredAccuracy">Desired position accuracy in degrees</param>
        /// <param name="gearRatio">Gear reduction ratio</param>
        /// <returns>Required encoder pulses per revolution</returns>
        public static int CalculateEncoderResolution(double desiredAccuracy, double gearRatio = 1.0)
        {
            double requiredResolution = 360.0 / (desiredAccuracy * gearRatio);
            return (int)Math.Ceiling(requiredResolution);
        }

        /// <summary>
        /// Calculates PID controller gains for motor position control
        /// </summary>
        /// <param name="motorInertia">Motor + load inertia in kg⋅m²</param>
        /// <param name="motorTorqueConstant">Motor torque constant in Nm/A</param>
        /// <param name="desiredBandwidth">Desired closed-loop bandwidth in Hz</param>
        /// <returns>PID gains (Kp, Ki, Kd)</returns>
        public static (double Kp, double Ki, double Kd) CalculatePIDGains(double motorInertia, double motorTorqueConstant, double desiredBandwidth)
        {
            double omega = 2 * Math.PI * desiredBandwidth;
            
            // Simplified PID tuning based on pole placement
            double Kp = motorInertia * omega * omega / motorTorqueConstant;
            double Kd = 2 * motorInertia * omega / motorTorqueConstant;
            double Ki = Kp * omega / 10; // Rule of thumb: Ki = Kp * omega / 10
            
            return (Kp, Ki, Kd);
        }

        /// <summary>
        /// Calculates motor acceleration/deceleration ramp
        /// </summary>
        /// <param name="targetSpeed">Target speed in RPM</param>
        /// <param name="acceleration">Acceleration in RPM/s</param>
        /// <returns>Ramp time in seconds</returns>
        public static double CalculateRampTime(double targetSpeed, double acceleration)
        {
            return Math.Abs(targetSpeed) / acceleration;
        }

        /// <summary>
        /// Calculates regenerative braking power
        /// </summary>
        /// <param name="motorSpeed">Motor speed in RPM</param>
        /// <param name="motorTorque">Braking torque in Nm</param>
        /// <param name="efficiency">Motor efficiency (0-1)</param>
        /// <returns>Regenerated power in watts</returns>
        public static double CalculateRegenerativePower(double motorSpeed, double motorTorque, double efficiency = 0.85)
        {
            double mechanicalPower = (motorSpeed * 2 * Math.PI / 60) * motorTorque; // Convert RPM to rad/s
            return mechanicalPower * efficiency;
        }

        /// <summary>
        /// Calculates current limit for motor protection
        /// </summary>
        /// <param name="motorType">Type of motor</param>
        /// <param name="ratedCurrent">Motor rated current</param>
        /// <param name="thermalTimeConstant">Motor thermal time constant in seconds</param>
        /// <param name="operationTime">Expected operation time in seconds</param>
        /// <returns>Safe current limit</returns>
        public static double CalculateCurrentLimit(MotorType motorType, double ratedCurrent, double thermalTimeConstant = 300, double operationTime = 60)
        {
            // I²t protection calculation
            double thermalFactor = Math.Sqrt(thermalTimeConstant / operationTime);
            
            double multiplier = motorType switch
            {
                MotorType.BrushedDC => 1.5,
                MotorType.BrushlessDC => 2.0,
                MotorType.StepperMotor => 1.4,
                MotorType.ServoMotor => 1.8,
                _ => 1.5
            };
            
            return ratedCurrent * Math.Min(multiplier, thermalFactor);
        }

        /// <summary>
        /// Generates motor control code template
        /// </summary>
        /// <param name="motorType">Type of motor</param>
        /// <param name="controlInterface">Control interface (PWM, SPI, etc.)</param>
        /// <returns>Code template string</returns>
        public static string GenerateControlCode(MotorType motorType, string controlInterface = "PWM")
        {
            return motorType switch
            {
                MotorType.BrushedDC => GenerateBrushedDCCode(controlInterface),
                MotorType.StepperMotor => GenerateStepperCode(controlInterface),
                MotorType.ServoMotor => GenerateServoCode(controlInterface),
                _ => "// Motor control code template not available for this type"
            };
        }

        private static string GenerateBrushedDCCode(string interface_)
        {
            return $@"
// Brushed DC Motor Control ({interface_})
class DCMotorController {{
    private int pwmPin;
    private int dirPin1, dirPin2;
    
    public DCMotorController(int pwm, int dir1, int dir2) {{
        this.pwmPin = pwm;
        this.dirPin1 = dir1;
        this.dirPin2 = dir2;
    }}
    
    public void SetSpeed(double speed) {{ // speed: -1.0 to +1.0
        if (speed > 0) {{
            digitalWrite(dirPin1, HIGH);
            digitalWrite(dirPin2, LOW);
        }} else {{
            digitalWrite(dirPin1, LOW);
            digitalWrite(dirPin2, HIGH);
            speed = -speed;
        }}
        analogWrite(pwmPin, (int)(speed * 255));
    }}
    
    public void Stop() {{
        analogWrite(pwmPin, 0);
    }}
    
    public void Brake() {{
        digitalWrite(dirPin1, HIGH);
        digitalWrite(dirPin2, HIGH);
        analogWrite(pwmPin, 255);
    }}
}}";
        }

        private static string GenerateStepperCode(string interface_)
        {
            return $@"
// Stepper Motor Control ({interface_})
class StepperController {{
    private int stepPin, dirPin, enablePin;
    private int stepsPerRev;
    
    public StepperController(int step, int dir, int enable, int steps) {{
        this.stepPin = step;
        this.dirPin = dir;
        this.enablePin = enable;
        this.stepsPerRev = steps;
    }}
    
    public void Step(int steps, int delayMicros = 1000) {{
        digitalWrite(enablePin, LOW); // Enable motor
        digitalWrite(dirPin, steps > 0 ? HIGH : LOW);
        
        for (int i = 0; i < abs(steps); i++) {{
            digitalWrite(stepPin, HIGH);
            delayMicroseconds(delayMicros / 2);
            digitalWrite(stepPin, LOW);
            delayMicroseconds(delayMicros / 2);
        }}
    }}
    
    public void Rotate(double degrees, double rpm = 60) {{
        int steps = (int)(degrees * stepsPerRev / 360.0);
        int delayMicros = (int)(60.0 * 1000000.0 / (rpm * stepsPerRev));
        Step(steps, delayMicros);
    }}
}}";
        }

        private static string GenerateServoCode(string interface_)
        {
            return $@"
// Servo Motor Control ({interface_})
class ServoController {{
    private int servoPin;
    private double minPulse, maxPulse;
    
    public ServoController(int pin, double minUs = 1000, double maxUs = 2000) {{
        this.servoPin = pin;
        this.minPulse = minUs;
        this.maxPulse = maxUs;
    }}
    
    public void SetAngle(double angle) {{ // angle: 0-180 degrees
        angle = constrain(angle, 0, 180);
        double pulseWidth = map(angle, 0, 180, minPulse, maxPulse);
        servoWrite(servoPin, (int)pulseWidth);
    }}
    
    public void SetPosition(double position) {{ // position: -1.0 to +1.0
        double angle = map(position, -1.0, 1.0, 0, 180);
        SetAngle(angle);
    }}
}}";
        }
    }
}
