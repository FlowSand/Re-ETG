#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
修复with expression语法错误
将 `var x = y with { prop = value };` 转换为合法的C#代码
"""

import os
import re

# 需要修复的文件和行号（从scan_code_issues.py的输出）
FILES_TO_FIX = [
    ("Assets/Scripts/ETG/FullInspector/tk`2.cs", 554, "width"),
    ("Assets/Scripts/ETG/FullInspector/tk`2.cs", 1232, "height"),
    ("Assets/Scripts/ETG/Core/Systems/Utilities/tk2dButton.cs", 66, "z"),
    ("Assets/Scripts/ETG/Core/Systems/Utilities/TweenMaterialExtensions.cs", 25, "a"),
    ("Assets/Scripts/ETG/Core/Systems/Utilities/TweenTextExtensions.cs", 20, "a"),
    ("Assets/Scripts/ETG/Core/VFX/Animation/TweenSpriteExtensions.cs", 20, "a"),
    ("Assets/Scripts/ETG/HutongGames/PlayMaker/Actions/GetAxisVector.cs", 83, "z"),
    ("Assets/Scripts/ETG/HutongGames/PlayMaker/Actions/TransformInputToWorldSpace.cs", 85, "z"),
]

PROJECT_ROOT = r"D:\Github\Re-ETG"

def fix_with_expression(file_path: str):
    """修复文件中的with expression"""
    full_path = os.path.join(PROJECT_ROOT, file_path)

    if not os.path.exists(full_path):
        print(f"[!] 文件不存在: {full_path}")
        return False

    try:
        with open(full_path, 'r', encoding='utf-8') as f:
            content = f.read()

        original_content = content

        # 匹配with expression模式
        # 例如: Rect rect1 = rect with { width = layoutWidth };
        # 转换为: Rect rect1 = rect; rect1.width = layoutWidth;

        # Pattern 1: Type varName = source with { prop = value };
        # source可以是: var, obj.prop, obj.method()等
        pattern1 = re.compile(
            r'(\w+\s+\w+)\s*=\s*([^\s]+)\s+with\s*\{\s*(\w+)\s*=\s*([^}]+)\s*\}\s*;',
            re.MULTILINE
        )

        def replace1(match):
            var_decl = match.group(1)  # "Vector3 size"
            source = match.group(2)     # "boxCollider.size"
            prop = match.group(3)       # "z"
            value = match.group(4).strip()  # "0.2f"

            # 生成新代码
            var_name = var_decl.split()[-1]  # 提取变量名 "size"
            return f"{var_decl} = {source};\n          {var_name}.{prop} = {value};"

        content = pattern1.sub(replace1, content)

        # Pattern 2: existingVar = source with { prop = value };
        pattern2 = re.compile(
            r'(\w+)\s*=\s*([^\s]+)\s+with\s*\{\s*(\w+)\s*=\s*([^}]+)\s*\}\s*;',
            re.MULTILINE
        )

        def replace2(match):
            var_name = match.group(1)  # "vector3_1"
            source = match.group(2)     # "Vector3.up"
            prop = match.group(3)       # "z"
            value = match.group(4).strip()  # "0.0f"

            return f"{var_name} = {source};\n      {var_name}.{prop} = {value};"

        content = pattern2.sub(replace2, content)

        if content != original_content:
            with open(full_path, 'w', encoding='utf-8') as f:
                f.write(content)
            print(f"[>] 已修复: {file_path}")
            return True
        else:
            print(f"[i] 无需修改: {file_path}")
            return False

    except Exception as e:
        print(f"[X] 修复失败 {file_path}: {e}")
        return False

def main():
    print("=" * 80)
    print("[*] With Expression Fixer")
    print("=" * 80)

    fixed_count = 0
    file_set = set()  # 去重，因为有些文件有多个问题

    # 收集唯一文件
    for file_path, line, prop in FILES_TO_FIX:
        file_set.add(file_path)

    print(f"[i] 需要修复 {len(file_set)} 个文件")
    print()

    for file_path in sorted(file_set):
        if fix_with_expression(file_path):
            fixed_count += 1

    print()
    print("=" * 80)
    print(f"[OK] 完成！修复了 {fixed_count} 个文件")
    print("=" * 80)

if __name__ == "__main__":
    main()
