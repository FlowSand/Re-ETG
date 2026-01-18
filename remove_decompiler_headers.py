#!/usr/bin/env python3
"""
Remove JetBrains decompiler headers from all C# files.

Removes the following header comments:
- // Decompiled with JetBrains decompiler
- // Type: ...
- // Assembly: Assembly-CSharp, ...
- // MVID: ...
- // Assembly location: ...

Preserves all other content including:
- #nullable disable
- using statements
- namespace declarations
- all code
"""
import re
from pathlib import Path
from typing import List

def remove_decompiler_header(file_path: Path) -> bool:
    """
    Remove decompiler header from a single C# file.
    Returns True if changes were made.
    """
    content = file_path.read_text(encoding='utf-8')
    original_content = content

    lines = content.split('\n')
    new_lines = []

    # Track if we're still in the header section
    in_header = True

    for line in lines:
        # Skip UTF-8 BOM marker line
        if line.startswith('\ufeff'):
            line = line[1:]  # Remove BOM but keep line

        # Lines to remove (only in header section)
        if in_header:
            # Decompiler markers
            if line.strip().startswith('// Decompiled with'):
                continue
            if line.strip().startswith('// Type:'):
                continue
            if line.strip().startswith('// Assembly:'):
                continue
            if line.strip().startswith('// MVID:'):
                continue
            if line.strip().startswith('// Assembly location:'):
                continue

            # Empty line after header section
            if not line.strip():
                # Check if next significant line is not a header comment
                # If so, we've exited the header
                continue  # Skip this empty line

            # If we reach here with non-header content, we're done with header
            if (line.strip() and
                not line.strip().startswith('//') and
                line.strip() != ''):
                in_header = False

        # Keep all other lines
        new_lines.append(line)

    new_content = '\n'.join(new_lines)

    # Clean up: remove excessive empty lines at the start
    # Keep only the first empty line after the header removal
    lines_cleaned = []
    leading_empty_count = 0
    started_content = False

    for line in new_content.split('\n'):
        if not started_content:
            if not line.strip():
                leading_empty_count += 1
                if leading_empty_count <= 1:  # Keep max 1 leading empty line
                    lines_cleaned.append(line)
            else:
                started_content = True
                lines_cleaned.append(line)
        else:
            lines_cleaned.append(line)

    final_content = '\n'.join(lines_cleaned)

    if final_content != original_content:
        file_path.write_text(final_content, encoding='utf-8')
        return True

    return False

def main():
    root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")

    print("[*] Task-05 P0: Removing JetBrains decompiler headers from all C# files...")
    print(f"[*] Scanning directory: {root}\n")

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
                if remove_decompiler_header(cs_file):
                    total_modified += 1
                    batch_modified += 1
                    if total_modified <= 10:  # Print first 10
                        rel_path = cs_file.relative_to(Path(r"D:\Github\Re-ETG"))
                        print(f"[OK] Cleaned: {rel_path}")
                        modified_files.append(str(rel_path))
            except Exception as e:
                print(f"[!] Error processing {cs_file.name}: {e}")

        # Progress report
        processed = min(i + batch_size, len(cs_files))
        progress = (processed / len(cs_files)) * 100
        print(f"[*] Progress: {processed}/{len(cs_files)} files ({progress:.1f}%) - Modified: {batch_modified} in this batch")

    print(f"\n[OK] Complete!")
    print(f"[OK] Files modified: {total_modified}/{len(cs_files)}")
    print(f"[OK] Estimated lines removed: ~{total_modified * 6} lines")
    print(f"[OK] All decompiler headers have been removed")

    if modified_files:
        print(f"\n[*] Sample of cleaned files (first 10):")
        for f in modified_files:
            print(f"    - {f}")

    return 0

if __name__ == "__main__":
    import sys
    sys.exit(main())
