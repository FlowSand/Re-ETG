#!/usr/bin/env python3
"""
Round 6: Fix generic type iterator references with empty method names.

Issue patterns:
1. Generic<T>.<>c__Iterator -> Generic_T___c__Iterator
2. Generic<T>.NestedClass.<>c__Iterator -> Generic_T__NestedClass___c__Iterator
"""

import os
import re
from pathlib import Path

def fix_generic_empty_iterators(file_path):
    """Fix generic type iterator references with empty angle brackets."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Pattern 1: Type<Params>.NestedClass.<>c__Iterator -> Type_Params__NestedClass___c__Iterator
        def fix_nested_empty_iterator(match):
            type_name = match.group(1)
            generic_params = match.group(2)
            nested_class = match.group(3)
            # Clean up generic parameters: remove spaces, replace commas with underscores
            generic_params_clean = generic_params.replace(',', '_').replace(' ', '')
            return f'{type_name}_{generic_params_clean}__{nested_class}___c__'

        # Match: TypeName<Params>.NestedClass.<>c__
        content = re.sub(
            r'(\w+)<([^>]+)>\.(\w+)\.<>c__',
            fix_nested_empty_iterator,
            content
        )

        # Pattern 2: Type<Params>.<>c__Iterator -> Type_Params___c__Iterator
        def fix_empty_iterator(match):
            type_name = match.group(1)
            generic_params = match.group(2)
            # Clean up generic parameters: remove spaces, replace commas with underscores
            generic_params_clean = generic_params.replace(',', '_').replace(' ', '')
            return f'{type_name}_{generic_params_clean}___c__'

        # Match: TypeName<Params>.<>c__
        # This must come after the nested class pattern to avoid partial matches
        content = re.sub(
            r'(\w+)<([^>]+)>\.<>c__',
            fix_empty_iterator,
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

        if fix_generic_empty_iterators(cs_file):
            modified_files += 1
            print(f"Fixed: {cs_file}")

    print(f"\nComplete!")
    print(f"Total files: {total_files}")
    print(f"Modified files: {modified_files}")
    print(f"Unchanged files: {total_files - modified_files}")

if __name__ == '__main__':
    main()
