# Arduino dependencies

- `Wire.h`: provided by the selected Arduino/ESP32 board core.
- `MadgwickAHRS.h`: external Madgwick filter library. The recovered sketch does not record the exact library package or version; install a compatible library exposing `Madgwick`, `begin()`, `updateIMU()`, and `q0`–`q3`, then record the tested version.
- ESP32 board support package: required for the ESP32 ADC APIs and pin configuration. Exact core version and board model were not recorded.

The MPU9250 is accessed with direct I2C register reads; no MPU9250-specific library is referenced.
