#!/usr/bin/env python3
"""Clean up redundant Unity engine module DLLs from lib/ directory"""

import os
import shutil
from pathlib import Path

# Directories
lib_dir = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG\lib")
backup_dir = Path(r"D:\Github\Re-ETG\Removed_Legacy_DLLs")

# Ensure backup directory exists
backup_dir.mkdir(exist_ok=True)

# DLLs to remove (redundant Unity engine modules)
dlls_to_remove = [
    # Unity Engine Modules (built into Unity 2022)
    "UnityEngine.CoreModule.dll",
    "UnityEngine.AIModule.dll",
    "UnityEngine.AnimationModule.dll",
    "UnityEngine.AudioModule.dll",
    "UnityEngine.ImageConversionModule.dll",
    "UnityEngine.IMGUIModule.dll",
    "UnityEngine.Physics2DModule.dll",
    "UnityEngine.ParticleSystemModule.dll",
    "UnityEngine.PhysicsModule.dll",
    "UnityEngine.ScreenCaptureModule.dll",
    "UnityEngine.TextRenderingModule.dll",
    "UnityEngine.UI.dll",
    "UnityEngine.UnityWebRequestWWWModule.dll",
    "UnityEngine.Timeline.dll",
    # Unity Packages (built-in)
    "Unity.Analytics.DataPrivacy.dll",
    # Mono (built-in)
    "Mono.Security.dll",
]

# Move DLLs and their .meta files
moved_count = 0
for dll_name in dlls_to_remove:
    dll_path = lib_dir / dll_name
    meta_path = lib_dir / f"{dll_name}.meta"

    if dll_path.exists():
        shutil.move(str(dll_path), str(backup_dir / dll_name))
        print(f"[OK] Moved {dll_name}")
        moved_count += 1

    if meta_path.exists():
        shutil.move(str(meta_path), str(backup_dir / f"{dll_name}.meta"))
        print(f"[OK] Moved {dll_name}.meta")

# Clean up orphaned .meta files
for meta_file in lib_dir.glob("*.meta"):
    dll_file = lib_dir / meta_file.name.replace(".meta", "")
    if not dll_file.exists():
        shutil.move(str(meta_file), str(backup_dir / meta_file.name))
        print(f"[OK] Moved orphaned {meta_file.name}")

print(f"\n{'='*60}")
print(f"Cleanup complete: {moved_count} DLLs moved to backup")
print(f"Backup location: {backup_dir}")
print(f"\nRemaining DLLs in lib/:")
for dll in sorted(lib_dir.glob("*.dll")):
    size = dll.stat().st_size / 1024
    print(f"  - {dll.name} ({size:.1f} KB)")
