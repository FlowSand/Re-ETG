#!/usr/bin/env python3
"""
Remove hexadecimal immediate number comments from C# files.

These comments are decompiler artifacts that show hex representation
of decimal numbers, e.g., "256 /*0x0100*/" or "16 /*0x10*/"

Pattern to remove: /*0x[0-9A-Fa-f]+*/
"""
import re
from pathlib import Path

def remove_hex_comments(file_path: Path) -> int:
    """
    Remove hexadecimal immediate number comments from a file.
    Returns the number of replacements made.
    """
    content = file_path.read_text(encoding='utf-8')
    original_content = content

    # Pattern: whitespace followed by /*0x...*/
    # This captures comments like:
    # - "256 /*0x0100*/"
    # - "16 /*0x10*/"
    # - "new Stack<int>(256 /*0x0100*/)"
    pattern = r'\s*/\*0x[0-9A-Fa-f]+\*/'

    # Replace with empty string (remove the comment)
    new_content = re.sub(pattern, '', content)

    # Count replacements
    replacements = content.count('/*0x') - new_content.count('/*0x')

    if new_content != original_content:
        file_path.write_text(new_content, encoding='utf-8')
        return replacements

    return 0

def main():
    root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")

    print("[*] Task-05 P2: Removing hexadecimal immediate number comments...")
    print(f"[*] Scanning directory: {root}\n")
    print("[*] Target pattern: /*0x[0-9A-Fa-f]+*/")
    print("[*] Example: '256 /*0x0100*/' -> '256'\n")

    # Find all C# files
    cs_files = list(root.rglob("*.cs"))

    print(f"[*] Found {len(cs_files)} C# files to scan\n")

    total_files_modified = 0
    total_replacements = 0
    modified_files = []

    for cs_file in cs_files:
        try:
            replacements = remove_hex_comments(cs_file)
            if replacements > 0:
                total_files_modified += 1
                total_replacements += replacements
                if total_files_modified <= 20:  # Print first 20
                    rel_path = cs_file.relative_to(Path(r"D:\Github\Re-ETG"))
                    print(f"[OK] Cleaned: {rel_path} ({replacements} comments removed)")
                    modified_files.append((str(rel_path), replacements))
        except Exception as e:
            print(f"[!] Error processing {cs_file.name}: {e}")

    print(f"\n[OK] Complete!")
    print(f"[OK] Files modified: {total_files_modified}")
    print(f"[OK] Total hex comments removed: {total_replacements}")
    print(f"[OK] Code is now cleaner and more readable")

    if modified_files:
        print(f"\n[*] Files with hex comments removed:")
        for path, count in modified_files:
            print(f"    - {path}: {count} comment(s)")
        if total_files_modified > 20:
            print(f"    ... and {total_files_modified - 20} more files")

    return 0

if __name__ == "__main__":
    import sys
    sys.exit(main())
