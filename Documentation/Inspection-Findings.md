# Inspection findings from the recovered source

## What is present

- Arduino sketch: `sketch_feb1a.ino`.
- Python bridge: `arduino_to_udp.py`.
- Unity project source directories: `Assets/`, `Packages/`, `ProjectSettings/`.
- Unity scene: `Assets/Scenes/SampleScene.unity`.
- Six project C# scripts: `HandUDPReceiver`, `HandController`, `ASLTranslator`, `ASLUIController`, `CameraController`, and `WindowsTTS`.
- Hand model asset `Assets/hand2.fbx`; its scene bone references resolve to the included model GUID.
- `Assets` files have corresponding `.meta` files in the archive.
- `Packages/manifest.json`, `Packages/packages-lock.json`, and `ProjectSettings/ProjectVersion.txt` are present.
- Unity version recorded as `6000.6.2f1`.
- `Library/` and `Temp/` were not in the archive.

## Unity scene wiring verified from serialized scene data

- `SampleScene.unity` is enabled in `EditorBuildSettings.asset`.
- `HandUDPReceiver` is attached to the scene and configured to listen on port `5005`.
- The receiver's `handController` reference is assigned.
- The `HandController` bone Transform references are assigned to the hand model.
- `ASLTranslator.hand` is assigned to the `HandController`.
- `ASLUIController` references its output TextMeshPro object and `ASLTranslator`.
- `WindowsTTS` is attached to `TTSManager`.
- The `startBtn` and `stopBtn` UI Button `m_OnClick` persistent-call lists are empty.

## Functional issues in recovered source

### Thumb path is disconnected

Arduino emits thumb values at fields 4 and 5. `HandUDPReceiver.ReceiveData()` has both thumb `ApplyFingerAngle` calls commented out. As a result, `HandController.thumbCMC` and `thumbIP` remain at their initial values unless changed manually elsewhere. `ASLTranslator` uses those values in multiple letter predicates.

### Duplicate / unreachable recognition rules

The following predicates are textually equivalent:

- C and O both require all four non-thumb fingers to be neither classified bent nor straight. C appears first, so O cannot be returned.
- H and U both require index and middle straight, ring and pinky bent. H appears first, so U cannot be returned.
- R and V both require index and middle straight, ring and pinky bent, and thumb bent. R appears first, so V cannot be returned.

J and Z do not have recognition rules. They require movement in standard ASL fingerspelling, but this implementation only evaluates static thresholds.

### UI behavior

`ASLTranslator.Update()` performs recognition and logs a result every frame regardless of whether `ASLUIController.translating` is true. The UI separately calls `RecognizeLetter()` while active.

The UI ignores `'-'` and ignores any character equal to `lastLetter`. `lastLetter` is not reset on a neutral pose, stop, or restart. Therefore consecutive repetitions of the same letter are suppressed.

The scene's named start/stop buttons are not connected to UI methods. `Q` and `E` are implemented as keyboard controls.

### Timing and threading

The Arduino initializes Madgwick with `filter.begin(50)` but delays 10 ms per loop, nominally producing about 100 updates per second. This rate mismatch should be resolved deliberately.

`HandUDPReceiver` runs a blocking UDP receive loop on a background thread and writes fields on a Unity `MonoBehaviour`/`HandController` from that thread. Unity scene-object state should generally be applied on the main thread. The catch-all exception handler hides the reason for receive/parse failures. Shutdown uses `Thread.Abort()`, which is obsolete and unsafe.

### Dependencies / reproducibility risks

- The exact Madgwick Arduino library package and version are not documented in the source.
- ESP32 Arduino core version and exact board model are not documented.
- TextMesh Pro assets exist, but the recovered `Packages/manifest.json` and lock file do not explicitly list `com.unity.textmeshpro`; verify on a clean import.
- `Assets/System.Speech.dll` is present and referenced by `WindowsTTS.cs`; confirm its origin, compatibility, and redistribution license.
- The source does not include a circuit diagram, sensor calibration record, screenshots, or a hardware bill of materials beyond pin and resistor constants.
- The project has not been compiled or run in Unity as part of this inspection. Source presence does not prove runtime correctness.

## GitHub inclusion decision

Include:

- `Arduino/sketch_feb1a.ino`
- `Python/arduino_to_udp.py` and `Python/requirements.txt`
- `Unity/Assets/`, including every `.meta` file
- `Unity/Packages/manifest.json` and `Unity/Packages/packages-lock.json`
- `Unity/ProjectSettings/`
- Documentation and root project files

Exclude:

- Unity `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, `MemoryCaptures/`, and build output folders
- Generated `.csproj`, `.sln`, `.slnx` files
- `.vscode/`, `.vs/`, `.idea/`, and machine-specific editor settings
- Python virtual environments, caches, and bytecode

Do not remove additional assets from `Unity/Assets/` until their scene/prefab/material dependencies and license terms have been checked.
