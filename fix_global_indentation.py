#!/usr/bin/env python3
"""
Fix indentation for types moved to global namespace.

Types in global namespace must have zero indentation at the type declaration level.
"""
import re
from pathlib import Path

def fix_global_namespace_indentation(file_path: Path) -> bool:
    """
    Fix indentation for a file in global namespace.
    Returns True if changes were made.
    """
    content = file_path.read_text(encoding='utf-8')
    original_content = content

    lines = content.split('\n')
    new_lines = []

    # Check if file has namespace declaration
    has_namespace = any(re.match(r'^\s*namespace\s+[\w.]+', line) for line in lines)

    if has_namespace:
        # File still has namespace, skip it
        return False

    # File has no namespace - all top-level type declarations should have zero indentation
    # Look for lines that start with attributes or type declarations and remove leading whitespace

    in_type = False
    brace_depth = 0
    type_start_indent = None

    for i, line in enumerate(lines):
        stripped = line.lstrip()

        # Skip empty lines and comment/using/nullable lines
        if (not stripped or
            stripped.startswith('//') or
            stripped.startswith('/*') or
            stripped.startswith('*') or
            stripped.startswith('using ') or
            stripped.startswith('#')):
            new_lines.append(line)
            continue

        # Check if this is a type declaration (class, struct, enum, interface, delegate)
        is_type_decl = re.match(
            r'^\s*(\[.*\]|public|private|protected|internal|static|sealed|abstract|partial)*\s*(class|struct|enum|interface|delegate)\s+',
            line
        )

        # Check if this is an attribute that might precede a type
        is_attribute = re.match(r'^\s*\[.*\]\s*$', line)

        if not in_type and (is_type_decl or is_attribute):
            # This is a top-level type or attribute - remove ALL leading whitespace
            new_lines.append(stripped)
            if is_type_decl:
                in_type = True
                brace_depth = 0
            continue

        if in_type:
            # Track brace depth
            for char in stripped:
                if char == '{':
                    brace_depth += 1
                elif char == '}':
                    brace_depth -= 1

            # Keep original line inside type
            new_lines.append(line)

            # Check if we've closed the type
            if brace_depth < 0:
                in_type = False
                brace_depth = 0
        else:
            new_lines.append(line)

    new_content = '\n'.join(new_lines)

    if new_content != original_content:
        file_path.write_text(new_content, encoding='utf-8')
        return True

    return False

def main():
    root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")

    print(f"[*] Fixing indentation for types in global namespace...")

    # Find all C# files (excluding private implementation details)
    cs_files = [
        f for f in root.rglob("*.cs")
        if "_003CPrivateImplementationDetails_003E" not in str(f)
    ]

    print(f"[*] Found {len(cs_files)} C# files to check\n")

    total_fixed = 0
    fixed_files = []

    for cs_file in cs_files:
        try:
            if fix_global_namespace_indentation(cs_file):
                total_fixed += 1
                rel_path = cs_file.relative_to(Path(r"D:\Github\Re-ETG"))
                fixed_files.append(str(rel_path))
                if total_fixed <= 20:  # Print first 20 only
                    print(f"[OK] Fixed indentation: {rel_path}")
        except Exception as e:
            print(f"[!] Error processing {cs_file.name}: {e}")

    if total_fixed > 20:
        print(f"[*] ... and {total_fixed - 20} more files")

    print(f"\n[OK] Complete!")
    print(f"[OK] Files fixed: {total_fixed}")
    print(f"[OK] Types in global namespace now have correct indentation")

    return 0

if __name__ == "__main__":
    import sys
    sys.exit(main())
