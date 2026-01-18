# Module: Brave.BulletScript

## Identity

- **Namespace:** `Brave.BulletScript`
- **Directory:** `Assets/Scripts/ETG/Brave`
- **Files:** 14
- **Category:** External (Bullet Pattern Framework)
- **Priority:** ⭐⭐⭐ CRITICAL

## Purpose

Domain Specific Language (DSL) framework for defining complex bullet patterns used in boss fights and enemy attacks. This is the core system that enables Enter the Gungeon's challenging and intricate boss bullet patterns.

## Responsibilities

- Bullet pattern script parsing and interpretation
- Bullet pattern execution and choreography
- Choreographed attack sequences
- Pattern timing and synchronization
- Integration with combat and enemy systems

## Public API

### Key Classes

- `Script` - Base class for bullet pattern scripts
- `BulletScriptSource` - Source definition for bullet patterns
- `BulletBank` - Manages bullet pooling and spawning
- `Bullet` - Individual bullet behavior controller

### Key Methods

- `Script.Start()` - Initialize and begin pattern execution
- `Script.Update()` - Per-frame pattern update
- Pattern definition methods (implementation-specific)

### Key Enums

- Pattern timing enums
- Bullet type enums

## Dependencies

### External Assemblies

- System (16 files) - Core .NET types, collections
- UnityEngine (11 files) - Transform, GameObject, physics

### Internal Modules (Brave.BulletScript uses)

- Dungeonator (1 file) - Spatial context for bullet patterns

## Dependents

### Modules Using Brave.BulletScript

- **Core.Systems** (224 files) - VERY HIGH usage - Bullet pattern definitions stored as data
- **Core.Combat** (37 files) - Pattern execution and choreography
- **Core.Core** (6 files) - Framework integration
- **Core.Dungeon** (1 file) - Room-specific patterns

**Total Dependents:** 268 files across 4 modules

**Dependency Strength:** CRITICAL - Second most critical external module

## Internal Structure

```
Brave/
├── Core DSL (Script.cs, BulletScriptSource.cs)
├── Execution (BulletBank.cs, Bullet.cs)
├── Pattern primitives (timing, spawning, movement)
└── Integration (combat system hooks)
```

## Architectural Notes

**Layer:** Foundation (Layer 0) - Core combat framework

**Design Pattern:**
- Interpreter pattern (DSL execution)
- Object Pool pattern (bullet management)
- Command pattern (bullet actions)

**Coupling:** Medium - Used heavily by Systems and Combat, but relatively isolated

**Circular Dependencies:** None

## Migration Guidance

### Critical Module Assessment

**Status:** CRITICAL - Essential for boss fight complexity
**Migration Priority:** Phase 1 (Must migrate early with Dungeonator)
**Risk Level:** HIGH - Boss fights are signature gameplay feature

### Migration Options

**Option A: Port Entire DSL Framework (STRONGLY RECOMMENDED)**
- Preserve all bullet patterns as-is
- Boss fights remain intact
- Minimal risk to gameplay experience
- Estimated effort: MEDIUM

**Option B: Rewrite Patterns in Native Code**
- Translate all DSL scripts to target language
- Extremely time-consuming (224+ pattern files)
- High risk of behavior changes
- Loss of data-driven pattern design
- Estimated effort: VERY HIGH (not recommended)

**Option C: Create Equivalent DSL**
- Design new bullet pattern DSL for target platform
- Requires translation layer or converter
- High complexity, uncertain results
- Estimated effort: VERY HIGH

### Key Integration Points to Preserve

**Critical Classes:**
- `Script` base class - All patterns inherit from this
- `BulletBank` - Manages bullet lifecycle
- Pattern execution API must remain compatible

**Critical Behaviors:**
- Pattern timing precision (frame-perfect synchronization)
- Bullet spawn ordering
- Choreography sequencing
- Integration with enemy AI states

### Dependencies to Address First

1. **Dungeonator** (1 file)
   - Minimal dependency for spatial context
   - Port Dungeonator first (already planned)

2. **UnityEngine** (11 files)
   - Transform, GameObject dependencies
   - Create platform abstraction layer

### Migration Strategy Recommendation

**Phase 1: Analysis**
1. Catalog all bullet patterns (224 files in Core.Systems)
2. Identify DSL features actually used
3. Create migration test plan for representative patterns

**Phase 2: Port Framework**
1. Port Brave.BulletScript module as-is
2. Maintain exact namespace and API
3. Create platform compatibility layer for Unity dependencies

**Phase 3: Validate Patterns**
1. Test sample patterns from each boss
2. Verify timing accuracy (critical for difficulty)
3. Performance profiling (bullet pooling efficiency)

**Phase 4: Data Migration**
1. Port all 224 pattern definition files
2. Verify each pattern compiles/runs
3. Visual validation of pattern behavior

### Testing Requirements

**Critical Test Cases:**
- All boss patterns execute correctly
- Timing synchronization accurate to frame
- Bullet pooling performs adequately
- Pattern choreography sequences work
- Integration with enemy AI states functional

**Performance Requirements:**
- Handle 100+ bullets on screen simultaneously
- Minimal GC allocation (pooling must work)
- Maintain 60 FPS during dense patterns

### Data Files Affected

**Core.Systems Pattern Definitions:**
- 224 files reference Brave.BulletScript
- These are data files defining boss patterns
- Must migrate alongside framework

### Alternative Technology Assessment

**Could replace with:**
- Custom pattern editor/framework (very high effort)
- Procedural pattern generation (loses hand-crafted design)
- Simplified pattern system (loses game's signature complexity)

**Recommendation:** DO NOT REPLACE - port existing framework

### Risk Assessment

**High Risk:**
- Boss fights are signature gameplay feature
- Pattern behavior changes would break difficulty balance
- Community expects authentic boss patterns

**Mitigation:**
- Port entire framework with high fidelity
- Extensive testing with original game reference
- Frame-by-frame pattern comparison

---

**Last Updated:** 2026-01-18
**Documentation Version:** 1.0
**Related Documentation:**
- [Module Manifest](../../../Docs/Module_Manifest.md)
- [Dependency Matrix](../../../Docs/Module_Dependency_Matrix.md)
- [Dependency Graph](../../../Docs/Module_Dependency_Graph.md)
- [Dungeonator MODULE_BOUNDARY](../Dungeonator/MODULE_BOUNDARY.md)
