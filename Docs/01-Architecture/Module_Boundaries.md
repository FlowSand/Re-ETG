# ETG Module Boundaries Index

**Purpose:** Central index of all module boundary documentation for AI-assisted architecture migration
**Last Updated:** 2026-01-18

---

## Table of Contents

1. [Overview](#overview)
2. [Module Directory](#module-directory)
3. [Migration Decision Tree](#migration-decision-tree)
4. [Module Categories](#module-categories)
5. [Quick Reference](#quick-reference)

---

## Overview

This document serves as the **central index** for all module boundary documentation in the ETG codebase. Each module has a dedicated `MODULE_BOUNDARY.md` file at its root directory providing detailed information about:

- Module identity and metrics
- Purpose and responsibilities
- Public API surface
- Dependencies and dependents
- Internal structure
- Migration guidance

**Total Modules:** 22 (13 external + 9 core subsystems)
**Documentation Status:** See module directory below

---

## Module Directory

### Foundation Layer - External Modules

| Module | Path | Files | Priority | Boundary Doc |
|--------|------|-------|----------|--------------|
| Dungeonator | [Assets/Scripts/ETG/Dungeonator](../Assets/Scripts/ETG/Dungeonator/MODULE_BOUNDARY.md) | 67 | ⭐⭐⭐ CRITICAL | ✅ Available |
| Brave.BulletScript | [Assets/Scripts/ETG/Brave](../Assets/Scripts/ETG/Brave/MODULE_BOUNDARY.md) | 14 | ⭐⭐⭐ CRITICAL | ✅ Available |
| InControl | [Assets/Scripts/ETG/InControl](../Assets/Scripts/ETG/InControl/MODULE_BOUNDARY.md) | 391 | ⭐⭐ HIGH | Template |
| FullInspector | [Assets/Scripts/ETG/FullInspector](../Assets/Scripts/ETG/FullInspector/MODULE_BOUNDARY.md) | 188 | ⭐⭐ HIGH | Template |
| Pathfinding | [Assets/Scripts/ETG/Pathfinding](../Assets/Scripts/ETG/Pathfinding/MODULE_BOUNDARY.md) | 4 | ⭐⭐ HIGH | Template |
| tk2dRuntime | [Assets/Scripts/ETG/tk2dRuntime](../Assets/Scripts/ETG/tk2dRuntime/MODULE_BOUNDARY.md) | 14 | ⭐ MEDIUM | Template |
| HutongGames.PlayMaker | [Assets/Scripts/ETG/HutongGames](../Assets/Scripts/ETG/HutongGames/MODULE_BOUNDARY.md) | N/A | LOW | Template |
| DaikonForge | [Assets/Scripts/ETG/DaikonForge](../Assets/Scripts/ETG/DaikonForge/MODULE_BOUNDARY.md) | 61 | LOW | Template |
| FullSerializer | [Assets/Scripts/ETG/FullSerializer](../Assets/Scripts/ETG/FullSerializer/MODULE_BOUNDARY.md) | 52 | LOW | Template |
| XInputDotNetPure | [Assets/Scripts/ETG/XInputDotNetPure](../Assets/Scripts/ETG/XInputDotNetPure/MODULE_BOUNDARY.md) | 9 | LOW | Template |
| SimplexNoise | [Assets/Scripts/ETG/SimplexNoise](../Assets/Scripts/ETG/SimplexNoise/MODULE_BOUNDARY.md) | 3 | LOW | Template |
| Kvant | [Assets/Scripts/ETG/Kvant](../Assets/Scripts/ETG/Kvant/MODULE_BOUNDARY.md) | N/A | LOW | Template |
| TestSimpleRNG | [Assets/Scripts/ETG/TestSimpleRNG](../Assets/Scripts/ETG/TestSimpleRNG/MODULE_BOUNDARY.md) | 1 | LOW | Template |

### Core Layer - System Modules

| Module | Path | Files | Priority | Boundary Doc |
|--------|------|-------|----------|--------------|
| Core.Systems | [Assets/Scripts/ETG/Core/Systems](../Assets/Scripts/ETG/Core/Systems/MODULE_BOUNDARY.md) | 990 | ⭐⭐⭐ CRITICAL | ✅ Available |
| Core.Core | [Assets/Scripts/ETG/Core/Core](../Assets/Scripts/ETG/Core/Core/MODULE_BOUNDARY.md) | 377 | ⭐⭐⭐ CRITICAL | Template |

### Domain Layer - Game Logic Modules

| Module | Path | Files | Priority | Boundary Doc |
|--------|------|-------|----------|--------------|
| Core.Actors | [Assets/Scripts/ETG/Core/Actors](../Assets/Scripts/ETG/Core/Actors/MODULE_BOUNDARY.md) | 167 | ⭐⭐ HIGH | Template |
| Core.Combat | [Assets/Scripts/ETG/Core/Combat](../Assets/Scripts/ETG/Core/Combat/MODULE_BOUNDARY.md) | 172 | ⭐⭐ HIGH | Template |
| Core.Items | [Assets/Scripts/ETG/Core/Items](../Assets/Scripts/ETG/Core/Items/MODULE_BOUNDARY.md) | 175 | ⭐⭐ HIGH | Template |
| Core.Dungeon | [Assets/Scripts/ETG/Core/Dungeon](../Assets/Scripts/ETG/Core/Dungeon/MODULE_BOUNDARY.md) | 125 | ⭐⭐ HIGH | Template |

### Presentation Layer - UI/Audio/VFX Modules

| Module | Path | Files | Priority | Boundary Doc |
|--------|------|-------|----------|--------------|
| Core.UI | [Assets/Scripts/ETG/Core/UI](../Assets/Scripts/ETG/Core/UI/MODULE_BOUNDARY.md) | 100 | ⭐ MEDIUM | Template |
| Core.Audio | [Assets/Scripts/ETG/Core/Audio](../Assets/Scripts/ETG/Core/Audio/MODULE_BOUNDARY.md) | 150 | ⭐ MEDIUM | Template |
| Core.VFX | [Assets/Scripts/ETG/Core/VFX](../Assets/Scripts/ETG/Core/VFX/MODULE_BOUNDARY.md) | 67 | LOW | Template |

---

## Migration Decision Tree

Use this decision tree to prioritize which modules to migrate first:

```
START HERE
│
├─ Is module CRITICAL to core gameplay?
│  ├─ YES → Migrate Early (Phase 1-2)
│  │        • Dungeonator
│  │        • Brave.BulletScript
│  │        • Core.Core
│  │        • Core.Systems
│  │
│  └─ NO → Continue to next question
│
├─ Is module heavily coupled to other modules?
│  ├─ YES → Migrate Mid-Phase (Phase 2-3)
│  │        • Core.Actors
│  │        • Core.Combat
│  │        • Core.Items
│  │        • Core.Dungeon
│  │
│  └─ NO → Continue to next question
│
├─ Is module presentation-focused (UI/Audio/VFX)?
│  ├─ YES → Migrate Late (Phase 3)
│  │        • Core.UI
│  │        • Core.Audio
│  │        • Core.VFX
│  │
│  └─ NO → Continue to next question
│
└─ Is module replaceable with standard library?
   ├─ YES → Replace Rather Than Port (Phase 4)
   │        • FullInspector (→ JsonUtility)
   │        • InControl (→ New Input System)
   │        • tk2dRuntime (→ Native 2D)
   │        • PlayMaker (→ Code directly)
   │
   └─ NO → Evaluate case-by-case
```

### Migration Priority Summary

**Phase 1: Foundation (Port First)**
- Dungeonator ⭐⭐⭐
- Brave.BulletScript ⭐⭐⭐
- Core.Core ⭐⭐⭐

**Phase 2: Core & Domain**
- Core.Systems ⭐⭐⭐ (split into subsystems)
- Core.Combat ⭐⭐
- Core.Actors ⭐⭐
- Core.Items ⭐⭐
- Core.Dungeon ⭐⭐

**Phase 3: Presentation**
- Core.UI ⭐
- Core.Audio ⭐
- Core.VFX

**Phase 4: Replacements**
- FullInspector → Standard JSON
- InControl → New Input System
- tk2dRuntime → Native 2D
- PlayMaker → Remove

---

## Module Categories

### By Layer

**Foundation (External):** 13 modules
- Critical: Dungeonator, Brave.BulletScript
- High: InControl, FullInspector, Pathfinding
- Medium: tk2dRuntime
- Low: 7 utility modules

**Core Systems:** 2 modules
- Both Critical: Core.Systems, Core.Core

**Domain Logic:** 4 modules
- All High Priority: Actors, Combat, Items, Dungeon

**Presentation:** 3 modules
- Medium: UI, Audio
- Low: VFX

### By Dependency Count (Incoming)

**Most Depended Upon:**
1. Dungeonator: 419 file dependencies ⭐⭐⭐
2. Core.Systems: 315 file dependencies ⭐⭐⭐
3. Core.Core: 265 file dependencies ⭐⭐⭐
4. UnityEngine: All modules
5. System (.NET): All modules

**Least Depended Upon (Isolated):**
1. Core.VFX: 8 dependencies
2. Core.UI: 17 dependencies
3. SimplexNoise: ~1 dependency
4. TestSimpleRNG: ~1 dependency

### By Size (Files)

**Largest Modules:**
1. Core.Systems: 990 files (needs splitting)
2. InControl: 391 files
3. Core.Core: 377 files
4. FullInspector: 188 files
5. Core.Items: 175 files

**Smallest Modules:**
1. SimplexNoise: 3 files
2. Pathfinding: 4 files
3. XInputDotNetPure: 9 files
4. Brave.BulletScript: 14 files
5. tk2dRuntime: 14 files

---

## Quick Reference

### Critical Modules (MUST PORT)
```
Dungeonator       → Spatial/dungeon framework (399 dependencies)
Brave.BulletScript → Boss bullet patterns (268 dependencies)
Core.Systems      → Game state and management (990 files)
Core.Core         → Framework base classes (377 files)
```

### Replaceable Modules (CAN SUBSTITUTE)
```
FullInspector  → Replace with Unity JsonUtility / Newtonsoft.Json
InControl      → Replace with Unity Input System
tk2dRuntime    → Replace with Unity native 2D (Sprite Renderer + Tilemap)
PlayMaker      → Remove, write behaviors in code
DaikonForge    → Replace with Unity UI (uGUI)
```

### Migration Guidance Links
- **Full Module Manifest:** [Module_Manifest.md](Module_Manifest.md)
- **Dependency Matrix:** [Module_Dependency_Matrix.md](Module_Dependency_Matrix.md)
- **Dependency Graph:** [Module_Dependency_Graph.md](Module_Dependency_Graph.md)
- **Analysis Data (JSON):** [dependency_analysis.json](dependency_analysis.json)

---

## How to Use This Index

### For AI-Assisted Analysis:
1. Start with [Module_Manifest.md](Module_Manifest.md) for complete module descriptions
2. Review [Module_Dependency_Graph.md](Module_Dependency_Graph.md) for visual architecture
3. Check individual `MODULE_BOUNDARY.md` files for module-specific details
4. Use [dependency_analysis.json](dependency_analysis.json) for programmatic analysis

### For Migration Planning:
1. Follow the Migration Decision Tree above
2. Review critical modules first (Dungeonator, Brave.BulletScript, Core.Core)
3. Plan replacement strategy for substitutable modules (FullInspector, InControl)
4. Defer presentation layer (UI, Audio, VFX) to final phase

### For Dependency Analysis:
1. Check [Module_Dependency_Matrix.md](Module_Dependency_Matrix.md) for detailed dependency counts
2. Identify circular dependencies (3 detected, all expected)
3. Use dependency strength classification to prioritize refactoring

---

## Notes

- **Template Status:** Some `MODULE_BOUNDARY.md` files are templates and need completion
- **Priority Levels:** ⭐⭐⭐ Critical | ⭐⭐ High | ⭐ Medium | Low
- **Documentation Format:** All boundary docs follow consistent template structure
- **Maintenance:** Update this index when adding new modules or changing boundaries

---

**Related Documentation:**
- [Module_Manifest.md](Module_Manifest.md) - Complete module catalog
- [Module_Dependency_Matrix.md](Module_Dependency_Matrix.md) - Dependency table
- [Module_Dependency_Graph.md](Module_Dependency_Graph.md) - Visual dependency graph
- [module_definitions.yaml](module_definitions.yaml) - Machine-readable module definitions
- [dependency_analysis.json](dependency_analysis.json) - Raw dependency data

---

**End of Module Boundaries Index**
