#!/usr/bin/env python3
"""
Fix invalid type names and anonymous type references in decompiled C# files.

Issues to fix:
1. <PrivateImplementationDetails> -> PrivateImplementationDetails (in namespace and type references)
2. <>__AnonType -> __AnonType (anonymous types)
3. Other invalid < > combinations in identifiers
"""

import os
import re
from pathlib import Path

def fix_invalid_type_names(file_path):
    """Fix invalid type names in a single file."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Fix: <PrivateImplementationDetails>{GUID} -> PrivateImplementationDetails_{GUID}
        # This handles both namespace declarations and type references
        content = re.sub(
            r'<PrivateImplementationDetails>\{([0-9A-F-]+)\}',
            r'PrivateImplementationDetails_\1',
            content
        )

        # Fix: <>__AnonType -> __AnonType (anonymous types in method signatures)
        content = re.sub(r'<>__AnonType', '__AnonType', content)

        # Fix: other patterns like <>f__AnonymousType
        content = re.sub(r'<>f__', '_f__', content)

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

        if fix_invalid_type_names(cs_file):
            modified_files += 1

    print(f"\nComplete!")
    print(f"Total files: {total_files}")
    print(f"Modified files: {modified_files}")
    print(f"Unchanged files: {total_files - modified_files}")

if __name__ == '__main__':
    main()
