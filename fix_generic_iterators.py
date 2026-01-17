#!/usr/bin/env python3
"""
Fix generic type iterator references in decompiled C# files.

Issue: Generic<T>.<Method>c__Iterator -> Generic_T__Method_c__Iterator
"""

import os
import re
from pathlib import Path

def fix_generic_iterators(file_path):
    """Fix generic type iterator references."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Fix: Generic<T>.<Method>c__Iterator -> Generic_T__Method_c__Iterator
        # This is more complex because we need to handle the generic parameters

        # Pattern 1: Type<T>.<Method>c__Iterator -> Type_T__Method_c__Iterator
        def fix_single_generic(match):
            type_name = match.group(1)
            generic_param = match.group(2)
            method_name = match.group(3)
            # Replace < > with underscores
            generic_param = generic_param.replace(',', '_').replace(' ', '')
            method_name = method_name.replace('<', '').replace('>', '')
            return f'{type_name}_{generic_param}__{method_name}c__'

        # Match: TypeName<GenericParam>.<MethodName>c__
        content = re.sub(
            r'(\w+)<([^>]+)>\.<([^>]+)>c__',
            fix_single_generic,
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

        if fix_generic_iterators(cs_file):
            modified_files += 1

    print(f"\nComplete!")
    print(f"Total files: {total_files}")
    print(f"Modified files: {modified_files}")
    print(f"Unchanged files: {total_files - modified_files}")

if __name__ == '__main__':
    main()
