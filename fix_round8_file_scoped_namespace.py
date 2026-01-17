#!/usr/bin/env python3
"""
Round 8: Convert C# 10.0 file-scoped namespaces to traditional block namespaces.

Unity 2022.3.62 defaults to C# 9.0, which doesn't support file-scoped namespaces.

Pattern:
  namespace Foo;
  <blank line>
  public class Bar { }

Converts to:
  namespace Foo
  {
    public class Bar { }
  }
"""

import os
import re
from pathlib import Path

def convert_file_scoped_namespace(file_path):
    """Convert file-scoped namespace to block namespace."""
    try:
        with open(file_path, 'r', encoding='utf-8-sig') as f:
            content = f.read()

        original_content = content

        # Check if this file has file-scoped namespace
        # Pattern: namespace Name; at the start (after comments and #nullable)
        match = re.search(r'^((?:.*\n)*?)(namespace\s+[A-Za-z0-9_.]+);(\s*\n)', content, re.MULTILINE)

        if not match:
            return False

        before_namespace = match.group(1)  # Everything before namespace
        namespace_decl = match.group(2)     # namespace Foo
        after_semicolon = match.group(3)    # Whitespace after semicolon

        # Get the content after the namespace declaration
        namespace_start = match.end()
        namespace_content = content[namespace_start:]

        # Remove trailing whitespace from file
        namespace_content = namespace_content.rstrip()

        # Build the new content with block namespace
        new_content = before_namespace
        new_content += namespace_decl + "\n"
        new_content += "{\n"

        # Process each line of namespace content to add indentation
        lines = namespace_content.split('\n')
        for line in lines:
            if line.strip():  # Non-empty line
                new_content += "  " + line + "\n"
            else:  # Empty line
                new_content += "\n"

        # Add closing brace
        new_content += "}\n"

        # Write back
        with open(file_path, 'w', encoding='utf-8-sig') as f:
            f.write(new_content)

        return True

    except Exception as e:
        print(f"Error processing {file_path}: {e}")
        import traceback
        traceback.print_exc()
        return False

def main():
    """Process all C# files with file-scoped namespaces."""
    root_dir = Path('Assets/Scripts/ETG')

    if not root_dir.exists():
        print(f"Error: Directory {root_dir} does not exist")
        return

    # Find files with file-scoped namespaces
    cs_files = []
    for cs_file in root_dir.rglob('*.cs'):
        try:
            with open(cs_file, 'r', encoding='utf-8-sig') as f:
                content = f.read()
                if re.search(r'^namespace\s+[A-Za-z0-9_.]+;\s*$', content, re.MULTILINE):
                    cs_files.append(cs_file)
        except:
            pass

    total_files = len(cs_files)
    modified_files = 0

    print(f"Found {total_files} C# files with file-scoped namespaces...")

    for i, cs_file in enumerate(cs_files, 1):
        if i % 100 == 0:
            print(f"Progress: {i}/{total_files} files processed, {modified_files} modified")

        if convert_file_scoped_namespace(cs_file):
            modified_files += 1

    print(f"\nComplete!")
    print(f"Total files: {total_files}")
    print(f"Modified files: {modified_files}")
    print(f"Unchanged files: {total_files - modified_files}")

if __name__ == '__main__':
    main()
