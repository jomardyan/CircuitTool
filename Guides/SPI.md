# SPI (Serial Peripheral Interface) Guide

SPI is a synchronous serial communication interface used for short-distance communication, primarily in embedded systems.

## Key Features
- Four-wire interface: MOSI, MISO, SCLK, SS/CS
- Full-duplex communication
- High-speed data transfer

## Typical Applications
- SD cards
- Displays
- Sensors

## Example Circuit
```
MCU (Master) --- MOSI/MISO/SCLK/SS --- Peripheral (Slave)
```

## Resources
- [SPI Wikipedia](https://en.wikipedia.org/wiki/Serial_Peripheral_Interface)
