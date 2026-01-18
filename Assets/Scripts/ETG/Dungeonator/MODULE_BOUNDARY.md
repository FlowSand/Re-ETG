# Module: Dungeonator

## Identity

- **Namespace:** `Dungeonator`
- **Directory:** `Assets/Scripts/ETG/Dungeonator`
- **Files:** 67
- **Category:** External (Procedural Generation Engine)
- **Priority:** ⭐⭐⭐ CRITICAL

## Purpose

Procedural dungeon generation system responsible for creating rooms, corridors, and dungeon layouts. This is the spatial/dungeon framework that provides the foundation for the entire game's level structure.

## Responsibilities

- Dungeon layout generation (rooms, corridors, connections)
- Room template management and instantiation
- Tilemap generation and rendering
- Dungeon flow control (critical path, optional rooms, boss rooms)
- Integration with game mechanics (enemy spawns, loot placement, room events)
- Spatial queries and collision detection

## Public API

### Key Classes

- `Dungeon` - Main dungeon container, manages rooms and flow
- `RoomHandler` - Individual room controller, handles room state and events
- `DungeonData` - Data structure for dungeon configuration
- `DungeonMaterial` - Tile materials and rendering settings
- `TK2DDungeonAssembler` - Builds dungeon geometry from templates
- `CellData` - Cell-level data structure
- `TileIndexGrid` - Grid-based tile indexing

### Key Interfaces

- `IPlaceConfigurable` - Interface for objects that can be placed in dungeons

### Key Enums

- `CellType` - Defines cell types (floor, wall, pit, etc.)
- `RuntimeRoomExitData.ExitDirection` - Room exit directions

## Dependencies

### External Assemblies

- System (98 files) - Collections, LINQ
- UnityEngine (35 files) - Rendering, transforms, physics

### Internal Modules (Dungeonator uses)

- HutongGames.PlayMaker (1 file) - Event integration
- Pathfinding (2 files) - Navigation integration (bidirectional)
- tk2dRuntime (1 file) - Sprite rendering (bidirectional)
- SimplexNoise (1 file) - Procedural noise generation

## Dependents

### Modules Using Dungeonator

- **Core.Systems** (114 files) - HIGHEST usage - Spatial queries, room management
- **Core.Dungeon** (82 files) - Room logic directly depends on Dungeon framework
- **Core.Core** (75 files) - Framework-level spatial queries
- **Core.Actors** (56 files) - AI navigation, actor placement
- **Core.Items** (41 files) - Item spawning and placement
- **Core.Combat** (22 files) - Projectile-dungeon collision
- **HutongGames.PlayMaker** (12 files) - Visual scripting integration
- **Core.UI** (6 files) - Minimap and UI overlays
- **Core.VFX** (3 files) - Effect placement
- **tk2dRuntime** (3 files) - Rendering integration (bidirectional)
- **Pathfinding** (2 files) - Navigation integration (bidirectional)

**Total Dependents:** 419 files across 11 modules

**Dependency Strength:** CRITICAL - Most depended upon module in the codebase

## Internal Structure

```
Dungeonator/
├── Core generation (Dungeon.cs, DungeonData.cs)
├── Room management (RoomHandler.cs, RuntimeDungeonRoom.cs)
├── Tilesets (DungeonMaterial.cs, DungeonTileStampData.cs)
├── Flow control (DungeonFlow.cs, DungeonFlowNode.cs)
├── Builders (TK2DDungeonAssembler.cs, ChunkBuilder.cs)
└── Pathfinding (FastDungeonLayoutPathfinder.cs)
```

## Architectural Notes

**Layer:** Foundation (Layer 0) - Depended on by almost all game logic

**Design Pattern:**
- Facade + Builder pattern
- `Dungeon` acts as facade for complex generation system
- `TK2DDungeonAssembler` implements Builder pattern

**Coupling:** High coupling by design - this is the central spatial system for the entire game

**Circular Dependencies:**
- ✅ Dungeonator ↔ Pathfinding (2 files each way) - Bidirectional navigation integration, expected
- ✅ Dungeonator ↔ tk2dRuntime (1→3 files) - Rendering integration, expected
- ✅ Dungeonator ↔ HutongGames.PlayMaker (1→12 files) - Event system, can be broken

## Migration Guidance

### Critical Module Assessment

**Status:** CRITICAL - Essential for game functionality
**Migration Priority:** Phase 1 (Must migrate early)
**Risk Level:** HIGH - 419 files depend on this module

### Migration Options

**Option A: Port Entire Module (RECOMMENDED)**
- Cleanly separated namespace makes porting feasible
- Preserve all functionality as-is
- Requires porting dependencies: tk2dRuntime → Native 2D rendering
- Estimated effort: MEDIUM

**Option B: Replace with Equivalent Library**
- Find procedural dungeon generation library for target platform
- High risk: ETG's dungeon generation is highly customized
- Requires extensive refactoring of dependent code
- Estimated effort: VERY HIGH

**Option C: Keep Data Structures, Rewrite Algorithms**
- Preserve data structures (CellData, DungeonData)
- Reimplement generation algorithms
- Maintains API surface for dependent code
- Estimated effort: HIGH

### Key Integration Points to Preserve

**Critical Interfaces:**
- `IPlaceConfigurable` - Used throughout codebase for object placement
- Must maintain compatible interface in any migration approach

**Critical Data Types:**
- `CellType` enum - Core to all dungeon logic, must be preserved exactly
- `RoomHandler` - Many systems subscribe to room lifecycle events
- `Dungeon` singleton pattern - Used extensively for spatial queries

**Critical Events:**
- `RoomHandler.OnEnemiesCleared`
- `RoomHandler.OnPlayerEntered`
- Room state change events

### Dependencies to Address First

1. **tk2dRuntime** (1 file, bidirectional)
   - Replace with Unity native 2D sprite rendering
   - Low complexity, required before porting Dungeonator

2. **HutongGames.PlayMaker** (1 file)
   - Can be removed (visual scripting not essential)
   - Break circular dependency before migration

3. **Pathfinding** (2 files, bidirectional)
   - Keep tightly coupled or replace both together
   - Consider standard pathfinding library

### Migration Strategy Recommendation

**Phase 1: Prepare Dependencies**
1. Replace tk2dRuntime with native rendering
2. Remove PlayMaker integration
3. Evaluate Pathfinding module

**Phase 2: Port Dungeonator**
1. Port entire module as-is to target platform
2. Maintain namespace and public API
3. Create compatibility shims for Unity-specific code

**Phase 3: Validate Integration**
1. Test with dependent modules (Core.Systems first)
2. Verify spatial queries work correctly
3. Validate room generation and events

### Testing Requirements

**Critical Test Cases:**
- Dungeon generation produces valid layouts
- Room connectivity works correctly
- Spatial queries return correct results
- Room events fire at appropriate times
- Actor placement respects dungeon constraints

---

**Last Updated:** 2026-01-18
**Documentation Version:** 1.0
**Related Documentation:**
- [Module Manifest](../../../Docs/Module_Manifest.md)
- [Dependency Matrix](../../../Docs/Module_Dependency_Matrix.md)
- [Dependency Graph](../../../Docs/Module_Dependency_Graph.md)
