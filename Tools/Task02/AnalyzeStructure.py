#!/usr/bin/env python3
"""分析Core目录结构和文件分布"""
from pathlib import Path

core = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG\Core")

print("=" * 70)
print("Core Directory Structure Analysis")
print("=" * 70)
print()

total = 0
subdirs = sorted([d for d in core.iterdir() if d.is_dir()])

for d in subdirs:
    cs_files = list(d.rglob('*.cs'))
    count = len(cs_files)
    total += count

    print(f"{d.name:20} {count:5} files")

    # 列出子子目录
    subsubs = sorted([s for s in d.iterdir() if s.is_dir()])
    for s in subsubs:
        sub_count = len(list(s.rglob('*.cs')))
        print(f"  └─ {s.name:25} {sub_count:5} files")

    if subsubs:
        print()

print("=" * 70)
print(f"{'TOTAL':20} {total:5} files")
print("=" * 70)
