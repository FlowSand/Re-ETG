#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
代码问题扫描器 - 不需要Unity运行
直接扫描C#代码找出可能的编译问题
"""

import os
import re
from pathlib import Path
from typing import List, Dict, Tuple

PROJECT_PATH = r"D:\Github\Re-ETG\Assets\Scripts\ETG"

class CodeScanner:
    """代码扫描器"""

    def __init__(self, project_path: str):
        self.project_path = project_path
        self.issues = []

    def scan_all(self):
        """扫描所有可能的问题"""
        print("[*] 开始扫描代码...")

        cs_files = list(Path(self.project_path).rglob("*.cs"))
        print(f"[i] 找到 {len(cs_files)} 个C#文件")

        for file_path in cs_files:
            self.scan_file(file_path)

        return self.issues

    def scan_file(self, file_path: Path):
        """扫描单个文件"""
        try:
            with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                content = f.read()
                lines = content.split('\n')

            # 检查各种问题
            self._check_primary_constructors(file_path, content, lines)
            self._check_init_setters(file_path, content, lines)
            self._check_record_types(file_path, content, lines)
            self._check_file_scoped_namespace(file_path, content, lines)
            self._check_with_expressions(file_path, content, lines)
            self._check_top_level_statements(file_path, content, lines)

        except Exception as e:
            print(f"[!] 读取文件失败 {file_path}: {e}")

    def _check_primary_constructors(self, file_path: Path, content: str, lines: List[str]):
        """检查primary constructor（C# 12）"""
        # 匹配类/结构体声明时带参数：struct Name(params) { 或 class Name(params) {
        pattern = re.compile(
            r'^\s*(public|private|protected|internal)?\s*(readonly\s+)?(struct|class)\s+(\w+)\s*\([^)]+\)\s*$',
            re.MULTILINE
        )

        for match in pattern.finditer(content):
            line_num = content[:match.start()].count('\n') + 1
            struct_or_class = match.group(3)
            name = match.group(4)

            # 检查下一行是否是 {
            next_line_start = match.end()
            remaining = content[next_line_start:].lstrip()
            if remaining.startswith('{'):
                self.issues.append({
                    'file': str(file_path),
                    'line': line_num,
                    'type': 'PRIMARY_CONSTRUCTOR',
                    'message': f'C# 12.0 primary constructor: {struct_or_class} {name}(...)'
                })

    def _check_init_setters(self, file_path: Path, content: str, lines: List[str]):
        """检查init-only setters（C# 9）"""
        pattern = re.compile(r'\{\s*get;\s*init;\s*\}')

        for i, line in enumerate(lines, 1):
            if pattern.search(line):
                self.issues.append({
                    'file': str(file_path),
                    'line': i,
                    'type': 'INIT_SETTER',
                    'message': f'C# 9.0 init-only setter: {line.strip()[:60]}'
                })

    def _check_record_types(self, file_path: Path, content: str, lines: List[str]):
        """检查record类型（C# 9）"""
        pattern = re.compile(r'^\s*(public|private|protected|internal)?\s*record\s+(class|struct)?\s+\w+', re.MULTILINE)

        for match in pattern.finditer(content):
            line_num = content[:match.start()].count('\n') + 1
            self.issues.append({
                'file': str(file_path),
                'line': line_num,
                'type': 'RECORD_TYPE',
                'message': f'C# 9.0 record type: {match.group()}'
            })

    def _check_file_scoped_namespace(self, file_path: Path, content: str, lines: List[str]):
        """检查file-scoped namespace（C# 10）"""
        pattern = re.compile(r'^\s*namespace\s+[\w\.]+\s*;\s*$', re.MULTILINE)

        for match in pattern.finditer(content):
            line_num = content[:match.start()].count('\n') + 1
            self.issues.append({
                'file': str(file_path),
                'line': line_num,
                'type': 'FILE_SCOPED_NAMESPACE',
                'message': f'C# 10.0 file-scoped namespace: {match.group().strip()}'
            })

    def _check_with_expressions(self, file_path: Path, content: str, lines: List[str]):
        """检查with表达式（C# 9）"""
        pattern = re.compile(r'\s+with\s*\{')

        for i, line in enumerate(lines, 1):
            if pattern.search(line) and 'with' in line:
                self.issues.append({
                    'file': str(file_path),
                    'line': i,
                    'type': 'WITH_EXPRESSION',
                    'message': f'C# 9.0 with expression: {line.strip()[:60]}'
                })

    def _check_top_level_statements(self, file_path: Path, content: str, lines: List[str]):
        """检查top-level statements（C# 9）"""
        # 简单检查：文件开始就是语句而不是using/namespace
        if lines:
            first_code_line = None
            for line in lines:
                stripped = line.strip()
                if stripped and not stripped.startswith('//') and not stripped.startswith('/*'):
                    first_code_line = stripped
                    break

            if first_code_line and not any(first_code_line.startswith(kw) for kw in ['using', 'namespace', '#', '/*']):
                # 可能是top-level statement
                pass  # 这个很难判断，暂不报告

def main():
    """主函数"""
    print("=" * 80)
    print("[*] C# Code Issue Scanner")
    print("=" * 80)
    print(f"Project Path: {PROJECT_PATH}")
    print("=" * 80)

    scanner = CodeScanner(PROJECT_PATH)
    issues = scanner.scan_all()

    print(f"\n[i] 扫描完成！发现 {len(issues)} 个潜在问题")

    if not issues:
        print("[OK] 没有发现问题！")
        return

    # 按类型分组
    by_type = {}
    for issue in issues:
        issue_type = issue['type']
        if issue_type not in by_type:
            by_type[issue_type] = []
        by_type[issue_type].append(issue)

    print("\n问题分类统计：")
    for issue_type, items in sorted(by_type.items(), key=lambda x: len(x[1]), reverse=True):
        print(f"  [{issue_type}]: {len(items)} 个")

    # 显示详细信息
    print("\n详细问题列表：")
    print("=" * 80)

    for i, issue in enumerate(issues[:50], 1):  # 只显示前50个
        rel_path = os.path.relpath(issue['file'], PROJECT_PATH)
        print(f"{i}. {rel_path}:{issue['line']}")
        print(f"   [{issue['type']}] {issue['message']}")
        print()

    if len(issues) > 50:
        print(f"... 还有 {len(issues) - 50} 个问题未显示")

    # 保存到文件
    output_file = os.path.join(os.path.dirname(PROJECT_PATH), "code_issues.txt")
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(f"Code Issues Report\n")
        f.write(f"Total Issues: {len(issues)}\n")
        f.write(f"=" * 80 + "\n\n")

        for i, issue in enumerate(issues, 1):
            rel_path = os.path.relpath(issue['file'], PROJECT_PATH)
            f.write(f"{i}. {rel_path}:{issue['line']}\n")
            f.write(f"   [{issue['type']}] {issue['message']}\n\n")

    print(f"\n[i] 详细报告已保存到: {output_file}")

if __name__ == "__main__":
    main()
