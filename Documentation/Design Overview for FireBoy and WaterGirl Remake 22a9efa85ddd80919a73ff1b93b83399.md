# Design Overview for FireBoy and WaterGirl Remake

Name : Le Duc Thanh

Student ID : 105548505

## Summary of Program

The FireBoy and WaterGirl Remake is a C# puzzle platformer game built using the SplashKit game framework and DotTiled for level loading. The game features two playable characters (FireBoy and WaterGirl) who must work together to complete levels while avoiding hazards, collecting diamonds, and reaching their respective exit doors.

### Underlying Mechanism

The program follows a modular, object-oriented architecture with clear separation of concerns:

- **Game Loop**: The main game loop handles input processing, physics updates, collision detection, and rendering in a structured sequence
- **Physics Engine**: A static Physics class manages gravity, movement, jumping, and collision resolution using AABB (Axis-Aligned Bounding Box) collision detection
- **Level Management**: TMX files are parsed using DotTiled to create levels with tiles, objects, and character starting positions
- **Factory Pattern**: Dynamic object creation from level data using factory classes for different game object types
- **Collision System**: Centralized collision detection and response through the CollisionManager class

### How the Game Works

1. **Character Input**: Players control FireBoy (WASD) and WaterGirl (Arrow Keys) simultaneously
2. **Physics Simulation**: Characters are affected by gravity, can jump when grounded, and have momentum-based movement
3. **Collision Detection**: Characters interact with platforms, hazards, doors, diamonds, and other objects
4. **Elemental Mechanics**: Characters have immunity to hazards of their own element (FireBoy immune to lava, WaterGirl immune to water)
5. **Cooperative Gameplay**: Both characters must reach their respective exit doors to complete a level
6. **Score System**: Players are scored based on completion time and diamond collection

## D-Level Criteria Fulfilled

### Abstraction — Custom Classes that Model the Domain

The program demonstrates strong abstraction through custom classes that model the game domain:

- **GameObject**: Abstract base class for all game objects with common properties (Position, Name, Type) and abstract methods (Draw, Update, GetAABB, ClearResource)
- **SolidObject**: Abstract class extending GameObject for objects with physics properties (Velocity, IsGrounded)
- **InteractableObject**: Abstract class for objects that characters can interact with, providing interaction logic and range checking
- **Character**: Abstract base class for playable characters with movement, animation, and physics capabilities
- **Level**: Comprehensive class managing level data, game objects, and game state

### Inheritance and Polymorphism

The program extensively uses inheritance and polymorphism:

- **Character Hierarchy**: FireBoy and WaterGirl inherit from Character, implementing specific movement and interaction logic
- **Hazard System**: LavaHazard, WaterHazard, and MudHazard inherit from Hazards, each with different immunity rules
- **Exit Door System**: FireExitDoor and WaterExitDoor inherit from ExitDoor, implementing character-specific activation
- **Diamond System**: BlueDiamond, RedDiamond, and MudDiamond inherit from Diamond, each collectible by specific characters
- **Platform System**: PlatformHorizontal and PlatformVertical inherit from Platform with different movement patterns

## HD-Level Criteria Fulfilled

### Design Patterns Used

1. **Abstract Factory Pattern**
    - **Where**: GameObjectFactoryManager, IGameObjectFactory, and all *Factory classes
    - **Why**: Dynamically creates game objects (Hazards, Doors, Diamonds, Platforms, etc.) from level data without knowing their concrete types.
    - **Benefit**: Extensible and decoupled object creation.
2. **Template Method Pattern**
    - **Where**: GameObject and its subclasses
    - **Why**: Enforces a consistent interface for update/draw logic, with specific implementations in derived classes.
3. **Strategy Pattern**
    - **Where**: Character input and movement logic (Character, FireBoy, WaterGirl)
    - **Why**: Allows different movement and input strategies for each character.
4. **Singleton Pattern**
    - **Where**: CollisionManager
    - **Why**: Ensures a single, centralized collision management instance.
5. **Composite Pattern**
    - **Where**: ObjectManager<T>, CompositeObjectManager
    - **Why**: Allows unified management and bulk operations on collections of game objects.
6. **Observer Pattern (Planned)**
    - **Where**: IGameEventObserver (for future event-driven UI, sound, analytics)
    - **Why**: Decouples event producers from consumers.
7. **Adapter Pattern**
    - **Where**: Integration of DotTiled map data into the game’s object model (LevelLoader)
    - **Why**: Adapts external TMX data to internal game objects.

### Complexity and Additional Functionalities

1. **Advanced Physics System**
    - Predictive collision detection preventing tunneling through thin walls
    - Safe distance calculation for smooth movement
    - Moving platform support with character attachment
2. **Dynamic Level Loading**
    - TMX file parsing for flexible level design
    - Support for multiple object types and layers
    - Tile rotation and flipping support
3. **Score System and Level Management**
    - Time-based scoring with penalties for exceeding time limits
    - Persistent high score tracking using file I/O
    - Level completion tracking and progression
4. **Elemental Interaction System**
    - Character-specific hazard immunity
    - Element-based diamond collection rules
    - Cooperative gameplay mechanics

### Real-World Evidence

These functionalities are practical and significantly useful in modern game development:

- **Physics Systems**: Similar to Unity's Rigidbody system, providing realistic movement and collision
- **Level Editors**: TMX format is widely used in professional game development (Unity, Godot, custom engines)
- **Factory Patterns**: Standard in game engines for object creation and resource management
- **Observer Pattern**: Used in Unity's event system and Unreal Engine's delegate system
- **Score Systems**: Essential for player engagement and progression tracking in commercial games

### Why It's Not Simple Compared to D Level

The HD version includes:

- **Complex Physics**: Multi-step collision resolution vs. simple collision detection
- **Design Patterns**: Multiple patterns working together vs. basic inheritance
- **File I/O**: Persistent data management vs. in-memory only
- **Dynamic Object Creation**: Factory pattern vs. hardcoded object creation
- **Advanced Game Mechanics**: Moving platforms, pushable objects, elemental interactions vs. basic movement
- **Modular Architecture**: Clear separation of concerns vs. monolithic design

## Required Roles

### Classes

| Class | Responsibility | Type Details | Notes |
| --- | --- | --- | --- |
| **GameObject** | Abstract base for all game objects | Abstract Class | Common interface/properties |
| **SolidObject** | Physics-enabled objects | Abstract Class | Adds velocity, grounded state |
| **InteractableObject** | Objects with interaction logic | Abstract Class | Handles proximity/interaction |
| **Character** | Playable character base | Abstract Class | Movement, animation, physics |
| **FireBoy** | Fire element character | Concrete Class | WASD controls, lava immunity |
| **WaterGirl** | Water element character | Concrete Class | Arrow controls, water immunity |
| **Hazards** | Environmental dangers base | Abstract Class | Animation, immunity checking |
| **LavaHazard** | Lava hazard | Concrete Class | Deadly to WaterGirl |
| **WaterHazard** | Water hazard | Concrete Class | Deadly to FireBoy |
| **MudHazard** | Mud hazard | Concrete Class | Deadly to both |
| **ExitDoor** | Level completion mechanism | Abstract Class | Activation logic, animation |
| **FireExitDoor** | Fire character exit | Concrete Class | Activated by FireBoy |
| **WaterExitDoor** | Water character exit | Concrete Class | Activated by WaterGirl |
| **Diamond** | Collectible base | Abstract Class | Animation, collection logic |
| **BlueDiamond** | Water character diamond | Concrete Class | Collectible by WaterGirl |
| **RedDiamond** | Fire character diamond | Concrete Class | Collectible by FireBoy |
| **MudDiamond** | Universal diamond | Concrete Class | Collectible by both |
| **Platform** | Moving platform base | Concrete Class | Activation, movement logic |
| **PlatformHorizontal** | Horizontal platform | Concrete Class | Left/right movement |
| **PlatformVertical** | Vertical platform | Concrete Class | Up/down movement |
| **Box** | Pushable object | Concrete Class | Gravity, collision, pushing |
| **Lever** | Interactive lever | Concrete Class | Timer, activation logic |
| **Button** | Pressure-sensitive button | Concrete Class | Character detection |
| **Level** | Level management | Concrete Class | Game state, object managers |
| **LevelLoader** | Level loading/validation | Static Class | TMX parsing, object creation |
| **ObjectManager<T>** | Manages object collections | Generic Class | Bulk operations, type safety |
| **CompositeObjectManager** | Manages multiple managers | Concrete Class | Bulk draw/update |
| **CollisionManager** | Collision detection | Singleton Class | Centralized logic |
| **CollisionResolver** | Collision resolution | Concrete Class | Handles collision responses |
| **PhysicsSystem** | Physics engine | Concrete Class | Gravity, movement, jumping |
| **GameObjectFactoryManager** | Object creation | Concrete Class | Factory pattern |
| **IGameObjectFactory** | Factory interface | Interface | Factory contract |
| **HazardFactory** | Hazard creation | Concrete Class | Creates hazard objects |
| **ExitDoorFactory** | Door creation | Concrete Class | Creates door objects |
| **DiamondFactory** | Diamond creation | Concrete Class | Creates diamond objects |
| **PlatformFactory** | Platform creation | Concrete Class | Creates platform objects |
| **LeverFactory** | Lever creation | Concrete Class | Creates lever objects |
| **ButtonFactory** | Button creation | Concrete Class | Creates button objects |
| **BoxFactory** | Box creation | Concrete Class | Creates box objects |
| **ActionResource** | Animation resources | Struct | Sprite, animation, options |
| **GameConstants** | Game configuration | Static Class | Physics constants, speeds |

### Enumerates

| Enum | Values | Notes |
| --- | --- | --- |
| **Direction** | Left, Right, Up, Down | Platform and character movement direction |
| **CollisionSide** | None, Top, Bottom, Left, Right | Side of collision for collision resolution |

### Main Responsibilities

**Game Management**: Level class manages the overall game state, object creation, and game loop coordination.

**Physics and Movement**: Physics class handles all movement calculations, gravity application, and collision resolution.

**Object Creation**: Factory classes create different types of game objects from level data, implementing the Factory pattern.

**Collision Detection**: CollisionManager centralizes all collision checking and interaction logic between characters and objects.

**Character Control**: Character classes handle input processing, movement logic, and character-specific behaviors.

**Rendering**: Each game object is responsible for its own drawing logic, following the single responsibility principle.

## Class Diagram

Due to the excessive length of the diagram, a link to Mermaid is provided below (the updated version compared to the D version of the project):

https://www.mermaidchart.com/app/projects/929ec608-d499-4dba-9331-5f257c83adcb/diagrams/0ecf8886-30ff-403e-a0af-2eb5b4a51458/version/v0.1/edit

- Classes’ internal contents are reduced down for clearer illustration of the diagram

## Sequence Diagram

![Untitled diagram _ Mermaid Chart-2025-07-12-104806.png](Untitled_diagram___Mermaid_Chart-2025-07-12-104806.png)

- For clearer visual: https://www.mermaidchart.com/app/projects/929ec608-d499-4dba-9331-5f257c83adcb/diagrams/7d7f0d47-0737-4714-9a68-b10828707fbb/version/v0.1/edit