# ModTools - Stardew Valley Toolkit Mod

A comprehensive Stardew Valley toolkit mod featuring resource multipliers, fishing enhancements, time control, and various quality-of-life improvements.

[中文版说明 / Chinese Documentation](README_zh.md)

## Features

### 🎯 Resource Multiplier System
- **Adjustable Multiplier**: Resource gain multiplier can be adjusted from 3x to 50x
- **Supported Items**: Wood, stone, ores, gems, minerals, and all resource-type items
- **Smart Detection**: Automatically detects resource items and applies multiplier effects

### 🎣 Fishing Enhancement Features
#### Difficulty Adjustments
- **Easier Fishing Mode**: Enable simpler fishing motion patterns
- **Difficulty Multiplier**: Adjust fishing difficulty multiplier (0.1-2.0)
- **Difficulty Additive**: Add difficulty adjustment value (-50 to +50)
- **Fishing Bar Height**: Customize fishing bar height (60-568)

#### Convenience Features
- **Always Perfect**: Always get perfect catches
- **Always Treasure**: Always find treasure while fishing
- **Instant Catch**: Instantly catch fish (skip minigame)
- **Instant Treasure**: Instantly catch treasure
- **Infinite Bait**: Bait never gets consumed
- **Infinite Tackle**: Tackle never gets consumed
- **Auto Hook**: Automatically hook fish when they bite

#### Fish Behavior
- **Fish Movement Speed**: Adjust fish movement speed in minigame (0.1-1.0)
- **Larger Fishing Bar**: Make the fishing bar larger for easier catching

### ⏰ Time Control System
- **Time Rate**: Adjust time passage speed (0.1-2.0x)
- **Slow Mode**: Time passes slower, giving more time to complete tasks
- **Fast Mode**: Time passes faster, quickly advance game progress

## Installation

### Prerequisites
- [SMAPI](https://smapi.io/) 3.0 or higher
- [Stardew Valley](https://www.stardewvalley.net/) 1.6 or higher

### Installation Steps
1. Download the latest version of ModTools
2. Extract to `Stardew Valley/Mods/` directory
3. Run SMAPI to start the game
4. The mod will load automatically

## Configuration

### Method 1: In-Game Configuration Menu (Recommended)
1. Press `K` key in-game to open the custom configuration menu
2. Adjust various settings
3. Settings will be saved automatically

### Method 2: Generic Mod Config Menu (GMCM)
If you have GMCM installed, you can find ModTools configuration options in the mod settings.

### Method 3: Configuration File
Edit the `config.json` file directly:
```json
{
  "Multiplier": 3,
  "EasierFishing": false,
  "FishDifficultyMultiplier": 0.5,
  "FishDifficultyAdditive": -20.0,
  "LargerFishingBar": true,
  "FishingBarHeight": 400,
  "SlowerFishMovement": false,
  "FishMovementSpeedMultiplier": 0.7,
  "AlwaysPerfect": false,
  "AlwaysFindTreasure": false,
  "InstantCatchFish": false,
  "InstantCatchTreasure": false,
  "InfiniteTackle": true,
  "InfiniteBait": true,
  "AutoHook": false,
  "TimeRateMultiplier": 1.0,
  "ReloadKey": "F5",
  "ConfigMenuKey": "K"
}
```

## Hotkeys

- **F5**: Reload configuration file
- **K**: Open custom configuration menu

## Usage Recommendations

### New Players
- Recommended resource multiplier: 3-5x
- Enable "Easier Fishing" and "Larger Bar"
- Turn on "Infinite Bait" and "Infinite Tackle"

### Experienced Players
- Adjust resource multiplier as needed
- Use time control features to manage game pace
- Enable convenience features as desired

### Challenge Mode
- Disable all convenience features
- Use normal or higher fishing difficulty
- Keep normal time rate

## Compatibility

### Tested Compatible Mods
- Generic Mod Config Menu (GMCM)
- Most resource-related mods
- Most fishing-related mods

### Known Issues
- May conflict with some mods that modify fishing mechanics
- Recommended to backup saves before use

## Troubleshooting

### Common Issues

**Q: Mod not working?**
A: Check if SMAPI is properly installed and view SMAPI logs to confirm the mod is loaded.

**Q: Fishing features abnormal?**
A: Check for conflicts with other fishing-related mods and try disabling other fishing mods.

**Q: Configuration menu won't open?**
A: Confirm you're pressing the `K` key and check for key conflicts.

**Q: Resource multiplier not working?**
A: Confirm the item is in the supported list and check the multiplier setting in the config file.

### Log Files
If you encounter issues, please check the SMAPI log files:
- Windows: `%APPDATA%/StardewValley/ErrorLogs/`
- macOS: `~/.config/StardewValley/ErrorLogs/`
- Linux: `~/.config/StardewValley/ErrorLogs/`

### Item ID Logging
The mod now includes comprehensive item ID logging functionality:
- **Item ID Logging**: Logs all items added to inventory with their IDs and types
- **Resource Multiplier Logging**: Detailed logs when resource items are detected and multiplied
- **Fishing Logging**: Logs fishing activities and caught items
- **Debug Information**: Additional debug logs for fishing mechanics and configuration changes

Log entries include item names, IDs, stack sizes, and special flags for resource/fish items.

## Changelog

### v1.0.1
- Added comprehensive item ID logging functionality
- Enhanced resource multiplier logging
- Added fishing activity logging
- Improved debug information for configuration changes

### v1.0.0
- Initial release
- Basic resource multiplier functionality
- Fishing enhancement features
- Time control system
- Multi-language support (English/Chinese)

## Contributing

Welcome to submit bug reports and feature suggestions!

## License

This project is licensed under the MIT License.

## Acknowledgments

Thanks to ConcernedApe for creating such an excellent game, and the SMAPI team for providing the mod development framework.

---

**Note**: Using mods may affect game balance, it's recommended to adjust settings according to personal preferences. Please backup saves regularly. 