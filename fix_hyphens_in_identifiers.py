#!/usr/bin/env python3
"""
Fix hyphens in PrivateImplementationDetails type names.

The GUID-style identifiers contain hyphens which are invalid in C# identifiers.
Replace hyphens with underscores in these type names.
"""

import os
import re
from pathlib import Path

def fix_hyphens_in_type_names(file_path):
    """Fix hyphens in PrivateImplementationDetails type names."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Fix: PrivateImplementationDetails_GUID-WITH-HYPHENS -> PrivateImplementationDetails_GUID_WITH_UNDERSCORES
        # Match the pattern and replace hyphens with underscores
        def replace_hyphens(match):
            guid_part = match.group(1).replace('-', '_')
            return f'PrivateImplementationDetails_{guid_part}'

        content = re.sub(
            r'PrivateImplementationDetails_([0-9A-Fa-f-]+)',
            replace_hyphens,
            content
        )

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

        if fix_hyphens_in_type_names(cs_file):
            modified_files += 1

    print(f"\nComplete!")
    print(f"Total files: {total_files}")
    print(f"Modified files: {modified_files}")
    print(f"Unchanged files: {total_files - modified_files}")

if __name__ == '__main__':
    main()
