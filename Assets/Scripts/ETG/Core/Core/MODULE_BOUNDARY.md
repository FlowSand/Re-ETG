# Module: Core.Core

## Identity

- **Namespace:** Global (no specific namespace)
- **Directory:** `Assets/Scripts/ETG/Core/Core`
- **Files:** 377 (second largest Core module)
- **Category:** Core Subsystem (Core Layer)
- **Priority:** ⭐⭐⭐ CRITICAL

## Purpose

Framework foundation providing base classes, enumerations, and interfaces used throughout the entire codebase. This is the bedrock upon which all game logic is built, defining fundamental types and contracts.

## Responsibilities

- Base classes for all game actors (GameActor, AIActor)
- Core enumerations (Achievement, GameMode, DungeonFlowLevelEntry, etc.)
- Interface definitions (IAutoAimTarget, IPlayerInteractable, etc.)
- Framework patterns and contracts
- Physics wrapper classes (SpeculativeRigidbody, PixelCollider)
- Core data structures and utilities

## Subsystems

### Framework Subsystem
- Base classes (GameActor, AIActor, etc.)
- Game object foundations
- Physics wrappers

### Enums Subsystem
- Global enumerations
- Gameplay constants
- Configuration enums

### Interfaces Subsystem
- Contract definitions
- Cross-cutting interfaces
- Plugin points

## Public API

### Key Base Classes

**Actor Framework:**
- `GameActor` - Base class for all game entities (players, enemies, NPCs)
- `AIActor` - Base class for AI-controlled entities
- `DungeonPlaceableBehaviour` - Base for objects that can be placed in dungeons
- `BraveBehaviour` - Unity MonoBehaviour extension

**Physics Framework:**
- `SpeculativeRigidbody` - Custom physics rigidbody wrapper for pixel-perfect movement
- `PixelCollider` - Pixel-perfect collision detection
- `CollisionData` - Collision event data

### Key Enumerations

**Gameplay:**
- `Achievement` - Achievement definitions
- `GameMode` - Game mode types (Normal, Boss Rush, etc.)
- `DungeonFlowLevelEntry` - Dungeon floor identifiers
- `GungeonFlags` - Game state flags
- `PlayableCharacters` - Character selection enum

**Systems:**
- `CoreDamageTypes` - Base damage type enumeration
- `ProjectileData.FixedFallbackOptions` - Projectile behavior options

### Key Interfaces

**Gameplay:**
- `IAutoAimTarget` - Auto-aim targeting interface
- `IPlayerInteractable` - Interactive object interface
- `IPlaceConfigurable` - Dungeon placement interface

**Systems:**
- `ILevelLoadedListener` - Level load notification
- `IPaydayReceiver` - Currency event handling

## Dependencies

### External Assemblies

- System (413 files) - Heavy .NET BCL usage
- UnityEngine (213 files) - Heavy Unity API usage

### Internal Modules (Core.Core uses)

**Foundation Dependencies:**
- **Dungeonator** (75 files) - HIGH - Spatial integration, placement
- **Brave.BulletScript** (6 files) - Bullet pattern integration
- **Pathfinding** (1 file) - Navigation integration
- **tk2dRuntime** (1 file) - Sprite rendering
- **InControl** (1 file) - Input integration
- **HutongGames.PlayMaker** (4 files) - Visual scripting integration

## Dependents

### Modules Using Core.Core

- **Core.Actors** (81 files) - VERY HIGH - Inherits from GameActor, AIActor
- **Core.Combat** (66 files) - HIGH - Uses physics and collision classes
- **Core.Items** (58 files) - HIGH - Inherits from base classes
- **Core.Dungeon** (29 files) - Uses placement and interaction interfaces
- **Core.Audio** (10 files) - Uses core types
- **Core.UI** (9 files) - Uses enums and interfaces
- **Core.VFX** (8 files) - Uses core types
- **Core.Systems** (4 files) - Bidirectional dependency

**Total Dependents:** 265 files across 8 modules

**Dependency Strength:** CRITICAL - Foundation for all domain logic

## Internal Structure

```
Core/Core/
├── Framework/
│   ├── GameActor.cs
│   ├── AIActor.cs
│   ├── SpeculativeRigidbody.cs
│   ├── PixelCollider.cs
│   └── Base behaviour classes
├── Enums/
│   ├── Achievement.cs
│   ├── GameMode.cs
│   ├── DungeonFlowLevelEntry.cs
│   └── Other enumerations
└── Interfaces/
    ├── IAutoAimTarget.cs
    ├── IPlayerInteractable.cs
    ├── IPlaceConfigurable.cs
    └── Other interfaces
```

## Architectural Notes

**Layer:** Core Framework (Layer 1) - Foundation depended on by all Domain logic

**Design Patterns:**
- Template Method pattern (base classes)
- Strategy pattern (interfaces)
- Wrapper pattern (physics classes)
- Abstract Base Class pattern

**Coupling:** MEDIUM - Depends on Foundation (Dungeonator), depended on by all Domain

**Circular Dependencies:**
- ⚠️ Core.Core ↔ Core.Systems (4 files) - Minor bidirectional dependency

## Migration Guidance

### Critical Module Assessment

**Status:** CRITICAL - Foundation for all game logic
**Migration Priority:** Phase 1 (port VERY EARLY with Foundation)
**Risk Level:** VERY HIGH - Breaking changes cascade to all modules

### Migration Options

**Option A: Direct Port (STRONGLY RECOMMENDED)**
- Port entire module intact
- Preserve all public API signatures
- Maintain exact class hierarchy
- Estimated effort: HIGH

**Option B: Refactor Class Hierarchy**
- Risky - breaks all dependent code
- Not recommended unless critical issues found
- Estimated effort: VERY HIGH (not advised)

### Key Integration Points to Preserve

**CRITICAL - DO NOT CHANGE:**

**Actor Base Classes:**
- `GameActor` - All game entities inherit from this
- `AIActor` - All enemies inherit from this
- Public methods and properties must be preserved exactly

**Physics Classes:**
- `SpeculativeRigidbody` - Custom physics, must maintain exact behavior
- `PixelCollider` - Pixel-perfect collision, critical for game feel
- Collision detection must be identical to original

**Enumerations:**
- All enum values must be preserved exactly
- Enum integer values should not change (save compatibility)
- Adding new values acceptable, removing is not

**Interfaces:**
- Interface contracts must be preserved
- All methods and properties must remain
- Changing signatures breaks implementers

### Dependencies to Address First

**Critical Foundation Dependency:**
1. **Dungeonator** (75 files) - HIGH usage
   - Core.Core has spatial integration with Dungeonator
   - Must port Dungeonator first or create adapter
   - Strategy: Port Dungeonator first (already planned)

**Other Dependencies:**
2. **Brave.BulletScript** (6 files) - Port with Foundation
3. **tk2dRuntime** (1 file) - Replace or port
4. **Pathfinding** (1 file) - Replace or port
5. **PlayMaker** (4 files) - Remove or replace

### Migration Strategy Recommendation

**Phase 1: Preparation**
1. Ensure Dungeonator is ported (75 files dependency)
2. Catalog all public API (classes, methods, properties)
3. Identify Unity-specific code requiring adaptation
4. Plan physics system port (SpeculativeRigidbody)

**Phase 2: Port Foundation Types**
1. **Enumerations First:**
   - Port all enums
   - Preserve exact values
   - Test: Enums accessible and correct
2. **Interfaces Second:**
   - Port all interface definitions
   - No implementation yet
   - Test: Interfaces compile
3. **Base Classes Third:**
   - Port GameActor, AIActor, base classes
   - Stub Unity dependencies if needed
   - Test: Classes instantiable

**Phase 3: Port Physics System**
1. **SpeculativeRigidbody:**
   - Complex custom physics system
   - May need significant adaptation for new engine
   - Critical for game feel
2. **PixelCollider:**
   - Pixel-perfect collision detection
   - Must maintain exact behavior
3. **CollisionData:**
   - Event data structures

**Phase 4: Integration Testing**
1. Test with first dependent module (Core.Actors)
2. Validate inheritance works correctly
3. Verify physics behavior matches original
4. Performance profiling

### Testing Requirements

**Critical Test Cases:**
- GameActor instances create correctly
- AIActor inheritance chain works
- SpeculativeRigidbody movement matches original
- PixelCollider detection pixel-perfect
- All enums accessible with correct values
- All interfaces implementable

**Physics Validation:**
- Actor movement feels identical to original
- Collision detection accuracy
- Performance under load (100+ actors)

**API Validation:**
- All public methods callable
- All properties accessible
- No missing functionality

### Physics System Complexity

**SpeculativeRigidbody:**
- Custom physics implementation for pixel-perfect movement
- Replaces or wraps Unity's physics system
- Critical for game feel (movement, collisions)
- May require significant porting effort

**Strategy:**
- Understand original implementation deeply
- Consider wrapping target engine's physics
- Maintain exact behavior (frame-perfect)
- Extensive testing required

**Risk:** VERY HIGH - Changes to physics affect entire game feel

### Risk Assessment

**Very High Risk:**
- Base class changes (cascade to 265 files)
- Physics system (SpeculativeRigidbody) - game feel
- Enum value changes (save compatibility)
- Interface contract changes (all implementers break)

**High Risk:**
- Unity API dependencies (requires adaptation)
- Dungeonator coupling (integration complexity)

**Mitigation:**
- Port VERY early in migration process
- Extensive API testing
- Physics validation suite
- Side-by-side comparison with original
- Zero tolerance for API breaking changes

### API Stability

**Public API MUST remain stable:**
- All public classes
- All public methods
- All public properties
- All enumerations
- All interfaces

**Internal changes acceptable:**
- Private methods
- Internal implementation
- Performance optimizations
- Target platform adaptations

### Performance Considerations

**Critical Performance:**
- GameActor update loop (100+ actors per frame)
- SpeculativeRigidbody physics (every actor, every frame)
- PixelCollider queries (collision detection hotpath)

**Requirements:**
- Actor updates <0.1ms per actor
- Collision queries <0.05ms per query
- No GC allocation in update loops

**Profiling Required:**
- Actor update time
- Physics system overhead
- Collision detection performance
- Memory allocation patterns

### Circular Dependency

**Core.Core ↔ Core.Systems (4 files):**
- Minor bidirectional dependency
- Not problematic
- Both are Core layer modules

**Strategy:**
- Accept circular dependency
- Port both modules in Phase 1
- No breaking required

---

**Last Updated:** 2026-01-18
**Documentation Version:** 1.0
**Related Documentation:**
- [Module Manifest](../../../../Docs/Module_Manifest.md)
- [Dependency Matrix](../../../../Docs/Module_Dependency_Matrix.md)
- [Dungeonator MODULE_BOUNDARY](../../Dungeonator/MODULE_BOUNDARY.md)
- [Core.Systems MODULE_BOUNDARY](../Systems/MODULE_BOUNDARY.md)

**CRITICAL NOTE:**
This module is the FOUNDATION of all game logic. Do NOT make breaking changes to public API. Any change to this module cascades to 265 files across 8 modules. Port with extreme care and extensive testing.
