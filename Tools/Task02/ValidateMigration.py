#!/usr/bin/env python3
"""
Task-02: 迁移验证脚本
验证文件迁移是否成功完成
"""

import sys
from pathlib import Path

def validate_migration(etg_root: Path) -> bool:
    """验证迁移结果"""

    print("=" * 60)
    print("Task-02: 迁移验证")
    print("=" * 60)
    print()

    errors = []
    warnings = []

    # 检查1: 根目录不应有.cs文件
    print("检查1: 根目录文件清理...")
    root_cs_files = list(etg_root.glob('*.cs'))
    if root_cs_files:
        errors.append(f"根目录仍有 {len(root_cs_files)} 个.cs文件（预期0个）")
        print(f"  [X] 根目录仍有 {len(root_cs_files)} 个.cs文件")
        if len(root_cs_files) <= 10:
            for f in root_cs_files:
                print(f"    - {f.name}")
    else:
        print(f"  [OK] 根目录已清空（0个.cs文件）")

    # 检查2: Core/目录应该存在且有文件
    print("\n检查2: Core/目录结构...")
    core_dir = etg_root / 'Core'
    if not core_dir.exists():
        errors.append("Core/目录不存在")
        print(f"  [X] Core/目录不存在")
    else:
        core_cs_files = list(core_dir.rglob('*.cs'))
        print(f"  [OK] Core/目录存在，包含 {len(core_cs_files)} 个.cs文件")

        # 检查预期的子目录
        expected_dirs = [
            'Actors', 'Items', 'Combat', 'Dungeon', 'UI',
            'VFX', 'Audio', 'Systems', 'Core'
        ]
        missing_dirs = []
        for dir_name in expected_dirs:
            if not (core_dir / dir_name).exists():
                missing_dirs.append(dir_name)

        if missing_dirs:
            warnings.append(f"缺少预期子目录: {', '.join(missing_dirs)}")
            print(f"  [\!] 缺少子目录: {', '.join(missing_dirs)}")
        else:
            print(f"  [OK] 所有预期子目录都存在")

    # 检查3: 命名空间检查（抽样）
    print("\n检查3: 命名空间声明...")
    if core_dir.exists():
        sample_files = list(core_dir.rglob('*.cs'))[:20]
        missing_namespace = []

        for cs_file in sample_files:
            try:
                content = cs_file.read_text(encoding='utf-8-sig')
                if 'namespace ETG.Core' not in content:
                    missing_namespace.append(cs_file.name)
            except:
                pass

        if missing_namespace:
            warnings.append(f"{len(missing_namespace)} 个抽样文件缺少ETG.Core命名空间")
            print(f"  [\!] {len(missing_namespace)}/20 抽样文件缺少命名空间")
            for fname in missing_namespace[:5]:
                print(f"    - {fname}")
        else:
            print(f"  [OK] 抽样文件都包含ETG.Core命名空间")

    # 检查4: .meta文件完整性
    print("\n检查4: .meta文件完整性...")
    if core_dir.exists():
        all_cs = list(core_dir.rglob('*.cs'))
        missing_meta = []

        for cs_file in all_cs:
            meta_file = cs_file.with_suffix('.cs.meta')
            if not meta_file.exists():
                missing_meta.append(cs_file.name)

        if missing_meta:
            errors.append(f"{len(missing_meta)} 个.cs文件缺少.meta文件")
            print(f"  [X] {len(missing_meta)} 个.cs文件缺少.meta")
            for fname in missing_meta[:5]:
                print(f"    - {fname}")
        else:
            print(f"  [OK] 所有.cs文件都有对应的.meta文件")

    # 检查5: 第三方库未改变
    print("\n检查5: 第三方库目录...")
    third_party_dirs = [
        'Dungeonator', 'AK', 'Brave', 'FullInspector', 'InControl',
        'Pathfinding', 'HutongGames', 'DaikonForge', 'tk2dRuntime'
    ]

    for dir_name in third_party_dirs:
        dir_path = etg_root / dir_name
        if not dir_path.exists():
            warnings.append(f"第三方目录不存在: {dir_name}（可能正常）")
            print(f"  [\!] {dir_name}/ 不存在")
        else:
            print(f"  [OK] {dir_name}/ 存在")

    # 检查6: 文件计数
    print("\n检查6: 文件数量统计...")
    if core_dir.exists():
        actual_count = len(list(core_dir.rglob('*.cs')))
        expected_min = 2200  # 允许一些误差
        expected_max = 2400

        if actual_count < expected_min or actual_count > expected_max:
            warnings.append(f"文件数量 {actual_count} 超出预期范围 [{expected_min}-{expected_max}]")
            print(f"  [\!] 文件数: {actual_count} (预期 ~2323)")
        else:
            print(f"  [OK] 文件数: {actual_count} (预期 ~2323)")

    # 汇总结果
    print("\n" + "=" * 60)
    print("验证结果")
    print("=" * 60)

    if warnings:
        print("\n警告:")
        for w in warnings:
            print(f"  [\!] {w}")

    if errors:
        print("\n错误:")
        for e in errors:
            print(f"  [X] {e}")
        print("\n[FAIL] 验证失败")
        return False
    else:
        print("\n[SUCCESS] 所有验证检查通过！")
        return True


def main():
    """主函数"""
    etg_root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")

    if not etg_root.exists():
        print(f"错误: 路径不存在: {etg_root}")
        sys.exit(1)

    success = validate_migration(etg_root)
    sys.exit(0 if success else 1)


if __name__ == '__main__':
    main()
