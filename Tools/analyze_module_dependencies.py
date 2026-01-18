#!/usr/bin/env python3
"""
ETG Module Dependency Analyzer

Analyzes module dependencies in the Enter the Gungeon decompiled codebase by:
1. Scanning all C# files for using statements
2. Building a dependency graph between modules
3. Generating dependency manifests in multiple formats (JSON, Markdown, Mermaid)

Usage:
    python Tools/analyze_module_dependencies.py --output Docs/
"""

import re
import json
import yaml
import argparse
from pathlib import Path
from collections import defaultdict, Counter
from typing import Dict, List, Set, Tuple

class ModuleDependencyAnalyzer:
    def __init__(self, root_path: Path, module_defs_path: Path):
        self.root_path = root_path
        self.module_defs_path = module_defs_path
        self.modules = {}
        self.dependency_graph = defaultdict(lambda: defaultdict(int))
        self.file_count = {}
        self.using_statements = defaultdict(set)

        self.load_module_definitions()

    def load_module_definitions(self):
        """Load module definitions from YAML file"""
        print(f"[*] Loading module definitions from {self.module_defs_path}")

        with open(self.module_defs_path, 'r', encoding='utf-8') as f:
            data = yaml.safe_load(f)

        for module in data['modules']:
            name = module['name']
            self.modules[name] = module
            self.file_count[name] = 0

        print(f"[OK] Loaded {len(self.modules)} module definitions\n")

    def parse_using_statements(self, file_path: Path) -> List[str]:
        """Extract using statements from a C# file"""
        usings = []

        try:
            with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                for line in f:
                    line = line.strip()

                    # Match using statements
                    # Pattern: using <namespace>;
                    match = re.match(r'^using\s+([\w\.]+)\s*;', line)
                    if match:
                        namespace = match.group(1)
                        usings.append(namespace)

                    # Stop at namespace or class declaration
                    if line.startswith('namespace ') or line.startswith('public class ') or line.startswith('internal class '):
                        break

        except Exception as e:
            print(f"[!] Error reading {file_path}: {e}")

        return usings

    def get_module_for_file(self, file_path: Path) -> str:
        """Determine which module a file belongs to"""
        file_str = str(file_path).replace('\\', '/')

        # Match against module paths (longest match wins)
        matched_module = None
        max_match_len = 0

        for name, module in self.modules.items():
            module_path = module['path'].replace('\\', '/')
            if module_path in file_str:
                match_len = len(module_path)
                if match_len > max_match_len:
                    max_match_len = match_len
                    matched_module = name

        return matched_module

    def get_module_for_namespace(self, namespace: str) -> str:
        """Map a namespace to a module"""
        # Check for exact namespace match
        for name, module in self.modules.items():
            mod_namespace = module.get('namespace', '')
            if mod_namespace and namespace.startswith(mod_namespace):
                return name

        # Check for common Unity/System namespaces
        if namespace.startswith('UnityEngine'):
            return 'UnityEngine'
        elif namespace.startswith('System'):
            return 'System'
        elif namespace.startswith('UnityEditor'):
            return 'UnityEditor'

        # Try to match by module name
        for name in self.modules.keys():
            if namespace.startswith(name.replace('.', '')):
                return name

        return None

    def scan_files(self):
        """Scan all C# files and build dependency graph"""
        print("[*] Scanning C# files...")

        cs_files = list(self.root_path.rglob('*.cs'))
        print(f"[*] Found {len(cs_files)} C# files to analyze\n")

        progress_interval = len(cs_files) // 20  # Report progress every 5%

        for idx, cs_file in enumerate(cs_files):
            # Progress reporting
            if progress_interval > 0 and idx % progress_interval == 0:
                progress = (idx / len(cs_files)) * 100
                print(f"[*] Progress: {idx}/{len(cs_files)} files ({progress:.1f}%)")

            # Determine which module this file belongs to
            source_module = self.get_module_for_file(cs_file)
            if not source_module:
                continue

            # Increment file count
            self.file_count[source_module] += 1

            # Parse using statements
            usings = self.parse_using_statements(cs_file)

            # Record dependencies
            for using in usings:
                target_module = self.get_module_for_namespace(using)
                if target_module and target_module != source_module:
                    self.dependency_graph[source_module][target_module] += 1
                    self.using_statements[source_module].add(using)

        print(f"\n[OK] Scanned {len(cs_files)} files")
        print(f"[OK] Identified dependencies for {len(self.dependency_graph)} modules\n")

    def detect_circular_dependencies(self) -> List[Tuple[str, str]]:
        """Detect circular dependencies between modules"""
        circular = []

        for source in self.dependency_graph:
            for target in self.dependency_graph[source]:
                if source in self.dependency_graph.get(target, {}):
                    if (target, source) not in circular:
                        circular.append((source, target))

        return circular

    def generate_json(self, output_path: Path):
        """Generate dependency_analysis.json"""
        print("[*] Generating JSON manifest...")

        modules_data = []

        for name, module in self.modules.items():
            module_data = {
                'name': name,
                'path': module['path'],
                'namespace': module.get('namespace', ''),
                'category': module['category'],
                'file_count': self.file_count.get(name, 0),
                'priority': module.get('priority', 'low'),
                'description': module.get('description', ''),
                'dependencies': [],
                'dependents': []
            }

            # Add dependencies (modules this module uses)
            if name in self.dependency_graph:
                for target, count in self.dependency_graph[name].items():
                    module_data['dependencies'].append({
                        'module': target,
                        'usage_count': count
                    })

            # Add dependents (modules that use this module)
            for source, targets in self.dependency_graph.items():
                if name in targets:
                    module_data['dependents'].append({
                        'module': source,
                        'usage_count': targets[name]
                    })

            modules_data.append(module_data)

        # Build dependency graph edges
        edges = []
        for source, targets in self.dependency_graph.items():
            for target, weight in targets.items():
                edges.append({
                    'from': source,
                    'to': target,
                    'weight': weight
                })

        # Detect circular dependencies
        circular_deps = self.detect_circular_dependencies()

        output_data = {
            'metadata': {
                'generator': 'analyze_module_dependencies.py',
                'total_modules': len(self.modules),
                'total_files_scanned': sum(self.file_count.values()),
                'circular_dependencies_found': len(circular_deps)
            },
            'modules': modules_data,
            'dependency_graph': {
                'edges': edges
            },
            'circular_dependencies': [{'from': a, 'to': b} for a, b in circular_deps]
        }

        json_path = output_path / 'dependency_analysis.json'
        with open(json_path, 'w', encoding='utf-8') as f:
            json.dump(output_data, f, indent=2)

        print(f"[OK] Generated {json_path}\n")

    def generate_markdown_matrix(self, output_path: Path):
        """Generate Module_Dependency_Matrix.md"""
        print("[*] Generating dependency matrix...")

        # Collect all dependencies
        dependencies = []
        for source, targets in self.dependency_graph.items():
            for target, count in targets.items():
                dependencies.append({
                    'from': source,
                    'to': target,
                    'count': count,
                    'from_category': self.modules[source]['category'],
                    'to_priority': self.modules.get(target, {}).get('priority', 'unknown')
                })

        # Sort by count (descending)
        dependencies.sort(key=lambda x: x['count'], reverse=True)

        # Generate markdown
        md = []
        md.append("# Module Dependency Matrix\n")
        md.append("**Generated by:** analyze_module_dependencies.py\n")
        md.append(f"**Total Dependencies:** {len(dependencies)}\n")
        md.append(f"**Total Modules:** {len(self.modules)}\n\n")

        md.append("## Summary Statistics\n")
        md.append(f"- Files Scanned: {sum(self.file_count.values())}\n")
        md.append(f"- Module-to-Module Dependencies: {len(dependencies)}\n")
        md.append(f"- Circular Dependencies: {len(self.detect_circular_dependencies())}\n\n")

        md.append("## Dependency Table\n\n")
        md.append("| From Module | To Module | Usage Count | From Category | To Priority |\n")
        md.append("|------------|-----------|-------------|---------------|-------------|\n")

        for dep in dependencies:
            md.append(f"| {dep['from']} | {dep['to']} | {dep['count']} | {dep['from_category']} | {dep['to_priority']} |\n")

        md.append("\n## Notes\n\n")
        md.append("- **Usage Count**: Number of files in 'From Module' that reference 'To Module'\n")
        md.append("- **From Category**: Category of the source module (external, core_subsystem)\n")
        md.append("- **To Priority**: Priority level of the target module (critical, high, medium, low)\n\n")

        matrix_path = output_path / 'Module_Dependency_Matrix.md'
        with open(matrix_path, 'w', encoding='utf-8') as f:
            f.write(''.join(md))

        print(f"[OK] Generated {matrix_path}\n")

    def generate_mermaid_graph(self, output_path: Path):
        """Generate Module_Dependency_Graph.md with Mermaid diagram"""
        print("[*] Generating dependency graph...")

        md = []
        md.append("# Module Dependency Graph\n\n")
        md.append("**Generated by:** analyze_module_dependencies.py\n\n")

        md.append("## Architecture Layers\n\n")
        md.append("```mermaid\n")
        md.append("graph TD\n\n")

        # Layer 0: Foundation (External dependencies)
        md.append("    subgraph Foundation[\"Layer 0: Foundation (External)\"]\n")
        md.append("        Unity[UnityEngine]\n")
        md.append("        System[System]\n")
        for name, module in self.modules.items():
            if module['category'] == 'external' and module.get('priority') in ['critical', 'high']:
                safe_name = name.replace('.', '_').replace('-', '_')
                md.append(f"        {safe_name}[{name}]\n")
        md.append("    end\n\n")

        # Layer 1: Core Systems
        md.append("    subgraph Core[\"Layer 1: Core Systems\"]\n")
        for name in ['Core.Systems', 'Core.Core']:
            if name in self.modules:
                safe_name = name.replace('.', '_')
                md.append(f"        {safe_name}[{name}]\n")
        md.append("    end\n\n")

        # Layer 2: Domain Logic
        md.append("    subgraph Domain[\"Layer 2: Domain Logic\"]\n")
        for name in ['Core.Actors', 'Core.Combat', 'Core.Items', 'Core.Dungeon']:
            if name in self.modules:
                safe_name = name.replace('.', '_')
                md.append(f"        {safe_name}[{name}]\n")
        md.append("    end\n\n")

        # Layer 3: Presentation
        md.append("    subgraph Presentation[\"Layer 3: Presentation\"]\n")
        for name in ['Core.UI', 'Core.VFX', 'Core.Audio']:
            if name in self.modules:
                safe_name = name.replace('.', '_')
                md.append(f"        {safe_name}[{name}]\n")
        md.append("    end\n\n")

        # Add key dependencies (top 20)
        dependencies = []
        for source, targets in self.dependency_graph.items():
            for target, count in targets.items():
                dependencies.append((source, target, count))

        dependencies.sort(key=lambda x: x[2], reverse=True)

        md.append("    %% Key Dependencies (top 20 by usage)\n")
        for source, target, count in dependencies[:20]:
            safe_source = source.replace('.', '_').replace('-', '_')
            safe_target = target.replace('.', '_').replace('-', '_')
            if count > 50:
                md.append(f"    {safe_source} ==>|{count}| {safe_target}\n")
            else:
                md.append(f"    {safe_source} -->|{count}| {safe_target}\n")

        md.append("```\n\n")

        md.append("## Legend\n\n")
        md.append("- **Solid arrows (-->)**: Standard dependencies\n")
        md.append("- **Thick arrows (==>)**: Heavy dependencies (>50 files)\n")
        md.append("- **Numbers on arrows**: Count of files with dependency\n\n")

        graph_path = output_path / 'Module_Dependency_Graph.md'
        with open(graph_path, 'w', encoding='utf-8') as f:
            f.write(''.join(md))

        print(f"[OK] Generated {graph_path}\n")

    def run_analysis(self, output_path: Path):
        """Run complete dependency analysis"""
        print("=" * 60)
        print("ETG Module Dependency Analyzer")
        print("=" * 60)
        print()

        # Scan files and build dependency graph
        self.scan_files()

        # Create output directory
        output_path.mkdir(parents=True, exist_ok=True)

        # Generate outputs
        self.generate_json(output_path)
        self.generate_markdown_matrix(output_path)
        self.generate_mermaid_graph(output_path)

        # Summary
        print("=" * 60)
        print("Analysis Complete!")
        print("=" * 60)
        print(f"[OK] Generated 3 output files in {output_path}")
        print(f"[OK] Modules analyzed: {len(self.modules)}")
        print(f"[OK] Files scanned: {sum(self.file_count.values())}")
        print(f"[OK] Dependencies identified: {sum(len(targets) for targets in self.dependency_graph.values())}")

        circular = self.detect_circular_dependencies()
        if circular:
            print(f"\n[!] WARNING: {len(circular)} circular dependencies detected!")
            for a, b in circular:
                print(f"    - {a} <--> {b}")
        else:
            print("\n[OK] No circular dependencies detected")


def main():
    parser = argparse.ArgumentParser(
        description='Analyze module dependencies in ETG codebase'
    )
    parser.add_argument(
        '--root',
        type=Path,
        default=Path('Assets/Scripts/ETG'),
        help='Root directory to scan for C# files (default: Assets/Scripts/ETG)'
    )
    parser.add_argument(
        '--module-defs',
        type=Path,
        default=Path('Docs/module_definitions.yaml'),
        help='Module definitions YAML file (default: Docs/module_definitions.yaml)'
    )
    parser.add_argument(
        '--output',
        type=Path,
        default=Path('Docs'),
        help='Output directory for generated files (default: Docs/)'
    )

    args = parser.parse_args()

    # Validate inputs
    if not args.root.exists():
        print(f"[!] Error: Root path does not exist: {args.root}")
        return 1

    if not args.module_defs.exists():
        print(f"[!] Error: Module definitions file does not exist: {args.module_defs}")
        return 1

    # Run analysis
    analyzer = ModuleDependencyAnalyzer(args.root, args.module_defs)
    analyzer.run_analysis(args.output)

    return 0


if __name__ == '__main__':
    import sys
    sys.exit(main())
