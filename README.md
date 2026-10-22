# AR Table Setting Trainer

An Android augmented reality app that teaches correct table setting. The user places a virtual table on a real floor, then drags tableware from the front of the table to its correct position. Placement, orientation, time and mistakes are scored, and recent sessions are saved on the device.


## Screenshots

| Main menu | Scanning for a surface | Placing the table |
| --- | --- | --- |
| ![Main menu](Assets/Screenshots/01-main-menu.jpeg) | ![Scanning](Assets/Screenshots/02-scanning.jpeg) | ![Placing the table](Assets/Screenshots/03-place-table.jpeg) |

| Simulation | Hint after a wrong drop | Results |
| --- | --- | --- |
| ![Simulation](Assets/Screenshots/04-simulation.jpeg) | ![Hint](Assets/Screenshots/05-hint.jpeg) | ![Results](Assets/Screenshots/06-results.jpeg) |

| History |
| --- |
| ![History](Assets/Screenshots/07-history.jpeg) |

## How to play

1. Choose a level on the main menu: Basic or Standard.
2. Press Start and move the phone slowly over the floor until a surface is detected.
3. Tap the detected surface to place the table. Tap elsewhere to move it, then press Confirm.
4. Press Begin to start the level.
5. Drag each item from the front of the table to its place. Select a piece of cutlery and press Rotate to turn it in 90 degree steps.
6. A wrong drop counts as a mistake and the item returns to the tray area. A correct drop locks the item in place.
7. When every item is placed, or time runs out, a results screen shows the score, time taken and mistakes.

The History screen lists the last five sessions, and Quit on the main menu closes the app.

## Features

- AR plane detection and table placement with a confirm step
- Two levels with different item sets, time limits, pass marks and scoring
- Drag and drop placement with position and rotation checks
- Colour guides(Slightly see-through) on the table that show where to place all items but not literally and disappear once the item is placed
- Hints that explain where each item belongs
- Live HUD with score, timer, items placed and mistakes
- Sound effects for pick up, correct, incorrect, start and finish
- Session history saved to a JSON file on the device
- Results screen with a short pause so the final placement can be seen

## Scoring

| Strategy | Formula |
| --- | --- |
| Basic | 100 x correct / total - 5 x mistakes |
| Time bonus | Basic score plus up to 10 points depending on time left |

Scores never drop below zero. A level is passed when the score reaches the pass mark set on the level asset.

## Design

| Pattern or technique | Where it is used |
| --- | --- |
| State machine | `GameStates.cs` and `GameController.cs` move between Start, Setup, Simulation and Completion |
| Observer (events) | `GameEvents.cs` carries score, timer, item and level events to the UI and audio |
| Strategy | `ScoringStrategies.cs` swaps between Basic and Time bonus scoring |
| Factory | `TablewareFactory.cs` builds the items for a level |
| Object pool | `ObjectPool.cs` reuses item instances between restarts |
| Repository | `JsonSessionRepository.cs` hides how sessions are stored |
| Inheritance and polymorphism | `TablewareItem` is extended by `Plate`, `Glass`, `Napkin` and `Cutlery`, each with its own hint and orientation rule |
| ScriptableObjects | `TableSettingLevel` defines each level in the Inspector |
| Addressables | `AssetLoader.cs` loads item prefabs on demand and releases them afterwards |

## Project structure

```
Assets/
  Scripts/            Game logic (states, events, scoring, items, UI, saving)
  Prefabs/            TablePlaceholder and the TableWare prefabs
  Models/             Third party table set and GUI pack
  Scenes/             ARTableSetting
  AddressableAssetsData/
  Screenshots/        Images used in this README
```

## Requirements

- Unity 6000.4.7f1
- AR Foundation with the ARCore XR plugin
- Addressables, TextMesh Pro and the Input System package
- An Android 10 (API level 29) or newer phone that supports ARCore
- Tested on an Android phone(Samsung A32)

## Build and run

1. Open the project in Unity 6000.4.7f1.
2. Open File > Build Profiles and select Android.
3. Open Window > Asset Management > Addressables > Groups and choose Build > New Build > Default Build Script.
4. Connect an ARCore compatible phone with USB debugging on.
5. Press Build And Run.

## Known limitations

- The detected floor height can be off by a few centimetres, so the table may not sit exactly on the floor. Scanning for a few more seconds before tapping improves it.
- Plane detection works best on textured, well lit floors.
- Session history is stored only on the device and keeps the latest 50 sessions.

## Assets and credits

- Dining Set (3D models) by FunFant: https://assetstore.unity.com/packages/3d/props/interior/dining-set-37029
- Free Casual GUI (interface sprites and icons) by Sky Den Games: https://assetstore.unity.com/packages/2d/gui/free-casual-gui-332804
- Both packs are used under the Standard Unity Asset Store EULA.
- Sound effects from Freesound, all used under the Creative Commons 0 licence:
  - [Item or Material Pickup Pop 1 of 3](https://freesound.org/s/665183/) by [el_boss](https://freesound.org/people/el_boss/)
  - [End Game - Fail](https://freesound.org/s/135831/) by [davidbain](https://freesound.org/people/davidbain/)
  - [correct.wav](https://freesound.org/s/546084/) by [StavSounds](https://freesound.org/people/StavSounds/)
  - [Incorrect Buzzer](https://freesound.org/s/700641/) by [Producing_RayLite](https://freesound.org/people/Producing_RayLite/)
  - [Bell Ringing](https://freesound.org/s/476175/) by [unadamlar](https://freesound.org/people/unadamlar/)