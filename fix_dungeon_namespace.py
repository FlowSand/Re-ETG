#!/usr/bin/env python3
"""
Fix namespace collision: Dungeon type vs Dungeon namespace
Replaces unqualified 'Dungeon' type references with 'Dungeonator.Dungeon'
"""
import re
import sys
from pathlib import Path

# Files with CS0118 namespace collision errors
FILES_TO_FIX = [
    r"Assets\Scripts\ETG\Core\Systems\Utilities\TK2DDungeonAssembler.cs",
    r"Assets\Scripts\ETG\Core\Systems\Utilities\TK2DInteriorDecorator.cs",
    r"Assets\Scripts\ETG\Core\Systems\Management\GameManager.cs",
    r"Assets\Scripts\ETG\Core\Systems\Management\LevelNameUIManager.cs",
    r"Assets\Scripts\ETG\Core\Dungeon\Rooms\DungeonFloorMusicController.cs",
    r"Assets\Scripts\ETG\Core\Systems\Data\DungeonDatabase.cs",
    r"Assets\Scripts\ETG\Core\Items\Active\PaydayDrillItem.cs",
    r"Assets\Scripts\ETG\Core\VFX\Animation\PitParticleKiller.cs",
    r"Assets\Scripts\ETG\Core\Systems\Utilities\DebrisObject.cs",
    r"Assets\Scripts\ETG\Core\Core\Framework\Projectile.cs",
]

def fix_dungeon_namespace(file_path: Path) -> int:
    """
    Fix Dungeon namespace collision in a single file.
    Returns the number of replacements made.
    """
    content = file_path.read_text(encoding='utf-8')
    original_content = content

    # Pattern: Dungeon followed by space and identifier (parameter/variable name)
    # This captures type declarations like: Dungeon d, Dungeon dungeon, etc.
    # But NOT: namespace Dungeon, // Dungeon, "Dungeon", etc.

    # Replace in method parameters and variable declarations
    # Pattern: \bDungeon\s+(\w+) where Dungeon is a whole word followed by identifier
    # Make sure it's not part of a namespace declaration or comment

    replacements = 0

    # Replace Dungeon type declarations (parameter, variable, return types)
    # Look for patterns like:
    # - "Dungeon variableName"
    # - "(Dungeon parameter)"
    # - "<Dungeon>"
    # But NOT "namespace Dungeon" or inside comments/strings

    lines = content.split('\n')
    new_lines = []

    for line in lines:
        original_line = line

        # Skip lines that are comments or namespace declarations
        stripped = line.strip()
        if (stripped.startswith('//') or
            stripped.startswith('/*') or
            stripped.startswith('*') or
            'namespace Dungeon' in line or
            'using Dungeon' in line):
            new_lines.append(line)
            continue

        # Replace Dungeon type references
        # Pattern 1: "Dungeon " followed by identifier (method params, variables)
        modified_line = re.sub(
            r'\bDungeon\s+(\w+)',
            r'Dungeonator.Dungeon \1',
            line
        )

        # Pattern 2: "<Dungeon>" or "<Dungeon," (generic type parameters)
        modified_line = re.sub(
            r'<Dungeon([,>])',
            r'<Dungeonator.Dungeon\1',
            modified_line
        )

        # Pattern 3: "(Dungeon " at start of parameter list
        modified_line = re.sub(
            r'\(Dungeon\s+',
            r'(Dungeonator.Dungeon ',
            modified_line
        )

        # Pattern 4: ", Dungeon " in parameter lists
        modified_line = re.sub(
            r',\s*Dungeon\s+',
            r', Dungeonator.Dungeon ',
            modified_line
        )

        if modified_line != original_line:
            replacements += 1

        new_lines.append(modified_line)

    new_content = '\n'.join(new_lines)

    # Write back if changes were made
    if new_content != original_content:
        file_path.write_text(new_content, encoding='utf-8')
        return replacements

    return 0

def main():
    root = Path(r"D:\Github\Re-ETG")
    total_files_fixed = 0
    total_replacements = 0

    print(f"[*] Fixing Dungeon namespace collisions...")
    print(f"[*] Processing {len(FILES_TO_FIX)} files\n")

    for file_rel in FILES_TO_FIX:
        file_path = root / file_rel

        if not file_path.exists():
            print(f"[!] File not found: {file_rel}")
            continue

        print(f"[*] Processing: {file_rel}")
        replacements = fix_dungeon_namespace(file_path)

        if replacements > 0:
            total_files_fixed += 1
            total_replacements += replacements
            print(f"    [OK] {replacements} lines modified")
        else:
            print(f"    [OK] No changes needed")

    print(f"\n[OK] Complete!")
    print(f"[OK] Files modified: {total_files_fixed}/{len(FILES_TO_FIX)}")
    print(f"[OK] Total lines with replacements: {total_replacements}")
    print(f"[OK] Expected error reduction: ~222 CS0118 errors")

    return 0

if __name__ == "__main__":
    sys.exit(main())
