# Module: Core.Combat

## Identity

- **Namespace:** Global (no specific namespace)
- **Directory:** `Assets/Scripts/ETG/Core/Combat`
- **Files:** 172
- **Category:** Core Subsystem (Domain Layer)
- **Priority:** ⭐⭐ HIGH

## Purpose

Combat mechanics system implementing projectiles, damage calculation, status effects, and the core combat loop. This is the heart of Enter the Gungeon's "bullet hell" gameplay.

## Responsibilities

- Projectile spawning and lifecycle management
- Damage calculations and knockback physics
- Status effects and buffs/debuffs
- Projectile modifiers and synergies
- Reload mechanics and gun behavior
- Bullet pattern execution (via Brave.BulletScript)

## Subsystems

### Projectiles Subsystem
- Projectile types and behaviors
- Projectile spawning and pooling
- Trajectory calculations

### Damage Subsystem
- Damage calculation algorithms
- Damage type system
- Knockback and impact effects

### Effects Subsystem
- Status effects (burn, poison, freeze, etc.)
- Visual feedback
- Buff/debuff system

## Public API

### Key Classes

**Projectiles:**
- `Projectile` - Base projectile class
- `ProjectileModule` - Projectile configuration and stats
- `ArcProjectile` - Arc-trajectory projectiles
- `BeamController` - Beam weapon controller
- `HelixProjectile` - Helical projectile patterns

**Damage:**
- `DamageTypeModifier` - Damage type system
- Damage calculation methods
- Knockback handlers

**Effects:**
- `SynergyProcessor` - Synergy effect processing
- Status effect implementations
- Projectile modifier classes

### Key Enums

- `DamageType` - Types of damage (physical, fire, poison, etc.)
- `ProjectileType` - Projectile categories

## Dependencies

### External Assemblies

- System (189 files) - Collections, math
- UnityEngine (97 files) - Physics, rendering, transforms

### Internal Modules (Core.Combat uses)

**Foundation Dependencies:**
- **Brave.BulletScript** (37 files) - HIGH - Boss bullet patterns
- **Dungeonator** (22 files) - Spatial collision, projectile-dungeon interaction
- **FullInspector** (30 files) - Projectile configuration serialization
- **Pathfinding** (3 files) - Homing projectile navigation

**Core Dependencies:**
- **Core.Core** (66 files) - HIGH - Base classes and enums
- **Core.Systems** (83 files) - HIGH - Game managers, databases

## Dependents

### Modules Using Core.Combat

- **Core.Items** (36 files) - Guns create and configure projectiles
- **Core.Actors** (31 files) - Actors affected by combat systems
- **Core.Dungeon** (5 files) - Environmental hazards and damage

**Total Dependents:** 72 files across 3 modules

**Dependency Strength:** HIGH - Core gameplay system

## Internal Structure

```
Core/Combat/
├── Projectiles/
│   ├── Projectile.cs (base)
│   ├── ProjectileModule.cs
│   ├── Specialized projectile types
│   └── Projectile behaviors
├── Damage/
│   ├── Damage calculation
│   ├── DamageTypeModifier.cs
│   └── Knockback system
└── Effects/
    ├── Status effects
    ├── SynergyProcessor.cs
    └── Buff/debuff system
```

## Architectural Notes

**Layer:** Domain Logic (Layer 2) - Core game mechanics

**Design Patterns:**
- Object Pool pattern (projectile pooling)
- Strategy pattern (projectile behaviors)
- Decorator pattern (projectile modifiers)
- Observer pattern (damage events)

**Coupling:** HIGH - Central combat system with many dependencies

**Circular Dependencies:** None

## Migration Guidance

### Critical Module Assessment

**Status:** HIGH PRIORITY - Defines core "bullet hell" gameplay
**Migration Priority:** Phase 2 (after Foundation and Core.Systems)
**Risk Level:** VERY HIGH - Complex physics, bullet patterns, balance

### Migration Options

**Option A: Direct Port (STRONGLY RECOMMENDED)**
- Port entire combat system intact
- Preserve bullet physics exactly
- Maintain game balance
- Estimated effort: HIGH

**Option B: Redesign Combat System**
- Risky - changes core gameplay feel
- May break bullet patterns
- Lose synergy system complexity
- Estimated effort: VERY HIGH (not recommended)

### Key Integration Points to Preserve

**Critical Classes:**
- `Projectile` - Base for all projectiles, must preserve physics
- `ProjectileModule` - Configuration system used extensively
- `SynergyProcessor` - Complex synergy system

**Critical Physics:**
- Projectile trajectory calculations
- Collision detection precision
- Knockback physics
- Bullet speed and acceleration

**Critical Systems:**
- Brave.BulletScript integration (37 files)
- Damage calculation algorithms
- Status effect application
- Synergy processing

### Dependencies to Address First

**Critical External Dependencies:**
1. **Brave.BulletScript** (37 files) - MUST port first
   - Boss and enemy bullet patterns depend on this
   - Cannot migrate combat without bullet script support
   - Port with Foundation layer

2. **Dungeonator** (22 files) - MUST port first
   - Projectile-dungeon collision detection
   - Spatial queries for projectile behavior
   - Port with Foundation layer

**Medium Priority Dependencies:**
3. **FullInspector** (30 files) - Replace or port
   - Projectile configuration serialization
   - Can replace with standard serialization
   - Plan replacement strategy

**Core Dependencies:**
4. **Core.Core** (66 files) - Port together
   - Base classes and enums
   - Must migrate Core.Core first

5. **Core.Systems** (83 files) - Port in parallel
   - Managers and utilities
   - Required for full functionality

### Migration Strategy Recommendation

**Phase 1: Prepare Dependencies**
1. Ensure Brave.BulletScript ported (bullet patterns)
2. Ensure Dungeonator ported (collision detection)
3. Ensure Core.Core ported (base classes)
4. Plan FullInspector replacement

**Phase 2: Port Combat Core**
1. **Projectile System First:**
   - Port Projectile base class
   - Port ProjectileModule configuration
   - Test: Simple projectiles spawn and move correctly
2. **Damage System Second:**
   - Port damage calculation
   - Port knockback physics
   - Test: Damage applied correctly
3. **Effects System Third:**
   - Port status effects
   - Port synergy processor
   - Test: Effects apply and stack correctly

**Phase 3: Bullet Pattern Integration**
1. Integrate with Brave.BulletScript
2. Test boss bullet patterns
3. Validate pattern timing and synchronization

**Phase 4: Synergy System**
1. Port synergy processing
2. Test item synergies
3. Validate complex interactions

**Phase 5: Polish and Balance**
1. Fine-tune projectile physics
2. Validate damage balance
3. Performance optimization

### Testing Requirements

**Critical Test Cases:**
- Projectiles spawn at correct position/velocity
- Projectile collision detection accurate
- Damage calculations produce correct values
- Knockback physics match original
- Status effects apply and tick correctly
- Synergies trigger appropriately
- Bullet patterns execute correctly
- Projectile pooling works efficiently

**Physics Validation:**
- Projectile trajectory precision
- Collision detection accuracy (pixel-perfect)
- Knockback force and direction
- Gravity and arc calculations

**Performance Requirements:**
- Handle 200+ projectiles on screen
- Collision detection optimized (spatial partitioning)
- Minimal GC allocation (pooling required)
- Maintain 60 FPS with dense bullet patterns

### Boss Pattern Validation

**Critical Boss Patterns to Test:**
- First boss (Gatling Gull)
- Mid-game boss (Bullet King)
- End-game boss (Lich)

**Validation Criteria:**
- Pattern timing matches original
- Bullet density correct
- Pattern choreography identical
- Difficulty balanced

### Synergy System Complexity

**Challenge:**
- SynergyProcessor handles complex item interactions
- 100+ synergies defined in Core.Systems databases
- Synergies modify projectile behavior dynamically

**Migration Strategy:**
- Port SynergyProcessor intact
- Test synergies incrementally
- Validate against synergy database
- Keep original as reference

### Risk Assessment

**Very High Risk:**
- Projectile physics (must be pixel-perfect)
- Bullet patterns (timing critical for difficulty)
- Synergy system (complex interactions)
- Game balance (damage, knockback values)

**High Risk:**
- Collision detection (performance and accuracy)
- Status effects (timing and stacking)
- Projectile pooling (memory and performance)

**Mitigation:**
- Frame-by-frame comparison with original
- Automated test suite for projectile physics
- Synergy test matrix
- Performance profiling throughout migration
- Keep original code as reference

### Performance Considerations

**Critical Performance Areas:**
- Projectile update loop (200+ projectiles)
- Collision detection (optimized spatial queries)
- Object pooling (zero allocation)
- Bullet pattern execution (minimal overhead)

**Optimization Requirements:**
- Spatial partitioning for collision detection
- Efficient projectile pooling (no GC pressure)
- SIMD for bulk projectile updates (if available)

**Profiling Required:**
- Projectile update time per frame
- Collision detection overhead
- Memory allocation (should be zero in steady state)
- Bullet pattern execution time

### Data Migration

**Configuration Data:**
- Projectile module configurations (30 files use FullInspector)
- Damage type definitions
- Status effect parameters
- Synergy definitions (in Core.Systems)

**Strategy:**
- Export FullInspector data to JSON
- Convert to target serialization format
- Validate all configurations migrate correctly

---

**Last Updated:** 2026-01-18
**Documentation Version:** 1.0
**Related Documentation:**
- [Module Manifest](../../../../Docs/Module_Manifest.md)
- [Dependency Matrix](../../../../Docs/Module_Dependency_Matrix.md)
- [Brave.BulletScript MODULE_BOUNDARY](../../Brave/MODULE_BOUNDARY.md)
- [Dungeonator MODULE_BOUNDARY](../../Dungeonator/MODULE_BOUNDARY.md)
- [Core.Systems MODULE_BOUNDARY](../Systems/MODULE_BOUNDARY.md)
