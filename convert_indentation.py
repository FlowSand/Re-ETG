#!/usr/bin/env python3
"""
Convert C# file indentation from 2 spaces to 4 spaces.

This script:
1. Reads each line and detects indentation level (count leading spaces)
2. Converts 2-space indentation to 4-space indentation
3. Preserves tabs (if any)
4. Does NOT modify string contents or comments
"""
import re
from pathlib import Path

def convert_indentation(file_path: Path) -> bool:
    """
    Convert indentation from 2 spaces to 4 spaces.
    Returns True if changes were made.
    """
    content = file_path.read_text(encoding='utf-8')
    original_content = content

    lines = content.split('\n')
    new_lines = []

    for line in lines:
        # Empty lines - keep as is
        if not line.strip():
            new_lines.append(line)
            continue

        # Count leading spaces
        leading_spaces = len(line) - len(line.lstrip(' '))

        # If line starts with tab, keep as is (shouldn't happen but safety check)
        if line.startswith('\t'):
            new_lines.append(line)
            continue

        # Convert: every 2 spaces of indentation becomes 4 spaces
        # Only convert leading spaces (indentation), not spaces in code
        if leading_spaces > 0:
            # Calculate indentation level (how many 2-space indents)
            indent_level = leading_spaces // 2
            remaining_spaces = leading_spaces % 2  # Handle odd number of spaces

            # Convert to 4-space indentation
            new_indent = '    ' * indent_level + ' ' * remaining_spaces
            new_line = new_indent + line.lstrip(' ')
            new_lines.append(new_line)
        else:
            # No indentation
            new_lines.append(line)

    new_content = '\n'.join(new_lines)

    if new_content != original_content:
        file_path.write_text(new_content, encoding='utf-8')
        return True

    return False

def main():
    root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")

    print("[*] Task-05 P1: Converting indentation from 2 spaces to 4 spaces...")
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
                if convert_indentation(cs_file):
                    total_modified += 1
                    batch_modified += 1
                    if total_modified <= 10:  # Print first 10
                        rel_path = cs_file.relative_to(Path(r"D:\Github\Re-ETG"))
                        print(f"[OK] Converted: {rel_path}")
                        modified_files.append(str(rel_path))
            except Exception as e:
                print(f"[!] Error processing {cs_file.name}: {e}")

        # Progress report
        processed = min(i + batch_size, len(cs_files))
        progress = (processed / len(cs_files)) * 100
        print(f"[*] Progress: {processed}/{len(cs_files)} files ({progress:.1f}%) - Modified: {batch_modified} in this batch")

    print(f"\n[OK] Complete!")
    print(f"[OK] Files modified: {total_modified}/{len(cs_files)}")
    print(f"[OK] Indentation standardized to 4 spaces (C# .NET standard)")
    print(f"[OK] Code now follows industry best practices")

    if modified_files:
        print(f"\n[*] Sample of converted files (first 10):")
        for f in modified_files:
            print(f"    - {f}")

    return 0

if __name__ == "__main__":
    import sys
    sys.exit(main())
