# Module: Core.Items

## Identity

- **Namespace:** Global (no specific namespace)
- **Directory:** `Assets/Scripts/ETG/Core/Items`
- **Files:** 175
- **Category:** Core Subsystem (Domain Layer)
- **Priority:** ⭐⭐ HIGH

## Purpose

Equipment, weapons, and pickup systems managing all player inventory, guns, active items, passive items, and item synergies. This module defines the "roguelike" aspect of Enter the Gungeon with its vast item variety.

## Responsibilities

- Gun definitions and weapon stats
- Active items (cooldown-based abilities)
- Passive items (stat modifiers and effects)
- Pickup spawning and collection
- Item synergy management
- Inventory system
- Item transformation and unlocks

## Subsystems

### Guns Subsystem
- Gun definitions and stats
- Gun firing modes
- Reload mechanics
- Gun transformations (formes)

### Active Items Subsystem
- Active items with cooldowns
- Item activation logic
- Cooldown management

### Passive Items Subsystem
- Passive stat modifiers
- Persistent effects
- Item combinations

### Pickups Subsystem
- Collectible items (keys, armor, hearts)
- Pickup spawning
- Currency system

## Public API

### Key Classes

**Guns:**
- `Gun` - Base gun class, inherits from PickupObject
- `GunFormeData` - Gun transformation/evolution data
- `ProjectileModule` - Gun projectile configuration
- Gun-specific implementations

**Active Items:**
- `PlayerItem` - Base class for active items
- Active item implementations (hundreds of items)

**Passive Items:**
- `PassiveItem` - Base class for passive items
- Passive item implementations

**Pickups:**
- `PickupObject` - Base class for all pickups
- `Currency` - Shell/money system
- Key, armor, heart pickups

### Key Interfaces

- Item activation interfaces
- Synergy interfaces

## Dependencies

### External Assemblies

- System (191 files) - Collections, LINQ
- UnityEngine (97 files) - Rendering, physics, animation

### Internal Modules (Core.Items uses)

**Foundation Dependencies:**
- **Dungeonator** (41 files) - Item placement and spawning
- **Pathfinding** (1 file) - Companion AI navigation

**Core Dependencies:**
- **Core.Core** (58 files) - Base classes and enums
- **Core.Systems** (61 files) - Item databases, synergy system
- **Core.Actors** (76 files) - HIGH - Items affect player/enemies
- **Core.Combat** (36 files) - Guns create projectiles

## Dependents

### Modules Using Core.Items

- **Core.UI** (22 files) - Inventory display, item tooltips
- **Core.Dungeon** (13 files) - Item chest spawning, shop system

**Total Dependents:** 35 files across 2 modules

**Dependency Strength:** MEDIUM - Used by presentation and dungeon systems

## Internal Structure

```
Core/Items/
├── Guns/
│   ├── Gun.cs (base)
│   ├── GunFormeData.cs
│   └── Gun implementations (100+ files)
├── Active/
│   ├── PlayerItem.cs (base)
│   └── Active item implementations
├── Passive/
│   ├── PassiveItem.cs (base)
│   └── Passive item implementations
└── Pickups/
    ├── PickupObject.cs (base)
    ├── Currency system
    └── Collectible implementations
```

## Architectural Notes

**Layer:** Domain Logic (Layer 2) - Game content and mechanics

**Design Patterns:**
- Template Method pattern (item base classes)
- Strategy pattern (item effects)
- Factory pattern (item spawning)
- Composite pattern (synergies)

**Coupling:** HIGH - Depends on Actors and Combat, but relatively independent

**Circular Dependencies:** None

## Migration Guidance

### Critical Module Assessment

**Status:** HIGH PRIORITY - Defines game content and variety
**Migration Priority:** Phase 2 (with other Domain modules)
**Risk Level:** MEDIUM - Mostly data-driven, lower risk than combat

### Migration Options

**Option A: Direct Port (RECOMMENDED)**
- Port all item implementations
- Maintain item stats and balance
- Preserve synergy system
- Estimated effort: HIGH (175 files, mostly data)

**Option B: Selective Port**
- Port only essential items first
- Add remaining items incrementally
- Risk: Incomplete content
- Estimated effort: MEDIUM (but incomplete)

### Key Integration Points to Preserve

**Critical Classes:**
- `Gun` - Base for all weapons
- `PlayerItem` - Base for active items
- `PassiveItem` - Base for passive items
- `PickupObject` - Base for all collectibles

**Critical Systems:**
- Item stat system (damage, fire rate, reload, etc.)
- Synergy system (item combinations)
- Gun forme system (gun transformations)
- Pickup collection logic

**Critical Data:**
- Gun stats and balance
- Item effect parameters
- Synergy definitions
- Unlock progression

### Dependencies to Address First

**Foundation Dependencies:**
1. **Dungeonator** (41 files) - Item placement
   - Port with Foundation layer
   - Required for item spawning

**Core Dependencies:**
2. **Core.Actors** (76 files) - HIGH dependency
   - Items modify actor stats
   - Must port Core.Actors first or in parallel

3. **Core.Combat** (36 files) - Gun projectiles
   - Guns depend on combat system
   - Must port Core.Combat first

4. **Core.Systems** (61 files) - Item databases
   - Item definitions reference databases
   - Must port Core.Systems first

### Migration Strategy Recommendation

**Phase 1: Prepare Dependencies**
1. Ensure Core.Actors ported (items affect actors)
2. Ensure Core.Combat ported (guns create projectiles)
3. Ensure Core.Systems ported (item databases)
4. Ensure Dungeonator ported (item placement)

**Phase 2: Port Base Classes**
1. Port PickupObject base
2. Port Gun, PlayerItem, PassiveItem bases
3. Test: Base functionality works

**Phase 3: Port Item Categories**
1. **Guns First:**
   - Port all gun implementations
   - Validate gun stats and firing
   - Test gun transformations (formes)
2. **Passive Items Second:**
   - Port passive item implementations
   - Test stat modifications
3. **Active Items Third:**
   - Port active item implementations
   - Test cooldown system

**Phase 4: Synergy System**
1. Port synergy processing
2. Validate synergy triggers
3. Test complex item interactions

**Phase 5: Pickups and Currency**
1. Port pickup collectibles
2. Test currency system
3. Validate shop interactions

### Testing Requirements

**Critical Test Cases:**
- Guns spawn with correct stats
- Gun firing mechanics work correctly
- Passive items modify stats appropriately
- Active items trigger and have correct cooldowns
- Synergies trigger when items combined
- Pickups collected correctly
- Currency system functions
- Gun formes/transformations work

**Content Validation:**
- All guns present and functional
- All active items work
- All passive items apply effects
- All synergies trigger correctly

**Performance Requirements:**
- Inventory queries efficient
- Item effect processing minimal overhead
- No GC allocation on item use

### Synergy System

**Complexity:**
- 100+ synergies defined in AdvancedSynergyDatabase (Core.Systems)
- Synergies modify multiple item properties
- Some synergies transform gun behavior

**Migration Strategy:**
- Port synergy definitions from Core.Systems
- Port synergy triggering logic
- Test each synergy incrementally
- Create synergy test matrix

### Data-Driven Content

**Advantage:**
- Most items are data-driven (stats, effects)
- Lower risk than systems code
- Easy to validate (compare stats)

**Migration Strategy:**
- Extract item data to structured format
- Port item implementations
- Validate stats match original
- Test gameplay feel for each item

### Risk Assessment

**Medium Risk:**
- Item stat balance (must preserve exactly)
- Synergy system (complex interactions)
- Gun transformation system

**Low Risk:**
- Item base classes (straightforward)
- Pickup collection (simple logic)
- Currency system (well-defined)

**Mitigation:**
- Automated test suite for item stats
- Synergy validation matrix
- Side-by-side comparison with original
- Incremental testing per item category

### Content Completeness

**175 Files:**
- ~100 gun implementations
- ~50 active items
- ~25 passive items
- Pickup implementations
- Base classes and systems

**Migration Priority:**
- Essential content first (starting guns, basic items)
- Common items second
- Rare/unlockable items third
- Secret items last

### Performance Considerations

**Performance Areas:**
- Inventory queries (must be fast)
- Item effect processing (minimal overhead)
- Synergy checks (optimized lookups)

**Optimization:**
- Cache item effects
- Efficient synergy lookups (hash-based)
- Minimize per-frame processing

---

**Last Updated:** 2026-01-18
**Documentation Version:** 1.0
**Related Documentation:**
- [Module Manifest](../../../../Docs/Module_Manifest.md)
- [Dependency Matrix](../../../../Docs/Module_Dependency_Matrix.md)
- [Core.Actors MODULE_BOUNDARY](../Actors/MODULE_BOUNDARY.md)
- [Core.Combat MODULE_BOUNDARY](../Combat/MODULE_BOUNDARY.md)
- [Core.Systems MODULE_BOUNDARY](../Systems/MODULE_BOUNDARY.md)
