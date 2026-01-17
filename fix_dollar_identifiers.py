#!/usr/bin/env python3
"""
Fix invalid identifier characters in decompiled C# files.

This script replaces $ characters in identifiers with valid alternatives.
In C#, $ can only be used for string interpolation, not in identifiers.
"""

import os
import re
from pathlib import Path

def fix_dollar_identifiers(file_path):
    """Fix $ characters in identifiers."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Replace $this with _this (common pattern in compiler-generated code)
        content = re.sub(r'\$this\b', '_this', content)

        # Replace other $ patterns in identifiers (like $PC in state machines)
        content = re.sub(r'\$([a-zA-Z_][a-zA-Z0-9_]*)', r'_\1', content)

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

        if fix_dollar_identifiers(cs_file):
            modified_files += 1

    print(f"\nComplete!")
    print(f"Total files: {total_files}")
    print(f"Modified files: {modified_files}")
    print(f"Unchanged files: {total_files - modified_files}")

if __name__ == '__main__':
    main()
