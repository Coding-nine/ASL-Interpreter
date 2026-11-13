# Setup and operation

## Before starting

1. Confirm the hardware pinout and divider wiring against the actual ESP32 board.
2. Install the ESP32 board package and the Madgwick library that supplies `MadgwickAHRS.h`.
3. Upload the Arduino sketch.
4. Identify the serial port assigned by the operating system.
5. Install Python dependencies.
6. Open the Unity project with the recorded editor version.

## Arduino upload

Open `Arduino/sketch_feb1a.ino`. Select the correct ESP32 board and serial port in Arduino IDE, then compile and upload. The sketch uses `Serial.begin(115200)`. Do not leave the Arduino Serial Monitor connected while the Python bridge is trying to open the same port.

## Python bridge

Create a Python virtual environment and install dependencies:

```bash
python -m venv .venv
.venv\Scripts\activate
python -m pip install -r Python/requirements.txt
```

Edit the `SERIAL_PORT` constant in `Python/arduino_to_udp.py` to match the actual port, for example `COM5`. Then run:

```bash
python Python/arduino_to_udp.py
```

The script sends the serial line unchanged to UDP `127.0.0.1:5005`.

## Unity

Open the `Unity/` folder in Unity Hub. Select editor `6000.6.2f1`, open `Assets/Scenes/SampleScene.unity`, and enter Play mode. Unity recreates the omitted `Library/` folder during import.

The scene contains an active `HandUDPReceiver` configured for port `5005`. Keep the Python bridge and Unity on the same computer for the current loopback configuration.

## Controls

- `C`: save the current wrist pose as neutral.
- `Q`: start appending detected letters to the UI.
- `E`: stop appending letters.
- Camera: WASD movement, right mouse button to look, Left Shift to move faster.

The scene's visible start and stop buttons have no persistent click callbacks in the serialized scene. The keyboard controls are the controls implemented in `ASLUIController.cs`.

## Verification checklist

- [ ] Arduino compiles for the selected ESP32 board.
- [ ] Serial output contains 14 comma-separated numeric fields per line.
- [ ] Serial output rate and Madgwick filter rate are reconciled.
- [ ] Python opens the intended serial port at 115200 baud.
- [ ] UDP datagrams reach local port 5005.
- [ ] Unity imports without missing scripts or missing packages.
- [ ] `HandUDPReceiver` receives packets without runtime exceptions.
- [ ] Wrist neutral calibration behaves as expected.
- [ ] Every finger sensor maps to the intended joint.
- [ ] Thumb sensor mapping is restored and calibrated.
- [ ] Duplicate ASL recognition conditions are corrected and handshapes are validated.
- [ ] UI controls, repeated-letter behavior, and Windows TTS are tested.
