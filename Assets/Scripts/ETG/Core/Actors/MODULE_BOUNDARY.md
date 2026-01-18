# Module: Core.Actors

## Identity

- **Namespace:** Global (no specific namespace)
- **Directory:** `Assets/Scripts/ETG/Core/Actors`
- **Files:** 167
- **Category:** Core Subsystem (Domain Layer)
- **Priority:** ⭐⭐ HIGH

## Purpose

Character and entity management system handling players, enemies, NPCs, and their behaviors. This module implements all actor-related gameplay logic including player control, enemy AI, and behavior systems.

## Responsibilities

- Player character control and abilities
- Enemy AI and behavior trees
- NPC interactions and dialogue systems
- Actor lifecycle management
- Behavior system framework
- Character animation and state management

## Subsystems

### Player Subsystem
- PlayerController
- Player-specific mechanics and abilities
- Player state management

### Enemy Subsystem
- AIActor base class
- Enemy-specific behaviors
- Boss enemy implementations

### Behaviors Subsystem
- Behavior tree components
- Attack behavior system
- Movement behavior system

## Public API

### Key Classes

**Player:**
- `PlayerController` - Main player character controller
- Player ability classes
- Player state handlers

**Enemy:**
- `AIActor` - Enemy AI base class
- Enemy behavior implementations
- Boss-specific controllers

**Behaviors:**
- `BehaviorBase` - Base class for AI behaviors
- `AttackBehaviorGroup` - Grouped attack behaviors
- Behavior tree node classes

### Key Interfaces

- Actor behavior interfaces
- AI state interfaces

## Dependencies

### External Assemblies

- System (187 files) - Collections, LINQ
- UnityEngine (94 files) - Transform, physics, animation

### Internal Modules (Core.Actors uses)

**Foundation Dependencies:**
- **Dungeonator** (56 files) - HIGH - Navigation and spatial queries
- **FullInspector** (89 files) - VERY HIGH - Behavior tree serialization
- **Pathfinding** (20 files) - HIGH - AI navigation

**Core Dependencies:**
- **Core.Core** (81 files) - HIGH - Base actor classes (GameActor, AIActor)
- **Core.Systems** (87 files) - HIGH - Game managers and utilities

## Dependents

### Modules Using Core.Actors

- **Core.Items** (76 files) - Items affect and interact with actors
- **Core.Combat** (31 files) - Combat systems affect actors
- **Core.Dungeon** (17 files) - Dungeon spawns and manages actors
- **Core.UI** (13 files) - UI displays actor information

**Total Dependents:** 137 files across 4 modules

**Dependency Strength:** HIGH - Core domain logic

## Internal Structure

```
Core/Actors/
├── Player/
│   ├── PlayerController.cs
│   ├── Player abilities
│   └── Player state management
├── Enemy/
│   ├── AIActor.cs
│   ├── Enemy implementations
│   └── Boss enemies
└── Behaviors/
    ├── BehaviorBase.cs
    ├── Attack behaviors
    └── Movement behaviors
```

## Architectural Notes

**Layer:** Domain Logic (Layer 2) - Game mechanics implementation

**Design Patterns:**
- Component pattern (actor behaviors)
- Behavior tree pattern (AI system)
- State machine pattern (actor states)
- Observer pattern (actor events)

**Coupling:** HIGH - Tightly coupled to Core.Core and multiple foundation modules

**Circular Dependencies:** None

## Migration Guidance

### Critical Module Assessment

**Status:** HIGH PRIORITY - Core gameplay logic
**Migration Priority:** Phase 2 (after Foundation and Core.Core)
**Risk Level:** HIGH - Central to gameplay feel and balance

### Migration Options

**Option A: Direct Port (RECOMMENDED)**
- Port entire module with Core.Core
- Maintain actor behavior exactly
- Preserve gameplay feel
- Estimated effort: HIGH

**Option B: Refactor Behavior System**
- Redesign AI behavior tree system for target platform
- Risky - may change gameplay behavior
- High effort, uncertain results
- Estimated effort: VERY HIGH

### Key Integration Points to Preserve

**Critical Classes:**
- `PlayerController` - Central to game feel, must preserve exactly
- `AIActor` - Base for all enemy AI
- `BehaviorBase` - Foundation of behavior system

**Critical Behaviors:**
- Player movement feel (speed, acceleration, dodgeroll)
- Enemy AI decision-making
- Behavior tree execution order
- Attack timing and synchronization

**Critical Data:**
- Behavior tree configurations (serialized in FullInspector)
- Enemy stat configurations
- Player ability parameters

### Dependencies to Address First

**External Dependencies:**
1. **FullInspector** (89 files) - Replace with standard serialization
   - Behavior trees heavily use FullInspector serialization
   - Need migration strategy for behavior data

2. **Dungeonator** (56 files) - Port with Foundation layer
   - AI navigation depends on dungeon spatial system
   - Required for enemy movement

3. **Pathfinding** (20 files) - Port or replace
   - Can replace with standard pathfinding library
   - Medium priority

**Core Dependencies:**
1. **Core.Core** (81 files) - Port together
   - Actors inherit from Core.Core base classes
   - Must migrate Core.Core first

2. **Core.Systems** (87 files) - Port in parallel
   - Actors use managers and utilities
   - Can migrate after Core.Systems available

### Migration Strategy Recommendation

**Phase 1: Prepare Dependencies**
1. Ensure Core.Core is ported (base classes)
2. Ensure Dungeonator is available (navigation)
3. Plan FullInspector replacement (behavior data)

**Phase 2: Port Core.Actors**
1. **Player First:** Port PlayerController and player systems
   - Critical for gameplay feel
   - Test extensively against original
2. **Enemy Second:** Port AIActor and enemy behaviors
   - Test AI decision-making
   - Verify behavior trees execute correctly
3. **Behaviors Last:** Port behavior system
   - Ensure compatibility with serialized data

**Phase 3: Integration Testing**
1. Player movement feel verification
2. Enemy AI behavior validation
3. Combat integration testing
4. Performance profiling

### Testing Requirements

**Critical Test Cases:**
- Player movement matches original (speed, acceleration, dodgeroll)
- Player abilities function correctly
- Enemy AI makes correct decisions
- Behavior trees execute in correct order
- Boss patterns execute correctly
- Collision and hitboxes accurate

**Gameplay Feel Tests:**
- Player movement feels identical to original
- Dodgeroll timing and invincibility frames accurate
- Enemy aggression and targeting behavior correct

**Performance Requirements:**
- Handle 20+ enemies on screen simultaneously
- AI decision-making <1ms per enemy per frame
- Behavior tree evaluation efficient

### Data Migration

**Behavior Tree Data:**
- 89 files use FullInspector for behavior serialization
- Need strategy to migrate serialized behavior data
- Consider: Export to JSON, convert to target format

**Configuration Data:**
- Enemy stats and parameters
- Player ability configurations
- Behavior tree structures

### Risk Assessment

**High Risk:**
- Player movement feel (must be pixel-perfect)
- Behavior tree execution (complex serialized data)
- AI decision-making (subtle bugs can break gameplay)

**Medium Risk:**
- Enemy stat balance (data migration)
- Behavior configurations (serialization format change)

**Mitigation:**
- Side-by-side comparison with original game
- Record gameplay sessions for A/B testing
- Extensive playtesting during migration
- Keep original behavior data as reference

### Performance Considerations

**Critical Performance Areas:**
- Player input response (must be instant)
- AI decision-making (must not impact framerate)
- Behavior tree evaluation (efficient traversal)
- Collision detection (optimized queries)

**Profiling Required:**
- Player controller update time
- AI behavior evaluation time
- Behavior tree traversal overhead

---

**Last Updated:** 2026-01-18
**Documentation Version:** 1.0
**Related Documentation:**
- [Module Manifest](../../../../Docs/Module_Manifest.md)
- [Dependency Matrix](../../../../Docs/Module_Dependency_Matrix.md)
- [Core.Core MODULE_BOUNDARY](../Core/MODULE_BOUNDARY.md)
- [Core.Systems MODULE_BOUNDARY](../Systems/MODULE_BOUNDARY.md)
- [Dungeonator MODULE_BOUNDARY](../../Dungeonator/MODULE_BOUNDARY.md)
