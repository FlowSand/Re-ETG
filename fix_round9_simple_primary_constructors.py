#!/usr/bin/env python3
"""
Round 9: Manually fix Primary Constructor patterns in simple inheritance cases.

Targets simple cases like:
  public class Foo(params) : Base(params) { }

Converts to:
  public class Foo : Base {
    public Foo(params) : base(params) { }
  }
"""

import os
import re
from pathlib import Path

# Files to fix with their patterns
FILES_TO_FIX = [
    # dfAnimated series - all follow same pattern
    'dfAnimatedFloat.cs',
    'dfAnimatedInt.cs',
    'dfAnimatedColor.cs',
    'dfAnimatedColor32.cs',
    'dfAnimatedQuaternion.cs',
    'dfAnimatedVector2.cs',
    'dfAnimatedVector3.cs',
    'dfAnimatedVector4.cs',
]

def fix_simple_inheritance(file_path):
    """Fix simple primary constructor with base call."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Pattern: public class ClassName(params) : BaseClass(baseParams)
        # Multi-line pattern to handle line breaks
        pattern = r'(\s+)public class (\w+)\(([^)]+)\) : (\w+(?:<[^>]+>)?)\(([^)]+)\)\s*\n\s*\{'

        def replace_func(match):
            indent = match.group(1)
            class_name = match.group(2)
            params = match.group(3)
            base_class = match.group(4)
            base_params = match.group(5)

            result = f"{indent}public class {class_name} : {base_class}\n"
            result += f"{indent}{{\n"
            result += f"{indent}  public {class_name}({params}) : base({base_params})\n"
            result += f"{indent}  {{\n"
            result += f"{indent}  }}\n"
            result += f"\n"  # Blank line before next method
            result += f"{indent}  "  # Indent for next member

            return result

        content = re.sub(pattern, replace_func, content)

        if content != original_content:
            with open(file_path, 'w', encoding='utf-8-sig') as f:
                f.write(content)
            return True
        return False

    except Exception as e:
        print(f"Error processing {file_path}: {e}")
        import traceback
        traceback.print_exc()
        return False

def main():
    """Process specific files."""
    root_dir = Path('Assets/Scripts/ETG')

    if not root_dir.exists():
        print(f"Error: Directory {root_dir} does not exist")
        return

    modified_count = 0

    for filename in FILES_TO_FIX:
        # Find the file
        files = list(root_dir.rglob(filename))
        if not files:
            print(f"Warning: {filename} not found")
            continue

        for file_path in files:
            print(f"Processing: {file_path}")
            if fix_simple_inheritance(file_path):
                print(f"  FIXED")
                modified_count += 1
            else:
                print(f"  No changes needed")

    print(f"\nComplete! Modified {modified_count} files")

if __name__ == '__main__':
    main()
