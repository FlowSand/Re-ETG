#!/usr/bin/env python3
"""Clean up redundant Unity engine module DLLs from lib/ directories"""

import os
import shutil
from pathlib import Path

# Directories
lib_dirs = [
    Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG\lib"),
    Path(r"D:\Github\Re-ETG\Assets\_RawDump\C#\Assembly-CSharp\lib"),
]
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
total_files = 0

for lib_dir in lib_dirs:
    if not lib_dir.exists():
        print(f"[SKIP] Directory not found: {lib_dir}")
        continue

    print(f"\n[PROCESSING] {lib_dir}")
    print("=" * 60)

    for dll_name in dlls_to_remove:
        dll_path = lib_dir / dll_name
        meta_path = lib_dir / f"{dll_name}.meta"

        if dll_path.exists():
            # Create unique backup name to avoid conflicts between directories
            lib_identifier = lib_dir.parts[-1]  # "lib" or directory name
            if lib_identifier == "lib":
                # Use parent directory name for uniqueness
                lib_identifier = lib_dir.parts[-2] if len(lib_dir.parts) > 2 else "lib"

            # Check if file already exists in backup, create unique name
            backup_dll_path = backup_dir / dll_name
            if backup_dll_path.exists():
                backup_dll_path = backup_dir / f"{dll_name}.{lib_identifier}"

            shutil.move(str(dll_path), str(backup_dll_path))
            print(f"[OK] Moved {dll_name}")
            moved_count += 1
            total_files += 1

        if meta_path.exists():
            # Same logic for .meta files
            backup_meta_path = backup_dir / f"{dll_name}.meta"
            if backup_meta_path.exists():
                lib_identifier = lib_dir.parts[-2] if len(lib_dir.parts) > 2 else "lib"
                backup_meta_path = backup_dir / f"{dll_name}.meta.{lib_identifier}"

            shutil.move(str(meta_path), str(backup_meta_path))
            print(f"[OK] Moved {dll_name}.meta")
            total_files += 1

    # Clean up orphaned .meta files
    for meta_file in lib_dir.glob("*.meta"):
        dll_file = lib_dir / meta_file.name.replace(".meta", "")
        if not dll_file.exists():
            shutil.move(str(meta_file), str(backup_dir / meta_file.name))
            print(f"[OK] Moved orphaned {meta_file.name}")
            total_files += 1

print(f"\n{'='*60}")
print(f"Cleanup complete: {moved_count} DLLs moved from {len(lib_dirs)} directories")
print(f"Total files moved: {total_files}")
print(f"Backup location: {backup_dir}")

# Print remaining DLLs in each directory
for lib_dir in lib_dirs:
    if lib_dir.exists():
        print(f"\nRemaining DLLs in {lib_dir.name}/:")
        dll_list = sorted(lib_dir.glob("*.dll"))
        if dll_list:
            for dll in dll_list:
                size = dll.stat().st_size / 1024
                print(f"  - {dll.name} ({size:.1f} KB)")
        else:
            print("  (no DLL files)")
