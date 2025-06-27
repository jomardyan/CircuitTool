# I2C (Inter-Integrated Circuit) Guide

I2C is a synchronous, multi-master, multi-slave, packet switched, single-ended, serial communication bus. It is widely used for attaching lower-speed peripheral ICs to processors and microcontrollers.

## Key Features
- Two-wire interface: SDA (data), SCL (clock)
- Supports multiple masters and slaves
- Speeds: Standard (100 kbit/s), Fast (400 kbit/s), Fast Plus (1 Mbit/s), High-speed (3.4 Mbit/s)

## Typical Applications
- Sensor interfacing
- EEPROMs
- Real-time clocks

## Example Circuit
```
MCU (Master) --- SDA/SCL --- Sensor (Slave)
```

## Resources
- [I2C Wikipedia](https://en.wikipedia.org/wiki/I%C2%B2C)
- [NXP I2C Bus Specification](https://www.nxp.com/docs/en/user-guide/UM10204.pdf)
