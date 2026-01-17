# DLL Cleanup Strategy - lib/ Directory

**Date:** 2026-01-17
**Context:** Resolving Unity Console error: `Plugin 'Assets/Scripts/ETG/lib/UnityEngine.UI.dll' has the same filename as Assembly Definition File 'Packages/com.unity.ugui/Runtime/UnityEngine.UI.asmdef'`

---

## Problem Analysis

The `Assets/Scripts/ETG/lib/` directory contains legacy Unity engine module DLLs from the original game build (likely Unity 5.x era based on decompilation). These DLLs conflict with Unity 2022.3's built-in module system, which uses Assembly Definition Files (.asmdef) instead of precompiled DLLs.

### Root Cause
Unity 2022.3 has all engine modules built-in via packages and .asmdef files. When legacy DLLs with matching filenames exist in Assets/, Unity reports filename conflicts and unpredictable behavior can occur.

---

## DLL Categorization

### Category A: Redundant Unity Engine Modules ❌ REMOVE

These are all built into Unity 2022.3 and MUST be removed:

| DLL | Reason | Unity 2022 Equivalent |
|-----|--------|----------------------|
| UnityEngine.CoreModule.dll | Built-in engine core | UnityEngine.CoreModule (built-in) |
| UnityEngine.AIModule.dll | Built-in AI/NavMesh | UnityEngine.AIModule (built-in) |
| UnityEngine.AnimationModule.dll | Built-in animation | UnityEngine.AnimationModule (built-in) |
| UnityEngine.AudioModule.dll | Built-in audio | UnityEngine.AudioModule (built-in) |
| UnityEngine.ImageConversionModule.dll | Built-in image conversion | UnityEngine.ImageConversionModule (built-in) |
| UnityEngine.IMGUIModule.dll | Built-in IMGUI | UnityEngine.IMGUIModule (built-in) |
| UnityEngine.Physics2DModule.dll | Built-in physics 2D | UnityEngine.Physics2DModule (built-in) |
| UnityEngine.ParticleSystemModule.dll | Built-in particles | UnityEngine.ParticleSystemModule (built-in) |
| UnityEngine.PhysicsModule.dll | Built-in physics 3D | UnityEngine.PhysicsModule (built-in) |
| UnityEngine.ScreenCaptureModule.dll | Built-in screen capture | UnityEngine.ScreenCaptureModule (built-in) |
| UnityEngine.TextRenderingModule.dll | Built-in text rendering | UnityEngine.TextRenderingModule (built-in) |
| **UnityEngine.UI.dll** | **Built-in UI system** | **Packages/com.unity.ugui** (ERROR SOURCE) |
| UnityEngine.UnityWebRequestWWWModule.dll | Built-in web requests | UnityEngine.UnityWebRequestModule (built-in) |
| UnityEngine.Timeline.dll | Built-in timeline | Packages/com.unity.timeline |
| Unity.Analytics.DataPrivacy.dll | Built-in analytics | Packages/com.unity.analytics |

**Total: 15 DLLs to remove**

### Category B: Third-Party Libraries ✅ KEEP

These are legitimate third-party dependencies:

| DLL | Purpose | Action |
|-----|---------|--------|
| GalaxyCSharp.dll | GOG Galaxy SDK | KEEP (有shims替代，但保留原DLL以防需要) |
| PlayMaker.dll | Visual scripting tool | KEEP (第三方资产) |
| dfScriptLite.dll | DFGUI framework | KEEP (第三方UI框架) |

**Total: 3 DLLs to keep**

### Category C: Review Needed ⚠️

| DLL | Analysis | Action |
|-----|----------|--------|
| Mono.Security.dll | Mono security library. Unity 2022 uses Mono 6.x with built-in security. | **REMOVE** (冗余) |
| Assembly-CSharp-firstpass.dll | User assembly from original build. May contain pre-build scripts or plugins. | **REVIEW CODE** → 如果是反编译源代码的编译产物，则**REMOVE**；如果是无源码的第三方插件，则**KEEP** |

---

## Cleanup Strategy

### Step 1: Verify No .asmdef References
**Status:** ✅ VERIFIED
All .asmdef files have empty `precompiledReferences` arrays. No assembly definitions explicitly reference these DLLs.

### Step 2: Move Redundant DLLs to Backup
**Target Location:** `D:\Github\Re-ETG\Removed_Legacy_DLLs\`
**Files to Move:** 15 Unity engine module DLLs + Mono.Security.dll (16 total)

**Why not delete directly?**
- Safety: Can restore if unexpected issues arise
- Documentation: Backup serves as record of what was removed

### Step 3: Handle Assembly-CSharp-firstpass.dll
**Action Required:** Check if source code exists in codebase
**Decision Tree:**
```
Does Assets/Scripts contain the source for Assembly-CSharp-firstpass?
├─ YES → Remove DLL (will be compiled from source)
└─ NO → Keep DLL (third-party plugin without source)
```

### Step 4: Cleanup Meta Files
**Critical:** Also move corresponding .meta files to maintain clean state

### Step 5: Unity Refresh
After cleanup:
1. Reopen Unity Editor
2. Wait for asset import
3. Verify error is gone: `Assets > Refresh` or restart Unity
4. Check Console for 0 errors related to DLL conflicts

---

## Execution Plan

### Phase 1: Prepare Backup Directory
```bash
mkdir D:\Github\Re-ETG\Removed_Legacy_DLLs
```

### Phase 2: Move Category A DLLs
```bash
# Move 15 Unity module DLLs + Mono.Security.dll
move "Assets/Scripts/ETG/lib/UnityEngine.*.dll" "D:\Github\Re-ETG\Removed_Legacy_DLLs\"
move "Assets/Scripts/ETG/lib/Unity.Analytics.DataPrivacy.dll" "D:\Github\Re-ETG\Removed_Legacy_DLLs\"
move "Assets/Scripts/ETG/lib/Mono.Security.dll" "D:\Github\Re-ETG\Removed_Legacy_DLLs\"

# Also move .meta files
move "Assets/Scripts/ETG/lib/UnityEngine.*.dll.meta" "D:\Github\Re-ETG\Removed_Legacy_DLLs\"
move "Assets/Scripts/ETG/lib/Unity.Analytics.DataPrivacy.dll.meta" "D:\Github\Re-ETG\Removed_Legacy_DLLs\"
move "Assets/Scripts/ETG/lib/Mono.Security.dll.meta" "D:\Github\Re-ETG\Removed_Legacy_DLLs\"
```

### Phase 3: Investigate Assembly-CSharp-firstpass.dll
```bash
# Search for source code
grep -r "Assembly-CSharp-firstpass" Assets/Scripts/
# If found → Remove DLL
# If not found → Keep DLL
```

### Phase 4: Git Commit
```bash
git add .
git commit -m "[Task-03] Remove redundant Unity engine module DLLs

- Moved 16 legacy Unity DLLs to Removed_Legacy_DLLs/
- Fixes: UnityEngine.UI.dll filename conflict with com.unity.ugui package
- Kept 3 third-party DLLs: GalaxyCSharp, PlayMaker, dfScriptLite
- Resolves Unity Console error: Plugin filename conflicts

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>"
```

---

## Expected Outcome

### Before Cleanup
```
Unity Console:
❌ Plugin 'Assets/Scripts/ETG/lib/UnityEngine.UI.dll' has the same filename as Assembly Definition File 'Packages/com.unity.ugui/Runtime/UnityEngine.UI.asmdef'
❌ Potential conflicts with 15 other engine modules
```

### After Cleanup
```
Assets/Scripts/ETG/lib/:
✅ GalaxyCSharp.dll (third-party)
✅ PlayMaker.dll (third-party)
✅ dfScriptLite.dll (third-party)
✅ Assembly-CSharp-firstpass.dll (if no source found)

Unity Console:
✅ No DLL filename conflict errors
✅ Ready for Task-04 compilation error fixing
```

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Accidentally remove needed DLL | Low | Medium | Backup to Removed_Legacy_DLLs/ not deleted |
| Unity cache confusion | Medium | Low | Restart Unity after cleanup |
| Third-party code depends on specific DLL version | Low | Medium | Keep GalaxyCSharp/PlayMaker/dfScriptLite untouched |

---

## Rollback Plan

If issues occur after cleanup:
```bash
# Restore all DLLs
move "D:\Github\Re-ETG\Removed_Legacy_DLLs\*.dll" "Assets/Scripts/ETG/lib\"
move "D:\Github\Re-ETG\Removed_Legacy_DLLs\*.meta" "Assets/Scripts/ETG/lib\"

# Git rollback
git reset --hard HEAD~1
```

---

## Success Criteria

Task-03 DLL cleanup is complete when:
- [x] Documentation created (this file)
- [ ] 16 redundant DLLs moved to backup
- [ ] Unity Console error resolved
- [ ] lib/ contains only 3-4 legitimate third-party DLLs
- [ ] Git commit created
- [ ] Unity project compiles (or shows only Task-04 type errors)

---

**Next Task:** Task-04 - Compilation Error Iterative Fixing
