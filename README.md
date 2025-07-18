# Swinburne OOP HD Project - FireBoy and WaterGirl Game

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

- **State Management System**: Game states for menu, playing, and game over
- **Character System**: Abstract base class inheriting from SolidObject
- **Game Object Hierarchy**: Separate hierarchies for solid and interactable objects
- **Factory Pattern**: For dynamic object creation from level data
- **Physics Engine**: Static class handling movement and collision detection
- **Level Management**: TMX file parsing and game state management
- **Collision System**: Centralized collision detection and response

---

## Core Class Structure

### 📁 **Game State Management**

#### `GameStateManager`
**Purpose**: Manages different game states and transitions
- **State Stack**: Maintains a stack of game states for easy transitions
- **Window Management**: Handles the main game window
- **Menu Integration**: Contains menu and level manager references

#### `IGameState` (Interface)
**Purpose**: Contract for all game states
- **Methods**: `Update()`, `Draw()`, `HandleInput()`

#### State Implementations
- **`MenuState`**: Handles menu navigation and level selection
- **`PlayingState`**: Manages active gameplay with pause functionality
- **`GameOverState`**: Displays end game results and scoring

### 📁 **Game Object Hierarchy**

#### Core Base Classes

#### `GameObject` (Abstract Base Class)
**Purpose**: Foundation for all objects in the game world
- **Properties**: Name, Type
- **Abstract Methods**: Draw(), Update(), GetAABB()

#### `SolidObject` (Abstract, extends GameObject)
**Purpose**: Objects with physical properties affected by physics
- **Properties**: Position, Velocity, IsGrounded
- **Used by**: Characters, Boxes, Platforms

#### `InteractableObject` (Abstract, extends GameObject)
**Purpose**: Objects that characters can interact with
- **Properties**: IsActivated, Position
- **Abstract Methods**: Interact(), CanInteract(), IsCharacterInRange()

### 📁 **Character System**

#### `Character` (Abstract, extends SolidObject)
**Purpose**: Base class for all playable characters
- **Animation System**: Multiple ActionResource objects for different states
- **Physics Integration**: Inherits position, velocity, and grounded state from SolidObject
- **Collision System**: AABB bounding box with hair height consideration
- **Input Abstraction**: Abstract methods for movement and jumping

#### Character Implementations
- **`FireBoy`**: WASD controls, fire element immunity
- **`WaterGirl`**: Arrow key controls, water element immunity

### 📁 **Interactive Objects**

#### Platform System
- **`Platform`** (Abstract): Moving platforms with direction and activation logic
- **Activation**: Controlled by buttons and levers

#### Input Objects
- **`Button`**: Pressure-activated switches
- **`Lever`** (Abstract): Timer-based activation switches

#### Environmental Hazards
- **`Hazards`** (Abstract): Base for all dangerous elements
- **Element Types**: Lava (deadly to WaterGirl), Water (deadly to FireBoy), Mud (deadly to both)

#### Collectibles
- **`Diamond`** (Abstract): Collectible gems with element restrictions
- **Special Diamonds**: Can be collected by any character

#### Exit System
- **`ExitDoor`** (Abstract): Level completion mechanisms
- **Dual State**: Open/closed animations based on character proximity
- **Element Matching**: Each door only responds to its matching character

### 📁 **Factory Pattern Implementation**

#### `GameObjectFactoryManager`
**Purpose**: Central registry for object creation
- **Factory Registration**: Dictionary of factory implementations
- **Object Creation**: Creates objects from TMX level data

#### Factory Implementations
- **`BoxFactory`**: Creates box objects
- **`DiamondFactory`**: Creates diamond collectibles
- **`DoorFactory`**: Creates exit doors
- **`HazardFactory`**: Creates environmental hazards
- **`LeverFactory`**: Creates lever switches
- **`PlatformFactory`**: Creates moving platforms

### 📁 **Physics and Collision**

#### `Physics` (Static Class)
**Purpose**: Handles all physics simulation
- **Character Updates**: Movement, gravity, jumping
- **Collision Resolution**: AABB-based collision detection
- **Movement Integration**: Velocity-based position updates

#### `CollisionManager` (Static Class)
**Purpose**: Centralized collision detection and response
- **Hazard Collisions**: Immunity checking and game over logic
- **Door Interactions**: Activation/deactivation based on proximity
- **Diamond Collection**: Element-based collection validation

### 📁 **Level Management**

#### `Level`
**Purpose**: Manages individual game levels
- **TMX Integration**: Parses Tiled map files
- **Object Management**: Contains all game objects and characters
- **Game State**: Tracks completion and game over conditions
- **Scoring**: Calculates level completion scores

#### `LevelManager`
**Purpose**: Manages multiple levels and progression
- **Level Storage**: Dictionary of loaded levels
- **Score Tracking**: High score management
- **Level Status**: Completion tracking

### 📁 **Support Classes**

#### `ActionResource` (Struct)
**Purpose**: Container for sprite animation resources
- **Components**: Bitmap, Animation, Sprite, DrawingOptions
- **Usage**: Used extensively in Character and animated objects

#### `GameConstants` (Static)
**Purpose**: Central configuration for physics and rendering
- **Physics**: Gravity, jump force, movement speed, friction
- **Rendering**: Sprite scale, tile size

#### Enumerations
- **`Direction`**: Left, Right, Up, Down (for platform movement)
- **`GameEventType`**: Event type definitions

---

## Key Design Decisions

### Inheritance Structure
The game uses a **dual inheritance hierarchy**:
1. **SolidObject**: For objects affected by physics (characters, boxes, platforms)
2. **InteractableObject**: For objects that can be interacted with (buttons, levers, hazards, diamonds, doors)

This allows for clean separation of concerns while avoiding multiple inheritance issues.

### State Management
The **GameStateManager** uses a stack-based approach for state transitions, allowing for:
- Easy pause/resume functionality
- Clean state transitions
- Modular state implementations

### Factory Pattern
The **Abstract Factory pattern** enables:
- Dynamic object creation from level data
- Easy addition of new object types
- Centralized object creation logic

---

## Game Flow

### Core Gameplay Loop
1. **Input Processing**: Handle character movement and interactions
2. **Physics Update**: Apply gravity, movement, and collision detection
3. **Interaction Checks**: Process hazard collisions, door interactions, diamond collection
4. **Rendering**: Draw all game objects with proper layering
5. **State Management**: Check win/lose conditions and handle state transitions

### Character Mechanics
- **Elemental Immunity**: Characters are immune to hazards of their element
- **Cooperative Gameplay**: Both characters must reach their respective exits
- **Physics-Based Movement**: Realistic gravity, jumping, and momentum

### Level Completion
- **Dual Exit Requirement**: Both characters must simultaneously be at their exits
- **Scoring System**: Based on completion time and diamonds collected
- **Progression Tracking**: Level completion status and high scores

---

## Technical Highlights

- **Clean Architecture**: Clear separation between solid objects and interactable objects
- **State Management**: Robust state system for menu, gameplay, and game over
- **Physics Integration**: Realistic physics simulation with AABB collision detection
- **Animation System**: Comprehensive sprite animation with state-based transitions
- **Level Editor Support**: Direct TMX file integration for level design
- **Factory Pattern**: Extensible object creation system
- **Event System**: Game event notifications for UI and scoring

This architecture provides a solid, extensible foundation for a puzzle platformer game with clear object relationships and maintainable code structure.
