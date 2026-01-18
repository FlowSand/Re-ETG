# ETG Module Manifest

**Generated:** 2026-01-18
**Tool:** analyze_module_dependencies.py + Manual Curation
**Purpose:** Complete catalog of all modules in the Enter the Gungeon codebase for AI-assisted architecture migration

---

## Table of Contents

1. [Overview](#overview)
2. [Foundation Layer - External Modules](#foundation-layer---external-modules)
3. [Core Layer - System Modules](#core-layer---system-modules)
4. [Domain Layer - Game Logic Modules](#domain-layer---game-logic-modules)
5. [Presentation Layer - UI/Audio/VFX Modules](#presentation-layer---uiaudiovfx-modules)
6. [Module Statistics](#module-statistics)

---

## Overview

The ETG codebase consists of **22 identified modules** organized into 4 architectural layers:

- **Layer 0 (Foundation)**: 13 external/third-party modules
- **Layer 1 (Core Systems)**: 2 core infrastructure modules
- **Layer 2 (Domain Logic)**: 4 game mechanics modules
- **Layer 3 (Presentation)**: 3 presentation modules

**Key Metrics:**
- Total Files: 4,009 C# files
- Module-to-Module Dependencies: 85
- Circular Dependencies: 3 (all involving Dungeonator, expected)

---

## Foundation Layer - External Modules

### Dungeonator

**Identity:**
- **Namespace:** `Dungeonator`
- **Directory:** `Assets/Scripts/ETG/Dungeonator`
- **Files:** 67
- **Category:** External (Procedural Generation Engine)
- **Priority:** ⭐ CRITICAL

**Purpose:**
Procedural dungeon generation system responsible for creating rooms, corridors, and dungeon layouts. Core spatial framework for the entire game.

**Responsibilities:**
- Dungeon layout generation (rooms, corridors, connections)
- Room template management and instantiation
- Tilemap generation and rendering
- Dungeon flow control (critical path, optional rooms, boss rooms)
- Integration with game mechanics (enemy spawns, loot placement, room events)
- Spatial queries and collision detection

**Public API (Key Classes):**
- `Dungeon` - Main dungeon container, manages rooms and flow
- `RoomHandler` - Individual room controller, handles room state and events
- `DungeonData` - Data structure for dungeon configuration
- `DungeonMaterial` - Tile materials and rendering settings
- `TK2DDungeonAssembler` - Builds dungeon geometry from templates
- `CellData` - Cell-level data structure
- `TileIndexGrid` - Grid-based tile indexing

**Dependencies:**
- System (98 files) - Collections, LINQ
- UnityEngine (35 files) - Rendering, transforms, physics
- HutongGames.PlayMaker (1 file) - Event integration
- Pathfinding (2 files) - Navigation integration
- tk2dRuntime (1 file) - Sprite rendering
- SimplexNoise (1 file) - Procedural noise

**Dependents (Modules Using Dungeonator):**
- Core.Systems (114 files) - HIGHEST usage
- Core.Dungeon (82 files)
- Core.Core (75 files)
- Core.Actors (56 files)
- Core.Items (41 files)
- Core.Combat (22 files)
- HutongGames.PlayMaker (12 files)
- Core.UI (6 files)
- Core.VFX (3 files)
- tk2dRuntime (3 files)
- Pathfinding (2 files)

**Total Dependents:** 419 files across 11 modules

**Internal Structure:**
```
Dungeonator/
├── Core generation (Dungeon.cs, DungeonData.cs)
├── Room management (RoomHandler.cs, RuntimeDungeonRoom.cs)
├── Tilesets (DungeonMaterial.cs, DungeonTileStampData.cs)
├── Flow control (DungeonFlow.cs, DungeonFlowNode.cs)
├── Builders (TK2DDungeonAssembler.cs, ChunkBuilder.cs)
└── Pathfinding (FastDungeonLayoutPathfinder.cs)
```

**Architectural Notes:**
- **Layer:** Foundation (depended on by almost all game logic)
- **Design Pattern:** Facade + Builder pattern
- **Coupling:** High (by design - central spatial system)

**Migration Guidance:**
- **Critical Module:** Essential for game functionality
- **Option A:** Port entire Dungeonator module (cleanly separated namespace)
- **Option B:** Replace with equivalent procedural generation library
- **Option C:** Keep data structures, rewrite generation algorithms
- **Dependencies to address:** tk2dRuntime (rendering) can be replaced with native sprite rendering
- **Key integration points:**
  - `IPlaceConfigurable` interface - must be preserved
  - `CellType` enum - core to all dungeon logic
  - `RoomHandler` events - many systems subscribe to room lifecycle

---

### InControl

**Identity:**
- **Namespace:** `InControl`
- **Directory:** `Assets/Scripts/ETG/InControl`
- **Files:** 391 (largest external module)
- **Category:** External (Input Management)
- **Priority:** ⭐ HIGH

**Purpose:**
Cross-platform input management system providing unified interface for keyboard, mouse, and gamepad input.

**Responsibilities:**
- Input device detection and management
- Input binding and remapping
- Cross-platform controller support
- Input event handling
- Device profile management

**Public API (Key Classes):**
- `InputManager` - Central input management
- `InputDevice` - Represents an input device
- `PlayerAction` - Bindable input actions
- `InputControl` - Individual control binding

**Dependencies:**
- System (387 files)
- UnityEngine (37 files)

**Dependents:**
- Core.Systems (22 files)
- Core.UI (11 files)

**Migration Guidance:**
- Can be replaced with Unity's new Input System
- Alternatively, keep as-is (self-contained, no major issues)
- Low migration priority (peripheral to core gameplay)

---

### FullInspector

**Identity:**
- **Namespace:** `FullInspector`
- **Directory:** `Assets/Scripts/ETG/FullInspector`
- **Files:** 188
- **Category:** External (Serialization Framework)
- **Priority:** ⭐ HIGH

**Purpose:**
Advanced serialization and inspector framework providing custom serialization beyond Unity's default.

**Responsibilities:**
- Custom object serialization
- Editor inspector enhancements
- Type metadata management
- Serialization policy enforcement

**Dependencies:**
- System (206 files)
- UnityEngine (18 files)

**Dependents:**
- Core.Systems (224 files) - VERY HIGH usage
- Core.Actors (89 files)
- Core.Combat (30 files)
- Core.Items (6 files)

**Migration Guidance:**
- Can be replaced with Unity's JsonUtility or Newtonsoft.Json
- May require refactoring serialization attributes
- Medium migration priority (used extensively but replaceable)

---

### Brave.BulletScript

**Identity:**
- **Namespace:** `Brave.BulletScript`
- **Directory:** `Assets/Scripts/ETG/Brave`
- **Files:** 14
- **Category:** External (Bullet Pattern Framework)
- **Priority:** ⭐ CRITICAL

**Purpose:**
DSL (Domain Specific Language) for defining complex bullet patterns used in boss fights and enemy attacks.

**Responsibilities:**
- Bullet pattern script parsing
- Bullet pattern execution
- Choreographed attack sequences
- Pattern timing and synchronization

**Dependencies:**
- System (16 files)
- UnityEngine (11 files)
- Dungeonator (1 file)

**Dependents:**
- Core.Systems (224 files) - VERY HIGH usage
- Core.Combat (37 files)
- Core.Core (6 files)
- Core.Dungeon (1 file)

**Migration Guidance:**
- CRITICAL for boss fight complexity
- Highly specialized DSL - difficult to replace
- Consider porting entire framework
- Alternative: rewrite boss patterns in native code (very time-consuming)

---

### Pathfinding

**Identity:**
- **Namespace:** `Pathfinding`
- **Directory:** `Assets/Scripts/ETG/Pathfinding`
- **Files:** 4
- **Category:** External (A* Pathfinding)
- **Priority:** HIGH

**Purpose:**
A* pathfinding library for enemy navigation and AI movement.

**Dependencies:**
- System (4 files)
- UnityEngine (3 files)
- Dungeonator (2 files) - Bidirectional

**Dependents:**
- Core.Actors (20 files) - Enemy AI navigation
- Dungeonator (2 files) - Dungeon pathfinding

**Migration Guidance:**
- Can be replaced with Unity NavMesh or A* Pathfinding Project
- Relatively simple to migrate (standard A* algorithm)

---

### tk2dRuntime

**Identity:**
- **Namespace:** `tk2dRuntime`
- **Directory:** `Assets/Scripts/ETG/tk2dRuntime`
- **Files:** 14
- **Category:** External (2D Sprite Toolkit)
- **Priority:** MEDIUM

**Purpose:**
2D sprite and tilemap rendering toolkit (legacy 2D solution).

**Dependencies:**
- System (16 files)
- UnityEngine (13 files)
- Dungeonator (3 files) - Bidirectional
- Core.Core (1 file)

**Dependents:**
- Core.Systems (36 files)
- Core.VFX (4 files)
- Dungeonator (1 file)
- Core.UI (1 file)

**Migration Guidance:**
- Replace with Unity's native Sprite Renderer and Tilemap
- Straightforward migration path available
- Medium priority

---

### Other External Modules (Lower Priority)

**HutongGames.PlayMaker** (External, Visual Scripting)
- Files: Not scanned separately (part of HutongGames folder)
- Purpose: Visual scripting system for behavior trees
- Usage: 10 files in Core.Systems
- Migration: Can remove if rewriting behaviors in code

**DaikonForge** (External, UI Framework)
- Purpose: Legacy UI and tweening framework
- Migration: Replace with Unity UI (uGUI)

**FullSerializer** (External, JSON Serialization)
- Files: 52
- Purpose: JSON serialization library
- Migration: Replace with Newtonsoft.Json or Unity JsonUtility

**XInputDotNetPure** (External, Xbox Controller)
- Files: 9
- Purpose: Xbox controller support
- Migration: Replace with InControl or Unity Input System

**SimplexNoise** (External, Noise Generation)
- Files: 3
- Purpose: Procedural noise generation
- Migration: Replace with Unity Mathf.PerlinNoise or custom implementation

**Kvant** (External, GPU Visual Effects)
- Purpose: GPU-accelerated effects
- Migration: Replace with Unity VFX Graph

**TestSimpleRNG** (External, RNG Utilities)
- Purpose: Random number generation
- Migration: Replace with System.Random or Unity.Mathematics

---

## Core Layer - System Modules

### Core.Systems

**Identity:**
- **Namespace:** Global (no specific namespace)
- **Directory:** `Assets/Scripts/ETG/Core/Systems`
- **Files:** 990 (LARGEST module)
- **Category:** Core Subsystem
- **Priority:** ⭐ CRITICAL

**Purpose:**
Centralized game systems including data management, game managers, and utility functions. Acts as the "catch-all" for core game functionality.

**Responsibilities:**
- Game state management
- Data persistence and loading
- Manager classes (AmmonomiconInstanceManager, ChallengeManager, etc.)
- Utility functions and helpers
- Configuration databases (AdvancedSynergyDatabase, BossManager)
- Runtime game managers

**Subsystems:**
- **Data:** Database objects, configuration assets
- **Management:** Manager classes, singleton systems
- **Utilities:** Helper functions, extension methods, utilities

**Public API (Key Classes):**
- `GameManager` - Central game state manager
- `SaveManager` - Save/load system
- `BossManager` - Boss data management
- `AdvancedSynergyDatabase` - Item synergy definitions
- `BraveUtility` - Utility helper methods

**Dependencies:**
- System (1062 files)
- UnityEngine (525 files)
- Dungeonator (114 files) - High usage
- Brave.BulletScript (224 files) - Very high usage
- FullInspector (224 files) - Very high usage
- InControl (22 files)
- tk2dRuntime (36 files)
- HutongGames.PlayMaker (10 files)

**Dependents:**
- Core.Actors (87 files)
- Core.Combat (83 files)
- Core.Items (61 files)
- Core.Dungeon (53 files)
- Core.UI (38 files)
- Core.Audio (23 files)
- Core.VFX (13 files)
- Core.Core (4 files)

**Architectural Notes:**
- **Oversized Module:** Should be split into smaller subsystems
- **Design Pattern:** Mix of Singleton, Manager, and Utility patterns
- **Refactoring Opportunity:** Separate Data, Management, and Utilities into distinct modules

**Migration Guidance:**
- **Phased approach:** Migrate subsystems independently
- **Priority order:** Data → Management → Utilities
- **Key dependencies:** Break Brave.BulletScript and FullInspector dependencies first
- **Risk:** High coupling - careful refactoring required

---

### Core.Core

**Identity:**
- **Namespace:** Global (no specific namespace)
- **Directory:** `Assets/Scripts/ETG/Core/Core`
- **Files:** 377
- **Category:** Core Subsystem
- **Priority:** ⭐ CRITICAL

**Purpose:**
Framework foundation providing base classes, enumerations, and interfaces used throughout the codebase.

**Responsibilities:**
- Base classes for game actors (GameActor, AIActor)
- Core enumerations (Achievement, GameMode, etc.)
- Interface definitions (IAutoAimTarget, etc.)
- Framework patterns and contracts

**Subsystems:**
- **Framework:** Base classes and game object foundations
- **Enums:** Global enumerations
- **Interfaces:** Contract definitions

**Public API (Key Types):**
- `GameActor` - Base class for all game entities
- `AIActor` - Base class for AI-controlled entities
- `SpeculativeRigidbody` - Physics rigidbody wrapper
- `PixelCollider` - Pixel-perfect collision
- `IAutoAimTarget` - Auto-aim interface
- Various enumerations (Achievement, DungeonFlowLevelEntry, etc.)

**Dependencies:**
- System (413 files)
- UnityEngine (213 files)
- Dungeonator (75 files) - High usage
- Pathfinding (1 file)
- tk2dRuntime (1 file)
- Brave.BulletScript (6 files)

**Dependents:**
- Core.Systems (4 files)
- Core.Actors (81 files) - High usage
- Core.Combat (66 files) - High usage
- Core.Items (58 files) - High usage
- Core.Dungeon (29 files)
- Core.Audio (10 files)
- Core.UI (9 files)
- Core.VFX (8 files)

**Architectural Notes:**
- **Layer:** Core framework (depended on by all domain logic)
- **Design Pattern:** Abstract Base Class pattern
- **Stable Module:** Few changes needed, well-defined boundaries

**Migration Guidance:**
- **Port First:** Migrate this module early (foundation for everything else)
- **Preserve API:** Keep public API signatures intact
- **Decouple Dungeonator:** May need adapter pattern to reduce coupling

---

## Domain Layer - Game Logic Modules

### Core.Actors

**Identity:**
- **Namespace:** Global
- **Directory:** `Assets/Scripts/ETG/Core/Actors`
- **Files:** 167
- **Category:** Core Subsystem
- **Priority:** ⭐ HIGH

**Purpose:**
Character and entity management system handling players, enemies, NPCs, and their behaviors.

**Responsibilities:**
- Player character control and abilities
- Enemy AI and behavior trees
- NPC interactions
- Actor lifecycle management
- Behavior system framework

**Subsystems:**
- **Player:** PlayerController, player-specific mechanics
- **Enemy:** AIActor, enemy-specific behaviors
- **Behaviors:** Behavior tree components

**Public API (Key Classes):**
- `PlayerController` - Main player character controller
- `AIActor` - Enemy AI base class
- `BehaviorBase` - Base class for AI behaviors
- `AttackBehaviorGroup` - Grouped attack behaviors

**Dependencies:**
- System (187 files)
- UnityEngine (94 files)
- Dungeonator (56 files) - High usage for navigation
- FullInspector (89 files) - Very high usage
- Pathfinding (20 files) - High usage for AI
- Core.Core (81 files) - High usage
- Core.Systems (87 files) - High usage

**Dependents:**
- Core.Items (76 files) - Items affect actors
- Core.Combat (31 files) - Combat affects actors
- Core.Dungeon (17 files) - Dungeon spawns actors
- Core.UI (13 files) - UI displays actor info

**Migration Guidance:**
- **Port with Core.Core:** Tightly coupled to framework
- **AI Behavior Trees:** May need refactoring for new engine
- **PlayerController:** Central to game feel, careful migration

---

### Core.Combat

**Identity:**
- **Namespace:** Global
- **Directory:** `Assets/Scripts/ETG/Core/Combat`
- **Files:** 172
- **Category:** Core Subsystem
- **Priority:** ⭐ HIGH

**Purpose:**
Combat mechanics including projectiles, damage calculation, and status effects.

**Responsibilities:**
- Projectile spawning and management
- Damage calculations and knockback
- Status effects and buffs/debuffs
- Projectile modifiers and synergies
- Reload mechanics

**Subsystems:**
- **Projectiles:** Projectile types and behaviors
- **Damage:** Damage calculation and application
- **Effects:** Status effects and visual feedback

**Public API (Key Classes):**
- `Projectile` - Base projectile class
- `ProjectileModule` - Projectile configuration
- `ArcProjectile` - Arc-trajectory projectiles
- `DamageTypeModifier` - Damage type system
- `SynergyProcessor` - Synergy effect processing

**Dependencies:**
- System (189 files)
- UnityEngine (97 files)
- Brave.BulletScript (37 files) - High usage for boss patterns
- Dungeonator (22 files)
- FullInspector (30 files)
- Core.Core (66 files)
- Core.Systems (83 files)

**Dependents:**
- Core.Items (36 files) - Guns create projectiles
- Core.Actors (31 files) - Actors affected by combat
- Core.Dungeon (5 files) - Environmental damage

**Migration Guidance:**
- **Preserve Bullet Patterns:** Brave.BulletScript dependency is critical
- **Physics Integration:** May need adapter for new physics engine
- **Synergy System:** Complex - port carefully

---

### Core.Items

**Identity:**
- **Namespace:** Global
- **Directory:** `Assets/Scripts/ETG/Core/Items`
- **Files:** 175
- **Category:** Core Subsystem
- **Priority:** HIGH

**Purpose:**
Equipment, weapons, and pickup systems managing all player inventory.

**Responsibilities:**
- Gun definitions and stats
- Active items (cooldown abilities)
- Passive items (stat modifiers)
- Pickup spawning and collection
- Item synergy management

**Subsystems:**
- **Guns:** Weapon definitions
- **Active:** Active items with cooldowns
- **Passive:** Passive stat modifiers
- **Pickups:** Collectible items

**Public API (Key Classes):**
- `Gun` - Base gun class
- `PlayerItem` - Base class for active items
- `PassiveItem` - Base class for passive items
- `PickupObject` - Base class for pickups
- `GunFormeData` - Gun transformation data

**Dependencies:**
- System (191 files)
- UnityEngine (97 files)
- Dungeonator (41 files)
- Core.Core (58 files)
- Core.Systems (61 files)
- Core.Actors (76 files) - High usage
- Core.Combat (36 files)

**Dependents:**
- Core.UI (22 files) - Inventory display
- Core.Dungeon (13 files) - Item spawning

**Migration Guidance:**
- **Data-Driven:** Mostly data, easy to port
- **Preserve Stats:** Keep item balance intact
- **Synergy System:** Depends on Core.Combat and Core.Systems

---

### Core.Dungeon

**Identity:**
- **Namespace:** Global
- **Directory:** `Assets/Scripts/ETG/Core/Dungeon`
- **Files:** 125
- **Category:** Core Subsystem
- **Priority:** HIGH

**Purpose:**
Room management, interactable objects, and dungeon-specific game mechanics.

**Responsibilities:**
- Room controller implementations
- Shrine and NPC interactions
- Boss room controllers
- Special encounter management
- Room event handling

**Subsystems:**
- **Generation:** Room generation logic
- **Rooms:** Room-specific controllers
- **Interactables:** Interactive objects (shrines, chests, NPCs)

**Public API (Key Classes):**
- `BossFinalRoomController` - Boss fight room logic
- `Chest` - Chest interactable
- `ShrineController` - Shrine interactions
- `ForgeHammerController` - Forge mechanics

**Dependencies:**
- System (134 files)
- UnityEngine (70 files)
- Dungeonator (82 files) - Very high usage
- Core.Systems (53 files)
- Core.Core (29 files)
- Core.Actors (17 files)
- Core.Items (13 files)
- HutongGames.PlayMaker (7 files)

**Dependents:**
- Core.Combat (5 files)
- Core.VFX (2 files)

**Migration Guidance:**
- **Tight Dungeonator Coupling:** Difficult to separate
- **Boss Room Logic:** Complex state machines
- **Port with Dungeonator:** Recommend migrating together

---

## Presentation Layer - UI/Audio/VFX Modules

### Core.UI

**Identity:**
- **Namespace:** Global
- **Directory:** `Assets/Scripts/ETG/Core/UI`
- **Files:** 100
- **Category:** Core Subsystem
- **Priority:** MEDIUM

**Purpose:**
User interface systems including HUD, menus, and in-game UI.

**Responsibilities:**
- HUD rendering
- Menu systems
- Ammonomicon (item database UI)
- Tooltip display
- UI input handling

**Subsystems:**
- **HUD:** Heads-up display elements
- **Ammonomicon:** Item/enemy database UI

**Dependencies:**
- System (108 files)
- UnityEngine (55 files)
- InControl (11 files) - Input display
- Core.Systems (38 files)
- Core.Actors (13 files)
- Core.Items (22 files)
- Dungeonator (6 files)
- Core.Core (9 files)

**Migration Guidance:**
- **Replace UI Framework:** Use Unity UI (uGUI)
- **Decouple InControl:** Use new input system
- **Independent Module:** Can be migrated last

---

### Core.Audio

**Identity:**
- **Namespace:** Global
- **Directory:** `Assets/Scripts/ETG/Core/Audio`
- **Files:** 150
- **Category:** Core Subsystem
- **Priority:** MEDIUM

**Purpose:**
Sound and music management including Wwise audio middleware integration.

**Responsibilities:**
- Audio event triggering
- Music system management
- Sound effect playback
- Wwise integration

**Subsystems:**
- **Integration:** Wwise audio middleware integration

**Dependencies:**
- System (167 files)
- UnityEngine (84 files)
- Core.Systems (23 files)
- Core.Core (10 files)

**Migration Guidance:**
- **Wwise Dependency:** Replace with Unity Audio or other middleware
- **Audio Events:** May need event system refactoring
- **Low Coupling:** Relatively easy to migrate

---

### Core.VFX

**Identity:**
- **Namespace:** Global
- **Directory:** `Assets/Scripts/ETG/Core/VFX`
- **Files:** 67 (smallest core module)
- **Category:** Core Subsystem
- **Priority:** LOW

**Purpose:**
Visual effects and animation systems for particles and special effects.

**Responsibilities:**
- Particle system management
- Visual effect spawning
- Animation control
- Effect pooling

**Subsystems:**
- **Animation:** Animation-based effects

**Dependencies:**
- System (74 files)
- UnityEngine (40 files)
- tk2dRuntime (4 files)
- Core.Systems (13 files)
- Core.Core (8 files)
- Dungeonator (3 files)

**Dependents:**
- Core.Dungeon (2 files)

**Migration Guidance:**
- **Replace VFX:** Use Unity VFX Graph or Particle System
- **Isolated Module:** Can be migrated independently
- **Low Priority:** Minimal dependencies

---

## Module Statistics

### Files Per Module

| Module | Files | % of Total |
|--------|-------|------------|
| Core.Systems | 990 | 24.7% |
| InControl | 391 | 9.8% |
| Core.Core | 377 | 9.4% |
| FullInspector | 188 | 4.7% |
| Core.Items | 175 | 4.4% |
| Core.Combat | 172 | 4.3% |
| Core.Actors | 167 | 4.2% |
| Core.Audio | 150 | 3.7% |
| Core.Dungeon | 125 | 3.1% |
| Core.UI | 100 | 2.5% |
| Core.VFX | 67 | 1.7% |
| Dungeonator | 67 | 1.7% |
| FullSerializer | 52 | 1.3% |
| DaikonForge | 61 | 1.5% |
| tk2dRuntime | 14 | 0.3% |
| Brave.BulletScript | 14 | 0.3% |
| XInputDotNetPure | 9 | 0.2% |
| Pathfinding | 4 | 0.1% |
| SimplexNoise | 3 | 0.1% |
| Other | ~40 | 1.0% |

### Dependency Counts

**Most Depended Upon:**
1. Dungeonator: 419 files across 11 modules ⭐⭐⭐
2. System (C#/.NET): All modules
3. UnityEngine: All modules
4. Core.Systems: 315 files across 7 modules
5. Core.Core: 265 files across 7 modules

**Highest Outgoing Dependencies:**
1. Core.Systems: 356+ dependencies
2. Core.Actors: 165+ dependencies
3. Core.Combat: 92+ dependencies

**Most Isolated (Fewest Dependencies):**
1. Core.VFX: 8 total dependencies
2. Core.UI: 17 dependencies
3. Core.Audio: 23 dependencies

### Priority Classification

**CRITICAL Modules (Must Port Early):**
- Dungeonator (foundation for spatial logic)
- Brave.BulletScript (boss patterns)
- Core.Systems (game state)
- Core.Core (framework base classes)

**HIGH Priority:**
- Core.Actors (player/enemy logic)
- Core.Combat (combat mechanics)
- Core.Items (inventory system)
- Core.Dungeon (room logic)
- InControl (input)
- FullInspector (serialization)
- Pathfinding (AI navigation)

**MEDIUM Priority:**
- Core.UI (user interface)
- Core.Audio (sound)
- tk2dRuntime (2D rendering)

**LOW Priority:**
- Core.VFX (visual effects)
- Other utility modules

---

## Migration Strategy Summary

### Phase 1: Foundation
1. Core.Core (framework base)
2. Dungeonator (spatial system)
3. Core.Systems (split into subsystems first)

### Phase 2: Core Gameplay
4. Brave.BulletScript (bullet patterns)
5. Core.Combat (projectiles, damage)
6. Core.Actors (player, enemies)
7. Core.Items (equipment)
8. Pathfinding (AI navigation)

### Phase 3: Integration
9. Core.Dungeon (room logic)
10. Core.Audio (sound)
11. Core.UI (interface)

### Phase 4: Polish
12. Core.VFX (effects)
13. Replace external dependencies (InControl, FullInspector, tk2dRuntime)

---

**End of Module Manifest**
