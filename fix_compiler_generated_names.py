#!/usr/bin/env python3
"""
Fix compiler-generated type names and field names in decompiled C# files.

Issues to fix:
1. <$> field names -> valid identifiers
2. Iterator type names: Type.<MethodName>c__IteratorN -> Type__MethodName_c__IteratorN
3. Other < > patterns in type/member references
"""

import os
import re
from pathlib import Path

def fix_compiler_generated_names(file_path):
    """Fix compiler-generated type and field names."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Fix: <$> field names -> __dollar (or remove entirely if assignment target)
        # Pattern: <$>fieldName = value -> __fieldName = value
        content = re.sub(r'<\$>(\w+)', r'__\1', content)

        # Fix: Type.<MethodName>c__IteratorN -> Type__MethodName_c__IteratorN
        # This pattern appears in "new Type.<Method>c__Iterator0()" constructs
        # Match: identifier.<something>c__ -> identifier__something_c__
        def fix_iterator_type(match):
            type_name = match.group(1)
            method_part = match.group(2)
            # Replace dots with double underscores, and < > with underscores
            method_part = method_part.replace('.', '__').replace('<', '').replace('>', '')
            return f'{type_name}__{method_part}c__'

        # Pattern: TypeName.<MethodName>c__Iterator
        content = re.sub(
            r'(\w+)\.<([^>]+)>c__',
            fix_iterator_type,
            content
        )

        # Fix: remaining <...> patterns in type references (but not generic types)
        # This catches patterns like <GetSomething> that aren't generics
        # Be careful not to match actual C# generics like List<int>
        def is_likely_generic(before, after):
            """Check if this looks like a C# generic type."""
            # Generics usually have type names inside: <int>, <string>, <MyType>
            # Compiler-generated have method names: <GetMethod>, <DoSomething>
            # Heuristic: if it starts with capital and has no special chars, might be generic
            inner = after.strip()
            if not inner:
                return False
            # If it's a simple type name (starts with capital), could be generic
            if inner[0].isupper() and inner.replace(',', '').replace(' ', '').replace('.', '').isalnum():
                return True
            return False

        # Replace < > in identifiers but preserve generics
        # This is tricky - we need to be more conservative
        # For now, let's just handle the specific patterns we know are wrong

        # Fix: Class.<>c__DisplayClass patterns
        content = re.sub(r'(\w+)\.<>c__DisplayClass(\d+)', r'\1__DisplayClass\2', content)

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

        if fix_compiler_generated_names(cs_file):
            modified_files += 1

    print(f"\nComplete!")
    print(f"Total files: {total_files}")
    print(f"Modified files: {modified_files}")
    print(f"Unchanged files: {total_files - modified_files}")

if __name__ == '__main__':
    main()
