# Module: Core.Dungeon

## Identity

- **Namespace:** Global (no specific namespace)
- **Directory:** `Assets/Scripts/ETG/Core/Dungeon`
- **Files:** 125
- **Category:** Core Subsystem (Domain Layer)
- **Priority:** ⭐⭐ HIGH

## Purpose

Room management, interactable objects, and dungeon-specific game mechanics. This module implements the dungeon gameplay layer on top of the Dungeonator spatial framework, handling room logic, encounters, and interactable objects.

## Responsibilities

- Room controller implementations
- Shrine and NPC interactions
- Boss room controllers and fight logic
- Special encounter management
- Room event handling and triggers
- Chest and loot systems
- Shop and merchant systems
- Environmental hazards and traps

## Subsystems

### Generation Subsystem
- Room generation logic
- Procedural content placement
- Encounter composition

### Rooms Subsystem
- Room-specific controllers
- Boss room logic
- Special room implementations

### Interactables Subsystem
- Interactive objects (shrines, chests, NPCs)
- Dialogue systems
- Transaction systems (shops)

## Public API

### Key Classes

**Room Controllers:**
- `BossFinalRoomController` - Boss fight room logic and state management
- Room controller implementations
- Special room handlers

**Interactables:**
- `Chest` - Chest interactable and loot system
- `ShrineController` - Shrine interactions and effects
- `ForgeHammerController` - Forge/crafting mechanics
- `TalkModule` - NPC dialogue system
- Shop controllers

**Encounters:**
- Encounter composition classes
- Wave management
- Enemy spawn logic

### Key Interfaces

- Interactable interfaces
- Room event interfaces

## Dependencies

### External Assemblies

- System (134 files) - Collections, LINQ
- UnityEngine (70 files) - Transform, physics, rendering

### Internal Modules (Core.Dungeon uses)

**Foundation Dependencies:**
- **Dungeonator** (82 files) - VERY HIGH - Room spatial data, dungeon context
- **HutongGames.PlayMaker** (7 files) - Visual scripting for some encounters

**Core Dependencies:**
- **Core.Systems** (53 files) - HIGH - Game managers, databases
- **Core.Core** (29 files) - Base classes
- **Core.Actors** (17 files) - Actor spawning and management
- **Core.Items** (13 files) - Item rewards, shop inventory
- **Core.Combat** (5 files) - Boss fight mechanics
- **Brave.BulletScript** (1 file) - Boss patterns

## Dependents

### Modules Using Core.Dungeon

- **Core.Combat** (5 files) - Environmental damage
- **Core.VFX** (2 files) - Room effects

**Total Dependents:** 7 files across 2 modules

**Dependency Strength:** LOW - Relatively isolated, few modules depend on it

## Internal Structure

```
Core/Dungeon/
├── Generation/
│   ├── Room generation
│   ├── Encounter composition
│   └── Procedural placement
├── Rooms/
│   ├── BossFinalRoomController.cs
│   ├── Special room controllers
│   └── Room state machines
└── Interactables/
    ├── Chest.cs
    ├── ShrineController.cs
    ├── Shop systems
    └── NPC controllers
```

## Architectural Notes

**Layer:** Domain Logic (Layer 2) - Dungeon gameplay mechanics

**Design Patterns:**
- State Machine pattern (room states, boss fights)
- Observer pattern (room events)
- Command pattern (interactable actions)
- Factory pattern (encounter spawning)

**Coupling:** HIGH to Dungeonator (82 files) - Intentional tight coupling

**Circular Dependencies:** None

## Migration Guidance

### Critical Module Assessment

**Status:** HIGH PRIORITY - Dungeon gameplay mechanics
**Migration Priority:** Phase 2 (with other Domain modules, after Dungeonator)
**Risk Level:** MEDIUM - Tight Dungeonator coupling, complex state machines

### Migration Options

**Option A: Port with Dungeonator (RECOMMENDED)**
- Migrate together with Dungeonator module
- Maintain tight integration
- Preserve room logic exactly
- Estimated effort: MEDIUM

**Option B: Port Independently**
- Wait for Dungeonator to be ported first
- Then port Core.Dungeon
- Risk: May miss integration issues
- Estimated effort: MEDIUM

### Key Integration Points to Preserve

**Critical Classes:**
- `BossFinalRoomController` - Complex boss fight state machine
- `Chest` - Loot system, reward generation
- `ShrineController` - Shrine effects and costs
- Room controller base classes

**Critical Systems:**
- Room state management (cleared, active, visited)
- Boss fight sequencing
- Encounter spawn logic
- Interactable activation system

**Critical Integration:**
- Dungeonator RoomHandler integration (82 files)
- Room event system
- Encounter composition

### Dependencies to Address First

**Critical Foundation Dependency:**
1. **Dungeonator** (82 files) - MUST port first
   - Core.Dungeon is effectively an extension of Dungeonator
   - Tightly coupled to RoomHandler and dungeon spatial system
   - Cannot port without Dungeonator

**Other Dependencies:**
2. **Core.Systems** (53 files) - Port first
   - Encounter databases
   - Game state management

3. **Core.Actors** (17 files) - Port first or in parallel
   - Enemy spawning
   - Boss actor management

4. **Core.Items** (13 files) - Port first or in parallel
   - Chest rewards
   - Shop inventory

5. **HutongGames.PlayMaker** (7 files) - Replace or remove
   - Visual scripting for some encounters
   - Can be replaced with code

### Migration Strategy Recommendation

**Phase 1: Wait for Dependencies**
1. Ensure Dungeonator fully ported (critical dependency)
2. Ensure Core.Systems ported (databases)
3. Ensure Core.Actors ported (spawning)
4. Ensure Core.Items ported (rewards)

**Phase 2: Port Room System**
1. **Room Controllers First:**
   - Port room controller base classes
   - Port special room implementations
   - Test: Rooms initialize and transition correctly
2. **Boss Rooms Second:**
   - Port BossFinalRoomController
   - Port boss fight state machines
   - Test: Boss fights work correctly
3. **Encounters Third:**
   - Port encounter composition
   - Port wave management
   - Test: Enemy waves spawn correctly

**Phase 3: Port Interactables**
1. **Chests First:**
   - Port chest system
   - Port loot generation
   - Test: Chests open and give correct rewards
2. **Shrines Second:**
   - Port shrine controllers
   - Port shrine effects
   - Test: Shrines function correctly
3. **NPCs/Shops Third:**
   - Port dialogue system
   - Port shop mechanics
   - Test: Transactions work

**Phase 4: Integration Testing**
1. Test room progression
2. Validate boss fights
3. Test interactable interactions
4. Verify encounter difficulty

### Testing Requirements

**Critical Test Cases:**
- Rooms load and initialize correctly
- Room state transitions work (inactive → active → cleared)
- Boss fights execute correctly
- Encounter waves spawn appropriately
- Chests open and give rewards
- Shrines apply effects correctly
- Shops function (buy/sell)
- NPCs trigger dialogue

**Boss Fight Validation:**
- Boss room initializes correctly
- Boss phases trigger at correct health thresholds
- Boss patterns execute (via Brave.BulletScript)
- Boss death triggers room clear
- Rewards spawn after boss defeat

**Performance Requirements:**
- Room initialization fast (<100ms)
- Encounter spawning efficient
- No stuttering during wave spawns

### Boss Room State Machine

**Complexity:**
- BossFinalRoomController is complex state machine
- Multiple phases based on boss health
- Triggers for phase transitions
- Integration with boss actor AI

**Migration Strategy:**
- Port state machine intact
- Preserve exact phase transitions
- Test against original boss fights
- Validate timing and sequencing

### Dungeonator Integration

**82 files depend on Dungeonator:**
- RoomHandler integration (room spatial data)
- Dungeon context queries
- Room connectivity logic
- Tile and cell queries

**Strategy:**
- Treat Core.Dungeon as Dungeonator extension
- Maintain tight coupling intentionally
- Port together if possible
- Test integration extensively

### Risk Assessment

**Medium Risk:**
- Boss fight state machines (complex logic)
- Encounter composition (difficulty balance)
- Dungeonator coupling (integration issues)

**Low Risk:**
- Chest system (straightforward)
- Shop mechanics (well-defined)
- NPC dialogue (isolated)

**Mitigation:**
- Port after Dungeonator fully tested
- Extensive boss fight testing
- Record original boss fights for comparison
- Test each interactable type thoroughly

### PlayMaker Dependency

**7 files use PlayMaker:**
- Visual scripting for some encounters
- Can be removed or replaced

**Strategy:**
- Identify PlayMaker usage
- Replace with code or remove
- Low priority - not critical

### Performance Considerations

**Performance Areas:**
- Room initialization time
- Encounter spawn performance
- Boss fight updates (state machine)

**Optimization:**
- Cache room data
- Optimize spawn logic
- Efficient state machine updates

---

**Last Updated:** 2026-01-18
**Documentation Version:** 1.0
**Related Documentation:**
- [Module Manifest](../../../../Docs/Module_Manifest.md)
- [Dependency Matrix](../../../../Docs/Module_Dependency_Matrix.md)
- [Dungeonator MODULE_BOUNDARY](../../Dungeonator/MODULE_BOUNDARY.md)
- [Core.Systems MODULE_BOUNDARY](../Systems/MODULE_BOUNDARY.md)
- [Core.Actors MODULE_BOUNDARY](../Actors/MODULE_BOUNDARY.md)
- [Core.Items MODULE_BOUNDARY](../Items/MODULE_BOUNDARY.md)
