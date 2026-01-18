#!/usr/bin/env python3
"""Check if commercial Unity assets are installed"""
from pathlib import Path

root = Path(r"D:\Github\Re-ETG\Assets")

# Define asset directories to check (name: possible paths)
assets = {
    "Wwise Audio": ["Plugins/Wwise", "Wwise"],
    "2D Toolkit (tk2d)": ["TK2DROOT", "tk2d", "Plugins/tk2d"],
    "Full Inspector": ["FullInspector", "Plugins/FullInspector"],
    "DaikonForge GUI": ["DaikonForge", "DFGUI", "Plugins/DaikonForge"]
}

print("[*] Checking for commercial Unity assets:\n")

found_count = 0
missing_assets = []

for name, possible_paths in assets.items():
    found = False
    found_path = None

    for path_str in possible_paths:
        check_path = root / path_str
        if check_path.exists():
            found = True
            found_path = check_path
            break

    if found:
        print(f"[OK] {name}: Found at {found_path.relative_to(root)}")
        found_count += 1
    else:
        print(f"[!!] {name}: NOT FOUND (checked: {', '.join(possible_paths)})")
        missing_assets.append(name)

print(f"\n[*] Summary: {found_count}/4 assets found")

if missing_assets:
    print(f"\n[!] Missing assets ({len(missing_assets)}):")
    for asset in missing_assets:
        print(f"    - {asset}")
    print("\n[*] User needs to import these assets into Unity to resolve compilation errors.")
else:
    print("\n[OK] All commercial assets are present!")
