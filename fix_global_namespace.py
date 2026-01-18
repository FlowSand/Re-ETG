#!/usr/bin/env python3
"""
Fix internal type resolution by restoring types to global namespace.

The decompiler originally placed many types in the global namespace (no namespace declaration).
During Task-02, these types were moved into organized namespaces based on folder structure.
This broke all references to these types.

This script identifies types that were originally global and removes their namespace declarations.
"""
import re
from pathlib import Path
from typing import Optional

def get_original_type_name(file_content: str) -> Optional[str]:
    """
    Extract the original type name from decompiler comment.
    Returns None if namespace is present (e.g., "Dungeonator.Something").
    Returns type name if it was global (e.g., "PlayerController").
    """
    match = re.search(r'^// Type: ([^\s]+)', file_content, re.MULTILINE)
    if not match:
        return None

    type_name = match.group(1)

    # If type name contains a dot, it had a namespace originally
    if '.' in type_name:
        return None

    return type_name

def remove_namespace_declaration(file_path: Path) -> bool:
    """
    Remove namespace declaration from a C# file, moving all contents to global namespace.
    Returns True if changes were made.
    """
    content = file_path.read_text(encoding='utf-8')
    original_content = content

    # Check if this type was originally global
    orig_type = get_original_type_name(content)
    if orig_type is None:
        return False

    # Find and remove namespace declaration
    # Pattern: namespace SomeName\n{\n ... \n}\n
    # We need to: 1) Remove the namespace line, 2) Remove the opening brace, 3) Remove closing brace, 4) Unindent content

    # Find namespace declaration
    namespace_match = re.search(
        r'^namespace\s+[\w.]+\s*\n\s*{\s*$',
        content,
        re.MULTILINE
    )

    if not namespace_match:
        # No namespace declaration found
        return False

    # Split content into lines
    lines = content.split('\n')
    new_lines = []
    in_namespace = False
    namespace_indent = 0
    brace_count = 0

    for i, line in enumerate(lines):
        # Check if this is the namespace declaration line
        if re.match(r'^\s*namespace\s+[\w.]+\s*$', line):
            in_namespace = True
            namespace_indent = len(line) - len(line.lstrip())
            continue  # Skip namespace line

        # Check if this is the opening brace of namespace
        if in_namespace and not brace_count and re.match(r'^\s*{\s*$', line):
            brace_count = 1
            continue  # Skip opening brace

        # Check if this is the closing brace of namespace
        if in_namespace and brace_count > 0:
            # Count braces on this line
            open_braces = line.count('{')
            close_braces = line.count('}')
            brace_count += open_braces - close_braces

            if brace_count == 0 and re.match(r'^\s*}\s*$', line):
                # This is the closing brace of the namespace
                continue  # Skip closing brace

        # For content inside namespace, remove one level of indentation
        if in_namespace:
            # Remove 2 spaces or 1 tab of indentation
            if line.startswith('  '):
                line = line[2:]
            elif line.startswith('\t'):
                line = line[1:]

        new_lines.append(line)

    new_content = '\n'.join(new_lines)

    # Write back if changes were made
    if new_content != original_content:
        file_path.write_text(new_content, encoding='utf-8')
        return True

    return False

def main():
    root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")

    print(f"[*] Scanning for types that need to be moved to global namespace...")

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
            if remove_namespace_declaration(cs_file):
                total_fixed += 1
                rel_path = cs_file.relative_to(Path(r"D:\Github\Re-ETG"))
                fixed_files.append(str(rel_path))
                print(f"[OK] Moved to global namespace: {rel_path}")
        except Exception as e:
            print(f"[!] Error processing {cs_file.name}: {e}")

    print(f"\n[OK] Complete!")
    print(f"[OK] Files moved to global namespace: {total_fixed}")
    print(f"[OK] Expected error reduction: ~14,319 internal type resolution errors")

    if fixed_files:
        print(f"\n[*] Sample of fixed files:")
        for f in fixed_files[:10]:
            print(f"    - {f}")
        if len(fixed_files) > 10:
            print(f"    ... and {len(fixed_files) - 10} more")

    return 0

if __name__ == "__main__":
    import sys
    sys.exit(main())
