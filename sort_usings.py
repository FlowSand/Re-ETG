#!/usr/bin/env python3
"""
Sort using statements in C# files according to standard conventions:
1. System.* namespaces (alphabetically)
2. Third-party libraries (UnityEngine, etc., alphabetically)
3. Project namespaces (Dungeonator, etc., alphabetically)

Each group is separated by a blank line.
"""
import re
from pathlib import Path
from typing import List, Tuple

def classify_using(using_line: str) -> Tuple[int, str]:
    """
    Classify a using statement and return (priority, namespace).
    Priority: 1 = System, 2 = Third-party, 3 = Project
    """
    # Extract namespace from "using Namespace;"
    match = re.match(r'using\s+([\w.]+)\s*;', using_line.strip())
    if not match:
        return (999, using_line)  # Shouldn't happen

    namespace = match.group(1)

    # System namespaces
    if namespace.startswith('System'):
        return (1, namespace)

    # Known third-party libraries
    third_party = [
        'UnityEngine',
        'UnityEditor',
        'Pathfinding',
        'HutongGames',
        'InControl',
        'Rewired',
        'Steamworks',
        'GOG',
        'tk2dRuntime',
        'FullInspector',
        'FullSerializer',
        'Ionic',
        'Newtonsoft',
    ]

    for lib in third_party:
        if namespace.startswith(lib):
            return (2, namespace)

    # Everything else is project namespace
    return (3, namespace)

def sort_usings(file_path: Path) -> bool:
    """
    Sort using statements in a C# file.
    Returns True if changes were made.
    """
    content = file_path.read_text(encoding='utf-8')
    original_content = content

    lines = content.split('\n')
    new_lines = []

    # Find using block
    using_start = -1
    using_end = -1
    usings = []

    i = 0
    while i < len(lines):
        line = lines[i]
        stripped = line.strip()

        # Start of using block
        if stripped.startswith('using ') and ';' in stripped:
            if using_start == -1:
                using_start = i
            usings.append(line)
            i += 1
            continue

        # End of using block (empty line or non-using line after usings started)
        if using_start != -1 and usings:
            # Check if this is a continuation of using block or end
            if stripped == '' or not stripped.startswith('using '):
                using_end = i
                break

        i += 1

    # If no usings found, return original
    if not usings:
        return False

    # If using_end not found, it's at the end
    if using_end == -1:
        using_end = i

    # Classify and sort usings
    classified = []
    for using in usings:
        priority, namespace = classify_using(using)
        classified.append((priority, namespace, using))

    # Sort by priority then namespace
    classified.sort(key=lambda x: (x[0], x[1].lower()))

    # Group by priority and format with blank lines between groups
    sorted_usings = []
    last_priority = None

    for priority, namespace, using_line in classified:
        # Add blank line between groups
        if last_priority is not None and last_priority != priority:
            sorted_usings.append('')
        sorted_usings.append(using_line)
        last_priority = priority

    # Reconstruct file
    new_lines = lines[:using_start] + sorted_usings + lines[using_end:]
    new_content = '\n'.join(new_lines)

    if new_content != original_content:
        file_path.write_text(new_content, encoding='utf-8')
        return True

    return False

def main():
    root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")

    print("[*] Task-05 P1: Sorting using statements in standard order...")
    print(f"[*] Scanning directory: {root}\n")
    print("[*] Sort order:")
    print("    1. System.* namespaces")
    print("    2. Third-party libraries (UnityEngine, Pathfinding, etc.)")
    print("    3. Project namespaces (Dungeonator, etc.)")
    print()

    # Find all C# files
    cs_files = list(root.rglob("*.cs"))

    print(f"[*] Found {len(cs_files)} C# files to process\n")

    total_modified = 0
    modified_files = []

    # Process in batches for progress reporting
    batch_size = 500
    for i in range(0, len(cs_files), batch_size):
        batch = cs_files[i:i+batch_size]
        batch_modified = 0

        for cs_file in batch:
            try:
                if sort_usings(cs_file):
                    total_modified += 1
                    batch_modified += 1
                    if total_modified <= 10:  # Print first 10
                        rel_path = cs_file.relative_to(Path(r"D:\Github\Re-ETG"))
                        print(f"[OK] Sorted: {rel_path}")
                        modified_files.append(str(rel_path))
            except Exception as e:
                print(f"[!] Error processing {cs_file.name}: {e}")

        # Progress report
        processed = min(i + batch_size, len(cs_files))
        progress = (processed / len(cs_files)) * 100
        print(f"[*] Progress: {processed}/{len(cs_files)} files ({progress:.1f}%) - Modified: {batch_modified} in this batch")

    print(f"\n[OK] Complete!")
    print(f"[OK] Files modified: {total_modified}/{len(cs_files)}")
    print(f"[OK] Using statements now follow .NET standard conventions")
    print(f"[OK] Code is more readable and consistent")

    if modified_files:
        print(f"\n[*] Sample of sorted files (first 10):")
        for f in modified_files:
            print(f"    - {f}")

    return 0

if __name__ == "__main__":
    import sys
    sys.exit(main())
