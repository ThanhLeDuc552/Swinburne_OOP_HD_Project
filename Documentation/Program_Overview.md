# FireBoy and WaterGirl Game - Program Overview

## Project Summary

This is a C# implementation of a **FireBoy and WaterGirl** puzzle platformer game built using the **SplashKit** game framework and **DotTiled** for level loading. The game features two playable characters who must work together to complete levels while avoiding hazards, collecting diamonds, and reaching their respective exit doors.

### Technical Stack
- **Language**: C# (.NET 8.0)
- **Game Framework**: SplashKit SDK
- **Level Editor**: Tiled Map Editor (TMX files)
- **Dependencies**: DotTiled (0.3.0) for TMX parsing

---

## Architecture Overview

The program follows **Object-Oriented Programming** principles with clear separation of concerns:

- **Character System**: Abstract base class with concrete implementations
- **Game Object Hierarchy**: Base classes for interactive and non-interactive objects
- **Factory Pattern**: For dynamic object creation from level data
- **Physics Engine**: Static class handling movement and collision detection
- **Level Management**: TMX file parsing and game state management
- **Collision System**: Centralized collision detection and response

---

## Detailed Class Documentation

### 📁 **Core Game Classes**

#### `Program.cs`
**Purpose**: Application entry point and main game loop
- **Key Responsibilities**:
  - Window creation (624x464 resolution)
  - Level loading from TMX files
  - Character instantiation and positioning
  - Main game loop execution with update/render cycle
  - End game state handling (Game Over/Level Complete)
- **Game Loop Flow**:
  1. Process input events
  2. Update character physics and animations
  3. Check all collision types (hazards, doors, diamonds)
  4. Render all game objects
  5. Handle win/lose conditions

#### `GameConstants.cs`
**Purpose**: Central configuration for game physics and rendering
- **Key Constants**:
  - `SPRITE_SCALE`: 0.1f (10% of original sprite size)
  - `GRAVITY_STRENGTH`: 0.001f (downward acceleration)
  - `JUMP_FORCE`: -0.31f (upward velocity for jumping)
  - `MOVE_SPEED`: 0.1f (horizontal movement speed)
  - `FRICTION`: 0.85f (deceleration factor)
  - `TERMINAL_VELOCITY`: 15.0f (maximum falling speed)
  - `TILE_SIZE`: 16.0 pixels

### 📁 **Character System**

#### `Character.cs` (Abstract Base Class)
**Purpose**: Base class for all playable characters with common functionality
- **Key Features**:
  - **Animation System**: Multiple `ActionResource` objects for different states (idle, move left/right, jump, fall)
  - **Physics Properties**: Position, velocity, grounded state, collision dimensions
  - **Collision Detection**: AABB bounding box calculation with hair height consideration
  - **Sprite Management**: Dynamic sprite offset calculation for consistent foot positioning
  - **Input Abstraction**: Abstract methods for input handling (implemented by derived classes)
- **Animation Logic**:
  - Automatically switches between animations based on physics state
  - Considers horizontal input during airborne movement
  - Handles sprite alignment for consistent character positioning
- **Collision System**: 
  - Uses adjusted bounding box (excludes hair height)
  - Calculates half-width/height for precise collision detection

#### `FireBoy.cs`
**Purpose**: Concrete implementation of the fire character
- **Controls**: WASD keys (A=left, D=right, W=jump)
- **Element Type**: "fire" (immune to lava hazards)
- **Resource Bundle**: "fireboy_movement.txt"
- **Unique Traits**: Can safely traverse lava but dies in water

#### `WaterGirl.cs`
**Purpose**: Concrete implementation of the water character
- **Controls**: Arrow keys (Left/Right arrows, Up=jump)
- **Element Type**: "water" (immune to water hazards)
- **Resource Bundle**: "watergirl_movement.txt"
- **Unique Traits**: Can safely traverse water but dies in lava

### 📁 **Physics Engine**

#### `Physics.cs` (Static Class)
**Purpose**: Handles all character movement, gravity, and collision resolution
- **Core Systems**:
  - **Gravity Application**: Continuous downward acceleration with terminal velocity
  - **Jump Mechanics**: Ground-state validation and velocity-based jumping
  - **Platform Collision**: Tile-based collision detection with directional resolution
  - **Movement Integration**: Smooth movement with collision-aware position updates
- **Collision Resolution Algorithm**:
  1. Calculate tile grid coordinates from character AABB
  2. Check each tile in the character's vicinity
  3. Determine collision direction using minimum overlap calculation
  4. Apply positional correction and velocity adjustment
  5. Update grounded state and jump availability
- **Advanced Features**:
  - **Safe Distance Calculation**: Prevents clipping through walls
  - **Predictive Collision**: Tests movement before applying position changes
  - **Directional Velocity Stopping**: Stops movement only in collision direction

### 📁 **Game Object Hierarchy**

#### `GameObject.cs` (Abstract Base Class)
**Purpose**: Foundation for all objects in the game world
- **Core Properties**: Position, Name
- **Abstract Methods**: Draw(), Update()
- **Design Pattern**: Template method pattern for consistent object behavior

#### `InteractableObject.cs` (Abstract Class, extends GameObject)
**Purpose**: Base for objects that characters can interact with
- **Interaction System**:
  - `CanInteract()`: Determines if interaction is possible
  - `IsCharacterInRange()`: Checks proximity for interaction
  - `Interact()`: Handles interaction logic
- **State Management**: Activation status and range tracking
- **Event Hooks**: OnEnterRange() and OnExitRange() for derived classes

### 📁 **Hazard System**

#### `Hazards.cs` (Abstract Class, extends InteractableObject)
**Purpose**: Base class for all environmental dangers
- **Animation System**: Liquid animation with 15-frame cycles
- **Immunity Logic**: Characters are immune to hazards of their own element
- **Collision Detection**: Sprite-based AABB intersection
- **Resource Management**: Automatic sprite scaling and animation handling
- **Derived Classes**:
  - `LavaHazard`: Deadly to WaterGirl, safe for FireBoy
  - `WaterHazard`: Deadly to FireBoy, safe for WaterGirl  
  - `MudHazard`: Deadly to both characters

### 📁 **Exit Door System**

#### `ExitDoor.cs` (Abstract Class, extends InteractableObject)
**Purpose**: Base class for level completion mechanisms
- **Dual State System**:
  - **Interacted State**: Door opens when correct character enters
  - **Not Interacted State**: Default closed state
- **Animation Management**: Separate sprites for open/closed states
- **Proximity Detection**: Automatic activation/deactivation based on character presence
- **Event Callbacks**: `OnDoorOpened()` and `OnDoorClosed()` for specialized behavior
- **Derived Classes**:
  - `FireExitDoor`: Only activated by FireBoy
  - `WaterExitDoor`: Only activated by WaterGirl

### 📁 **Collectible System**

#### `Diamond.cs` (Base Class, extends InteractableObject)
**Purpose**: Collectible items with element-specific collection rules
- **Animation System**: 40-frame diamond animation cycle
- **Collection Logic**: 
  - Element-specific diamonds can only be collected by matching character
  - Special diamonds can be collected by any character
- **Visual Feedback**: Different sprite sizes for normal vs. special diamonds
- **Automatic Removal**: Handled by collision manager upon collection
- **Derived Classes**:
  - `BlueDiamond`: Collectible by WaterGirl only
  - `RedDiamond`: Collectible by FireBoy only
  - `MudDiamond`: Special diamond collectible by both

### 📁 **Factory Pattern Implementation**

#### `IGameObjectFactory.cs`
**Purpose**: Interface defining the factory contract
- **Method**: `CreateGameObject(string name, Point2D position, DotTiled.Object objData)`
- **Design Pattern**: Abstract Factory for extensible object creation

#### `GameObjectFactoryManager.cs`
**Purpose**: Central registry for all object factories
- **Registered Factories**:
  - `HazardFactory`: Creates hazard objects
  - `ExitDoorFactory`: Creates exit door objects
  - `DiamondFactory`: Creates diamond objects
- **Usage**: Level class uses this to create objects from TMX data

#### `DiamondFactory.cs` (Example Implementation)
**Purpose**: Factory for creating diamond objects
- **Creation Strategy**: Dictionary-based mapping of names to creation functions
- **Supported Types**: BlueDiamond, MudDiamond, RedDiamond
- **Error Handling**: Throws exceptions for unknown diamond types

### 📁 **Level Management**

#### `Level.cs`
**Purpose**: Central level management and TMX file processing
- **TMX Integration**:
  - Parses Tiled map files using DotTiled library
  - Processes multiple layer types (Object, Tile, Image)
  - Extracts tileset information for tile rendering
- **Object Management**:
  - Maintains lists of hazards, exit doors, diamonds
  - Handles character starting positions
  - Manages game state (game over, level completion)
- **Rendering Pipeline**:
  1. Background image layer
  2. Tile layer with rotation support
  3. Object layer (hazards, doors, diamonds)
- **Game State Logic**:
  - Tracks individual exit door activation
  - Requires both characters to reach exits for completion
  - Handles immediate game over on hazard collision

#### `LevelManager.cs`
**Purpose**: Would handle multiple levels and progression (if implemented)

### 📁 **Collision Management**

#### `CollisionManager.cs` (Static Class)
**Purpose**: Centralized collision detection and response system
- **Hazard Collision System**:
  - Checks character immunity based on element types
  - Immediately triggers game over for deadly collisions
  - Provides console feedback for collision events
- **Exit Door Management**:
  - Handles activation/deactivation based on character proximity
  - Tracks completion state for both characters
  - Manages win condition logic
- **Diamond Collection**:
  - Validates collection eligibility based on character type
  - Safely removes collected diamonds from the level
  - Prevents collection of incompatible diamond types

### 📁 **Resource Management**

#### `ActionResource.cs` (Struct)
**Purpose**: Container for sprite animation resources
- **Components**:
  - `Bitmap`: The sprite image
  - `AnimationScript`: Animation timing and frame data
  - `Animation`: Runtime animation state
  - `Sprite`: SplashKit sprite object with position and scale
  - `DrawingOptions`: Rendering options for the sprite
- **Usage**: Used extensively in Character and game object classes

#### `StartPos.cs`
**Purpose**: Manages character starting positions from level data
- **Functionality**:
  - Stores element type and position coordinates
  - Automatically positions characters based on their element type
  - Integrates with TMX object layer for level design flexibility

---

## Game Flow and Mechanics

### Core Gameplay Loop
1. **Character Input**: Process WASD (FireBoy) and Arrow Keys (WaterGirl)
2. **Physics Update**: Apply gravity, handle jumping, process movement
3. **Collision Detection**: Check platform collisions and resolve positions
4. **Interaction Checks**: Test for hazard collisions, door interactions, diamond collection
5. **Rendering**: Draw background, tiles, objects, and characters
6. **State Management**: Check win/lose conditions

### Winning Conditions
- Both FireBoy and WaterGirl must reach their respective exit doors
- Doors activate when the correct character enters the area
- Doors deactivate if the character leaves the area
- Level completes only when both doors are simultaneously active

### Losing Conditions
- Any character touches a hazard they're not immune to
- Game immediately ends with "GAME OVER" message

### Character Mechanics
- **Elemental Immunity**: Characters are immune to hazards of their own element
- **Cooperative Gameplay**: Both characters must survive and reach exits
- **Physics-Based Movement**: Realistic gravity, jumping, and momentum
- **Precision Controls**: Designed for puzzle-solving and careful navigation

---

## Design Patterns Used

1. **Template Method Pattern**: GameObject hierarchy with abstract Draw/Update methods
2. **Abstract Factory Pattern**: GameObjectFactoryManager with specific factory implementations
3. **Strategy Pattern**: Character input handling through abstract methods
4. **Observer Pattern**: Collision system with centralized event handling
5. **State Pattern**: Door animation states and character action states
6. **Facade Pattern**: Physics class providing simplified interface to complex collision logic

---

## Technical Highlights

- **Precise Collision Detection**: AABB-based system with minimum overlap resolution
- **Smooth Animation System**: Frame-based animations with proper sprite alignment
- **Modular Architecture**: Easy to extend with new character types, hazards, or objects
- **Level Editor Integration**: Direct TMX file support for level design
- **Resource Management**: Efficient sprite and animation resource handling
- **Cross-Platform Compatibility**: Built on SplashKit for multi-platform support

This architecture provides a solid foundation for a puzzle platformer game with room for expansion and modification while maintaining clean, maintainable code structure. 