#!/usr/bin/env python3
"""
Fix Unicode escape sequences in decompiled C# files.

This script converts Unicode escape sequences back to their actual characters:
- \u003C -> <
- \u003E -> >
- \u0024 -> $
- \u002D -> -
- \u007B -> {
- \u007D -> }
"""

import os
import re
from pathlib import Path

# Define replacements
UNICODE_REPLACEMENTS = {
    r'\\u003C': '<',
    r'\\u003E': '>',
    r'\\u0024': '$',
    r'\\u002D': '-',
    r'\\u007B': '{',
    r'\\u007D': '}',
}

def fix_unicode_escapes(file_path):
    """Fix Unicode escape sequences in a single file."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Apply all replacements
        for pattern, replacement in UNICODE_REPLACEMENTS.items():
            content = re.sub(pattern, replacement, content)

        # Only write if changes were made
        if content != original_content:
            with open(file_path, 'w', encoding='utf-8-sig') as f:
                f.write(content)
            return True
        return False

    except Exception as e:
        print(f"Error processing {file_path}: {e}")
        return False

def main():
    """Process all C# files in the Assets/Scripts/ETG directory."""
    root_dir = Path('Assets/Scripts/ETG')

    if not root_dir.exists():
        print(f"Error: Directory {root_dir} does not exist")
        return

    cs_files = list(root_dir.rglob('*.cs'))
    total_files = len(cs_files)
    modified_files = 0

    print(f"Found {total_files} C# files to process...")

    for i, cs_file in enumerate(cs_files, 1):
        if i % 100 == 0:
            print(f"Progress: {i}/{total_files} files processed, {modified_files} modified")

        if fix_unicode_escapes(cs_file):
            modified_files += 1

    print(f"\nComplete!")
    print(f"Total files: {total_files}")
    print(f"Modified files: {modified_files}")
    print(f"Unchanged files: {total_files - modified_files}")

if __name__ == '__main__':
    main()
