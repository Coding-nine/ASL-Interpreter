# Hardware and software dependencies

## Hardware identified in the sketch

- ESP32-class board (the code uses ESP32 ADC configuration and GPIO assignments).
- MPU9250 IMU at I2C address `0x68`.
- Ten flex sensors.
- Fixed resistors used by the resistance calculation.
- USB serial connection to the computer.

The exact ESP32 board model, sensor breakout revision, power wiring, and resistor tolerances are not recorded in the recovered files.

## Pin map

### MPU9250

| Signal | ESP32 pin / setting |
|---|---:|
| SDA | GPIO 21 |
| SCL | GPIO 22 |
| I2C address | `0x68` |

The sketch wakes the device by writing zero to register `0x6B`, then reads 14 bytes beginning at register `0x3B`. It uses accelerometer and gyroscope values; the magnetometer portion of the MPU9250 is not read.

### Flex sensors

| Sensor | ESP32 ADC pin | Fixed resistor in sketch |
|---|---:|---:|
| Thumb MCP | GPIO 32 | 10 kΩ |
| Thumb PIP | GPIO 33 | 10 kΩ |
| Index MCP | GPIO 34 | 22 kΩ |
| Index PIP | GPIO 25 | 10 kΩ |
| Middle MCP | GPIO 26 | 22 kΩ |
| Middle PIP | GPIO 27 | 10 kΩ |
| Ring MCP | GPIO 14 | 10 kΩ |
| Ring PIP | GPIO 12 | 10 kΩ |
| Pinky MCP | GPIO 13 | 10 kΩ |
| Pinky PIP | GPIO 4 | 10 kΩ |

The sketch sets 12-bit ADC resolution (`0..4095`) and `ADC_11db` attenuation. Confirm that every listed pin is available on the exact ESP32 board revision and check board-specific boot-strapping constraints before wiring.

## Flex divider calculation

The sketch calculates:

```text
voltage = adc * (3.3 / 4095)
resistance = R_FIXED * (3.3 / voltage - 1)
```

This equation corresponds to a divider with the flex sensor on the 3.3 V side and the fixed resistor between the ADC node and ground. The actual wiring should be checked against the physical glove before applying power. A near-zero ADC voltage is returned as resistance `0` to avoid division by zero.

## Arduino libraries

| Dependency | Status |
|---|---|
| `Wire.h` | Supplied by the Arduino/ESP32 core |
| `MadgwickAHRS.h` | External Madgwick filter library; exact package and version are not recorded |
| ESP32 board support | Required for `analogReadResolution`, `analogSetAttenuation`, `ADC_11db`, and the selected pin map |

The MPU9250 is accessed directly through `Wire` register transactions; no separate MPU9250 driver library is included or referenced by the sketch.

The sketch uses `Madgwick filter`, `filter.begin(50)`, and `filter.updateIMU(...)`. Install a library whose `MadgwickAHRS.h` exposes that API, then compile against the selected ESP32 board package. Record the exact library name/version after a successful build instead of guessing it from the header filename alone.

## Python dependency

`Python/requirements.txt` contains:

```text
pyserial>=3.5,<4.0
```

The `socket` and `threading`-independent bridge behavior uses Python's standard library for UDP sockets; no third-party UDP package is required.

## Unity packages and binaries

The recovered `Packages/manifest.json` declares Unity packages including:

- Input System `1.20.0`
- Universal Render Pipeline `17.6.0`
- UGUI `2.6.0`
- Timeline `6.6.0`
- AI Navigation `2.0.14`
- Visual Scripting `1.9.12`
- Unity Test Framework `1.8.0`

The project also includes `Assets/System.Speech.dll`, and `WindowsTTS.cs` references `System.Speech.Synthesis`. This makes the current speech implementation Windows-oriented. The DLL's origin and redistribution rights were not established by the archive inspection.

TextMesh Pro assets are present under `Assets/TextMesh Pro/`. `com.unity.textmeshpro` is not explicitly listed in the recovered package manifest or lock file; verify TextMesh Pro availability on a clean Unity 6000.6.2f1 import.

## Calibration warning

`HandUDPReceiver.cs` contains inspector calibration values. The scene overrides some defaults:

| Channel | Script default straight/bent | Scene straight/bent |
|---|---:|---:|
| Ring MCP | 18000 / 8000 | 60000 / 35000 |
| Ring PIP | 15000 / 7000 | 35000 / 10000 |
| Pinky MCP | 6000 / 0 | 10000 / 0 |
| Pinky PIP | 12000 / 6000 | 25000 / 10000 |

The scene values are the values loaded when `SampleScene` opens. Calibrate each sensor against measured straight and bent resistance; do not assume either set is correct for a different glove.
