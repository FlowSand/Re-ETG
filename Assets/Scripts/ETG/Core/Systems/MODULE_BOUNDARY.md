# Module: Core.Systems

## Identity

- **Namespace:** Global (no specific namespace)
- **Directory:** `Assets/Scripts/ETG/Core/Systems`
- **Files:** 990 (LARGEST module in codebase)
- **Category:** Core Subsystem
- **Priority:** ⭐⭐⭐ CRITICAL

## Purpose

Centralized game systems including data management, game managers, and utility functions. Acts as the integration layer between Foundation modules and Domain logic. This is the "catch-all" for core game functionality that doesn't fit into other specific modules.

## Responsibilities

- Game state management and lifecycle
- Data persistence and save/load system
- Manager classes (AmmonomiconInstanceManager, ChallengeManager, etc.)
- Configuration databases (AdvancedSynergyDatabase, BossManager)
- Runtime game managers
- Utility functions and helper methods
- Cross-cutting concerns (logging, debugging, profiling)

## Subsystems

This oversized module contains three major subsystems:

### Data Subsystem
- Database objects (synergies, bosses, enemies, items)
- Configuration assets
- Static data definitions

### Management Subsystem
- Manager classes (GameManager, SaveManager, etc.)
- Singleton systems
- Game state controllers
- Service locators

### Utilities Subsystem
- Helper functions
- Extension methods
- Utility classes (BraveUtility, etc.)
- Common algorithms

## Public API

### Key Classes

**Core Managers:**
- `GameManager` - Central game state manager (singleton)
- `SaveManager` - Save/load persistence system
- `BossManager` - Boss data and configuration management
- `AmmonomiconInstanceManager` - In-game encyclopedia management
- `ChallengeManager` - Challenge mode management

**Database Classes:**
- `AdvancedSynergyDatabase` - Item synergy definitions
- `EnemyDatabase` - Enemy configuration database
- `ItemDatabase` - Item configuration database

**Utility Classes:**
- `BraveUtility` - General utility helper methods
- Extension methods for Unity types
- Math and geometry utilities

### Key Interfaces

- Various manager interfaces
- Data provider interfaces

## Dependencies

### External Assemblies

- System (1062 files) - Extensive use of .NET BCL
- UnityEngine (525 files) - Heavy Unity API usage

### Internal Modules (Core.Systems uses)

**Critical Dependencies:**
- **Brave.BulletScript** (224 files) - VERY HIGH - Bullet pattern definitions stored as data
- **FullInspector** (224 files) - VERY HIGH - Data serialization framework
- **Dungeonator** (114 files) - HIGH - Spatial system integration

**Medium Dependencies:**
- tk2dRuntime (36 files) - Sprite rendering
- InControl (22 files) - Input management
- HutongGames.PlayMaker (10 files) - Visual scripting
- Pathfinding (6 files) - Navigation
- DaikonForge (9 files) - UI framework
- FullSerializer (8 files) - Serialization

**Low Dependencies:**
- TestSimpleRNG (1 file) - Random number generation
- Kvant (1 file) - Visual effects

## Dependents

### Modules Using Core.Systems

- **Core.Actors** (87 files) - Actor behaviors use managers
- **Core.Combat** (83 files) - Combat system uses game state
- **Core.Items** (61 files) - Item system uses databases
- **Core.Dungeon** (53 files) - Dungeon generation uses configuration
- **Core.UI** (38 files) - UI uses managers and state
- **Core.Audio** (23 files) - Audio uses managers
- **Core.VFX** (13 files) - VFX uses utilities
- **Core.Core** (4 files) - Bidirectional dependency

**Total Dependents:** 362 files across 8 modules

**Dependency Strength:** CRITICAL - Central integration point

## Internal Structure

```
Core/Systems/
├── Data/
│   ├── Databases (BossManager.cs, EnemyDatabase.cs, etc.)
│   ├── Configuration (AdvancedSynergyDatabase.cs, etc.)
│   └── Static data definitions
├── Management/
│   ├── Managers (GameManager.cs, SaveManager.cs, etc.)
│   ├── Singleton systems
│   └── Service locators
└── Utilities/
    ├── BraveUtility.cs
    ├── Extension methods
    └── Helper classes
```

## Architectural Notes

**Layer:** Core Systems (Layer 1) - Integration layer between Foundation and Domain

**Design Patterns:**
- Singleton pattern (managers)
- Service Locator pattern
- Utility/Helper pattern
- Database pattern (configuration objects)

**Coupling:** VERY HIGH - This module is coupled to almost everything

**Issues:**
- ⚠️ **Oversized Module:** 990 files is too large for a single module
- ⚠️ **Mixed Concerns:** Data, management, and utilities should be separated
- ⚠️ **High Coupling:** Depends on many external modules
- ⚠️ **Refactoring Needed:** Should be split into smaller, focused modules

**Circular Dependencies:**
- ⚠️ Core.Systems ↔ Core.Core (4 files) - Minor bidirectional dependency

## Migration Guidance

### Critical Module Assessment

**Status:** CRITICAL - Central integration point for entire game
**Migration Priority:** Phase 2 (after Foundation, before Domain)
**Risk Level:** VERY HIGH - Extremely high coupling, largest module

### Refactoring Recommendation

**BEFORE MIGRATION:** Consider splitting into smaller modules:

1. **Core.Data** (Database and configuration)
   - AdvancedSynergyDatabase
   - BossManager
   - EnemyDatabase, ItemDatabase
   - Static data definitions

2. **Core.Management** (Manager classes)
   - GameManager
   - SaveManager
   - AmmonomiconInstanceManager
   - Other manager classes

3. **Core.Utilities** (Helper functions)
   - BraveUtility
   - Extension methods
   - Math/geometry utilities

**Benefit:** Reduces coupling, makes migration more manageable

### Migration Options

**Option A: Migrate as Monolith (NOT RECOMMENDED)**
- Port entire 990-file module at once
- Very high risk
- Difficult to test and validate
- Estimated effort: VERY HIGH

**Option B: Phased Migration by Subsystem (RECOMMENDED)**
1. **Phase 2a:** Migrate Data subsystem
   - Lower risk (mostly static data)
   - Minimal runtime dependencies
2. **Phase 2b:** Migrate Management subsystem
   - Medium risk (manager singletons)
   - Depends on Data subsystem
3. **Phase 2c:** Migrate Utilities subsystem
   - Low risk (stateless helpers)
   - Can be migrated independently

**Option C: Refactor Then Migrate (BEST)**
1. Refactor into separate modules first (in Unity project)
2. Reduce coupling between subsystems
3. Migrate each module independently
4. Estimated effort: HIGH (but lower risk)

### Key Integration Points to Preserve

**Critical Managers:**
- `GameManager` singleton - Accessed from almost everywhere
- `SaveManager` - Save/load functionality
- Manager access patterns must remain consistent

**Critical Databases:**
- `AdvancedSynergyDatabase` - Item synergy system depends on this
- Boss/Enemy/Item databases - Core gameplay data

**Critical Utilities:**
- `BraveUtility` methods - Used extensively
- Extension methods - May need to be preserved or reimplemented

### Dependencies to Address First

**External Dependencies (Must port/replace first):**
1. **Brave.BulletScript** (224 files) - Port with Foundation layer
2. **FullInspector** (224 files) - Replace with standard serialization
3. **Dungeonator** (114 files) - Port with Foundation layer

**Strategy:**
- Wait until Dungeonator and Brave.BulletScript are ported
- Plan FullInspector replacement strategy
- Then begin Core.Systems migration

### Migration Strategy Recommendation

**Phase 1: Preparation**
1. Analyze 990 files to categorize into subsystems
2. Identify true public API (what other modules actually use)
3. Create dependency map within Core.Systems
4. Consider refactoring before migration

**Phase 2: External Dependencies**
1. Ensure Dungeonator ported (114 files dependency)
2. Ensure Brave.BulletScript ported (224 files dependency)
3. Replace or adapt FullInspector (224 files dependency)

**Phase 3: Subsystem Migration**
1. **Data First:** Port databases and configuration
   - Lowest risk, mostly data objects
   - Test: Data loads correctly
2. **Management Second:** Port manager classes
   - Medium risk, singleton patterns
   - Test: Managers initialize and function
3. **Utilities Last:** Port utility functions
   - Low risk, stateless functions
   - Test: Utilities work correctly

**Phase 4: Integration Testing**
1. Test with each dependent module
2. Validate game state management
3. Verify save/load functionality
4. Performance profiling

### Testing Requirements

**Critical Test Cases:**
- Game initialization and lifecycle
- Save/load system functionality
- Database queries return correct data
- Manager singletons accessible
- Utility functions produce correct results

**Integration Test Cases:**
- Core.Actors can access managers
- Core.Combat can query databases
- Core.Items can use synergy database
- UI can access game state

### Risk Assessment

**Very High Risk:**
- Largest module (990 files)
- Highest coupling (depends on 13 other modules)
- Central integration point (8 modules depend on it)
- Mixed concerns (data, management, utilities)

**Mitigation:**
- Refactor before migration (split into subsystems)
- Phased migration by subsystem
- Extensive testing at each phase
- Keep original code as reference

**Breaking Change Risk:**
- Manager singleton patterns may need adaptation
- Extension methods may need reimplementation
- Utility helper methods must maintain exact behavior

### Performance Considerations

**Critical Performance Requirements:**
- Manager access must be fast (singleton pattern)
- Database queries must be efficient
- Utility methods must not introduce overhead

**Profiling Required:**
- Measure manager access performance
- Profile database query performance
- Compare with original implementation

---

**Last Updated:** 2026-01-18
**Documentation Version:** 1.0
**Related Documentation:**
- [Module Manifest](../../../../Docs/Module_Manifest.md)
- [Dependency Matrix](../../../../Docs/Module_Dependency_Matrix.md)
- [Dependency Graph](../../../../Docs/Module_Dependency_Graph.md)
- [Dungeonator MODULE_BOUNDARY](../../Dungeonator/MODULE_BOUNDARY.md)
- [Brave.BulletScript MODULE_BOUNDARY](../../Brave/MODULE_BOUNDARY.md)

**Refactoring Recommendation:**
See [Architecture_Analysis.md](../../../../Docs/Architecture_Analysis.md) for detailed refactoring plan to split this oversized module.
