# LevelLoader Class Documentation

## Overview
The `LevelLoader` class is a utility class designed to handle all TMX (Tiled Map Editor) file loading and parsing responsibilities, separating these concerns from the main `Level` class gameplay logic.

## Key Responsibilities

### 1. **TMX File Loading**
- Loads TMX map files using DotTiled library
- Handles file path resolution and error handling
- Provides static methods for easy access

### 2. **Level Data Extraction**
- Extracts map properties (width, height, tile dimensions)
- Separates different layer types (ObjectLayer, TileLayer, ImageLayer)
- Extracts tile data and flipping flags
- Loads background image paths

### 3. **Game Object Creation**
- Parses object layer and creates appropriate game objects
- Uses factory pattern to create objects based on TMX object types
- Populates ObjectManager instances with created objects
- Handles character start position setup

### 4. **Tile Management**
- Loads tileset bitmaps from TMX file
- Creates bitmap dictionary with proper GID mapping
- Sets up tile cell details for SplashKit rendering

### 5. **Level Validation**
- Validates TMX files have required layers
- Checks for essential game objects (character start positions)
- Provides detailed error messages for missing components

### 6. **Object Manager Setup**
- Creates and initializes all ObjectManager instances
- Sets up CompositeObjectManager with proper registration
- Adds default characters (FireBoy, WaterGirl)

## Benefits of Using LevelLoader

### **Separation of Concerns**
- **Before**: Level class handled both file loading AND gameplay logic
- **After**: LevelLoader handles file operations, Level class focuses on gameplay

### **Reusability**
- LevelLoader can be used by different level types
- Can easily load multiple levels or switch between levels
- Factory methods can be reused across different game modes

### **Maintainability**
- Changes to TMX file format only affect LevelLoader
- Easier to debug loading issues vs gameplay issues
- Clear responsibility boundaries

### **Testability**
- Can test level loading separately from gameplay
- Can mock level data for gameplay testing
- Validation logic can be tested independently

### **Performance**
- Level data can be cached or preloaded
- Object creation can be optimized in one place
- Memory management is centralized

## What Should Move FROM Level Class TO LevelLoader

### ✅ **Already Moved**
1. **TMX File Loading**: `Loader.LoadMap()` calls
2. **Layer Extraction**: Finding ObjectLayer, TileLayer, ImageLayer
3. **Map Property Extraction**: Width, height, tile dimensions
4. **Tile Loading**: ProcessTile() method functionality
5. **Object Creation**: ProcessObject() method functionality
6. **Bitmap Management**: Tile bitmap loading and setup
7. **Object Manager Initialization**: Creating and setting up managers

### 🔄 **Could Be Moved (Optional)**
1. **Background Loading**: Currently in Level constructor
2. **Rotation Options Setup**: Standard configuration
3. **Physics System Initialization**: Could be factory-created
4. **Collision Manager Setup**: Could be standardized

### ❌ **Should Stay in Level Class**
1. **Game State**: _gameOver, _levelCompleted, etc.
2. **Update Logic**: Character updates, collision checking
3. **Draw Logic**: Rendering calls and drawing order
4. **Score Calculation**: Gameplay-specific logic
5. **Timer Management**: Level-specific timing
6. **Collision Detection**: Game runtime logic

## Usage Example

```csharp
// Old way (everything in Level constructor)
public Level(string tmxFilePath)
{
    // 50+ lines of loading, parsing, and setup code
    // Mixed with gameplay initialization
}

// New way (using LevelLoader)
public Level(string tmxFilePath)
{
    // Load and validate
    Map map = LevelLoader.LoadMap(tmxFilePath);
    var validation = LevelLoader.ValidateLevel(map);
    if (!validation.IsValid) throw new Exception("Invalid level");
    
    // Extract data
    _levelData = LevelLoader.ExtractLevelData(map);
    
    // Create objects
    var managers = LevelLoader.CreateObjectManagers();
    LevelLoader.CreateGameObjects(_levelData.ObjectLayer, managers...);
    
    // Focus on gameplay setup
    InitializeGameSystems();
    StartLevelTimer();
}
```

## Class Structure

```
LevelLoader (static utility)
├── LoadMap() - Basic TMX loading
├── ExtractLevelData() - Parse map into LevelData object
├── CreateGameObjects() - Create objects from TMX data
├── CreateObjectManagers() - Set up all managers
├── ValidateLevel() - Ensure level integrity
└── Helper methods for tiles, rotation, etc.

LevelData (data container)
├── Map properties (width, height, etc.)
├── Layer references
├── Tile data and bitmaps
└── Background information

LevelValidationResult (validation result)
├── IsValid flag
└── Error messages list
```

## Best Practices

1. **Use LevelLoader for all TMX-related operations**
2. **Keep Level class focused on gameplay logic**
3. **Validate levels before using them**
4. **Cache LevelData for performance if needed**
5. **Use ObjectManagers instead of raw lists**
6. **Let LevelLoader handle object creation consistency**

## Future Enhancements

1. **Level Caching**: Cache parsed LevelData objects
2. **Async Loading**: Load levels in background
3. **Multiple Tilesets**: Support more than just "Bricks"
4. **Custom Properties**: Parse custom TMX properties
5. **Level Streaming**: Load/unload level sections dynamically
6. **Error Recovery**: Handle corrupted TMX files gracefully
