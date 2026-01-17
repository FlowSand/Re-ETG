#!/usr/bin/env python3
"""分析低置信度分类"""
import json
from pathlib import Path

json_path = Path(r"D:\Github\Re-ETG\Docs\classification.json")
output_path = Path(r"D:\Github\Re-ETG\Docs\low_confidence_analysis.txt")

with open(json_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

low_all = [x for x in data if x['confidence'] == 'low']

# 按命名空间统计
by_ns = {}
for x in low_all:
    by_ns[x['namespace']] = by_ns.get(x['namespace'], 0) + 1

sorted_ns = sorted(by_ns.items(), key=lambda x: x[1], reverse=True)

# Utilities继承统计
utilities = [x for x in low_all if x['namespace'] == 'ETG.Core.Systems.Utilities']
inheritances = {}
for x in utilities:
    inheritances[x['inheritance']] = inheritances.get(x['inheritance'], 0) + 1

sorted_inh = sorted(inheritances.items(), key=lambda x: x[1], reverse=True)[:20]

# 写入结果
with open(output_path, 'w', encoding='utf-8') as f:
    f.write(f"=== Low Confidence Analysis ===\n")
    f.write(f"Total low confidence: {len(low_all)}\n\n")

    f.write("Distribution by Namespace:\n")
    for ns, count in sorted_ns:
        f.write(f"  {ns}: {count}\n")

    f.write(f"\nUtilities Low Confidence: {len(utilities)}\n")
    f.write("Top Inheritance Patterns:\n")
    for inh, count in sorted_inh:
        f.write(f"  {inh}: {count}\n")

print(f"Analysis saved to {output_path}")
