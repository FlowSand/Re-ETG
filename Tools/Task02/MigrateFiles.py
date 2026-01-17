#!/usr/bin/env python3
"""
Task-02: 文件迁移脚本
执行文件移动、添加命名空间、保持GUID一致性
"""

import os
import re
import shutil
import json
from pathlib import Path
from typing import List

class FileMigrator:
    """文件迁移器"""

    def __init__(self, classification_json: Path, etg_root: Path, dry_run: bool = True):
        self.classification_json = classification_json
        self.etg_root = etg_root
        self.dry_run = dry_run
        self.stats = {'moved': 0, 'modified': 0, 'errors': 0, 'skipped': 0}

        # 加载分类结果
        print(f"加载分类结果: {classification_json}")
        with open(classification_json, 'r', encoding='utf-8') as f:
            self.classifications = json.load(f)
        print(f"[OK] 加载了 {len(self.classifications)} 个文件的分类信息")

    def migrate_all(self):
        """执行所有文件的迁移"""
        print("\n开始迁移文件...")
        print(f"模式: {'DRY-RUN (仅模拟)' if self.dry_run else '正式执行'}")
        print()

        for i, classification in enumerate(self.classifications, 1):
            if i % 50 == 0:
                print(f"进度: {i}/{len(self.classifications)}")

            try:
                self.migrate_file(classification)
            except Exception as e:
                print(f"✗ 错误 [{classification['file']}]: {e}")
                self.stats['errors'] += 1

        self._print_summary()

    def migrate_file(self, classification: dict):
        """迁移单个文件"""
        source_path = Path(classification['file'])
        namespace = classification['namespace']
        target_rel_dir = classification['target_dir']

        if not source_path.exists():
            if self.dry_run:
                print(f"[DRY-RUN] 跳过（源文件不存在）: {source_path.name}")
            self.stats['skipped'] += 1
            return

        # 目标目录
        target_dir = self.etg_root / target_rel_dir
        target_cs = target_dir / source_path.name
        source_meta = source_path.with_suffix('.cs.meta')
        target_meta = target_dir / source_meta.name

        # 检查目标文件是否已存在
        if target_cs.exists() and not self.dry_run:
            print(f"[!] 跳过（目标已存在）: {source_path.name} → {target_rel_dir}")
            self.stats['skipped'] += 1
            return

        if self.dry_run:
            print(f"[DRY-RUN] {source_path.name:50} → {namespace}")
            return

        # === 正式执行 ===

        # 1. 读取源文件
        try:
            content = source_path.read_text(encoding='utf-8-sig')
        except Exception as e:
            print(f"[X] 无法读取 {source_path.name}: {e}")
            self.stats['errors'] += 1
            return

        # 2. 添加命名空间
        modified_content = self.add_namespace(content, namespace)

        # 3. 创建目标目录
        target_dir.mkdir(parents=True, exist_ok=True)

        # 4. 写入目标.cs文件
        try:
            target_cs.write_text(modified_content, encoding='utf-8-sig')
            self.stats['modified'] += 1
        except Exception as e:
            print(f"[X] 无法写入 {target_cs}: {e}")
            self.stats['errors'] += 1
            return

        # 5. 复制.meta文件（保持GUID）
        if source_meta.exists():
            try:
                shutil.copy2(source_meta, target_meta)
            except Exception as e:
                print(f"[X] 无法复制.meta {source_meta.name}: {e}")
                # 不算严重错误，继续
        else:
            print(f"[!] 警告: .meta文件不存在: {source_meta.name}")

        # 6. 删除源文件
        try:
            source_path.unlink()
            if source_meta.exists():
                source_meta.unlink()
            self.stats['moved'] += 1
            print(f"[OK] {source_path.name:50} → {target_rel_dir}")
        except Exception as e:
            print(f"[X] 无法删除源文件 {source_path.name}: {e}")
            self.stats['errors'] += 1

    def add_namespace(self, content: str, namespace: str) -> str:
        """添加命名空间包装"""

        # 检查是否已有namespace
        if re.search(r'^\s*namespace\s+', content, re.MULTILINE):
            return content  # 已有命名空间，跳过

        lines = content.split('\n')

        # 找到插入点
        insert_index = 0
        using_end_index = 0
        nullable_index = -1

        for i, line in enumerate(lines):
            stripped = line.strip()

            # 跳过注释
            if stripped.startswith('//') or stripped.startswith('/*') or stripped.startswith('*'):
                continue

            # 跟踪using语句结束位置
            if stripped.startswith('using '):
                using_end_index = i + 1
                continue

            # 跟踪#nullable指令
            if '#nullable' in stripped:
                nullable_index = i
                continue

            # 找到第一个类型声明
            if re.match(r'^\s*(?:public|internal|private|protected)?\s*(?:abstract|sealed|static|partial)?\s*(?:class|struct|enum|interface)\s+', line):
                insert_index = i
                break

        # 确定实际插入点
        if nullable_index >= 0:
            insert_index = nullable_index + 1
        elif using_end_index > 0:
            insert_index = using_end_index
        else:
            # 找不到using或nullable，在第一个非注释行
            for i, line in enumerate(lines):
                if line.strip() and not line.strip().startswith('//'):
                    insert_index = i
                    break

        # 构建namespace包装
        namespace_open = [
            '',
            f'namespace {namespace}',
            '{'
        ]

        namespace_close = [
            '}',
            ''
        ]

        # 插入namespace声明
        new_lines = lines[:insert_index] + namespace_open + lines[insert_index:] + namespace_close

        # 缩进namespace内的内容
        indented_lines = []
        indent_start = insert_index + len(namespace_open)
        indent_end = len(new_lines) - len(namespace_close)

        for i, line in enumerate(new_lines):
            if indent_start <= i < indent_end:
                # 只缩进非空行
                if line.strip():
                    indented_lines.append('    ' + line)
                else:
                    indented_lines.append(line)
            else:
                indented_lines.append(line)

        return '\n'.join(indented_lines)

    def _print_summary(self):
        """打印汇总信息"""
        print("\n" + "=" * 60)
        print("迁移完成")
        print("=" * 60)
        print(f"  已迁移: {self.stats['moved']} 个文件")
        print(f"  已修改: {self.stats['modified']} 个文件")
        print(f"  已跳过: {self.stats['skipped']} 个文件")
        print(f"  错  误: {self.stats['errors']} 个文件")
        print("=" * 60)

        if self.stats['errors'] > 0:
            print("\n[!] 有错误发生，请检查上面的输出")
        elif self.stats['moved'] > 0:
            print("\n[OK] 迁移成功！")
        else:
            print("\n[!] 没有文件被迁移")


def main():
    """主函数"""
    import sys
    import argparse

    parser = argparse.ArgumentParser(description='Task-02: 文件迁移脚本')
    parser.add_argument('--execute', action='store_true', help='执行实际迁移（默认为dry-run）')
    args = parser.parse_args()

    # 配置路径
    etg_root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")
    classification_json = Path(r"D:\Github\Re-ETG\Docs\classification.json")

    if not etg_root.exists():
        print(f"错误: 路径不存在: {etg_root}")
        sys.exit(1)

    if not classification_json.exists():
        print(f"错误: 分类文件不存在: {classification_json}")
        print("请先运行 ClassifyAndMoveFiles.py 生成分类结果")
        sys.exit(1)

    print("=" * 60)
    print("Task-02: 文件迁移")
    print("=" * 60)
    print()

    dry_run = not args.execute

    if dry_run:
        print("[!] DRY-RUN模式: 仅模拟，不实际修改文件")
        print("  使用 --execute 参数执行实际迁移")
        print()

    else:
        print("[!] 正式执行模式: 将实际移动文件！")
        print()
        response = input("确认继续？(yes/no): ").strip().lower()
        if response != 'yes':
            print("已取消")
            sys.exit(0)
        print()

    # 创建迁移器
    migrator = FileMigrator(classification_json, etg_root, dry_run=dry_run)

    # 执行迁移
    migrator.migrate_all()

    if not dry_run:
        print("\n下一步: 打开Unity验证编译结果")


if __name__ == '__main__':
    main()
