#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
自动化Unity编译错误修复脚本
功能：触发Unity编译 → 捕获错误 → 自动修复 → 循环直到0错误
"""

import os
import re
import subprocess
import time
import json
from pathlib import Path
from typing import List, Dict, Tuple, Optional
import shutil

# 配置
UNITY_PATH = r"C:\Program Files\Unity\Hub\Editor\2022.3.62f1\Editor\Unity.exe"
PROJECT_PATH = r"D:\Github\Re-ETG"
LOG_FILE = os.path.join(PROJECT_PATH, "auto_compile_log.txt")
ERROR_REPORT = os.path.join(PROJECT_PATH, "error_report.json")
MAX_ROUNDS = 20  # 最大修复轮次

class UnityCompiler:
    """Unity编译器管理类"""

    def __init__(self, unity_path: str, project_path: str):
        self.unity_path = unity_path
        self.project_path = project_path
        self.log_file = LOG_FILE

    def compile(self, timeout: int = 180) -> Tuple[bool, str]:
        """触发Unity编译并捕获输出"""
        print("[*] Triggering Unity compilation...")

        # 清除旧日志
        if os.path.exists(self.log_file):
            os.remove(self.log_file)

        cmd = [
            self.unity_path,
            "-quit",
            "-batchmode",
            "-projectPath", self.project_path,
            "-logFile", self.log_file,
            "-executeMethod", "UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation"
        ]

        try:
            # 启动Unity进程
            process = subprocess.Popen(
                cmd,
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                cwd=self.project_path
            )

            # 等待一段时间让Unity初始化
            time.sleep(5)

            # 等待编译完成或超时
            start_time = time.time()
            while True:
                # 检查是否超时
                if time.time() - start_time > timeout:
                    print("[!] 编译超时，终止进程...")
                    process.kill()
                    break

                # 检查进程是否结束
                if process.poll() is not None:
                    break

                # 检查日志文件是否存在且有内容
                if os.path.exists(self.log_file):
                    with open(self.log_file, 'r', encoding='utf-8', errors='ignore') as f:
                        content = f.read()
                        # 如果日志包含编译完成标记，可以提前退出
                        if "CompilationPipeline" in content and len(content) > 1000:
                            time.sleep(3)  # 再等待3秒确保日志完整
                            break

                time.sleep(2)

            # 读取日志
            if os.path.exists(self.log_file):
                with open(self.log_file, 'r', encoding='utf-8', errors='ignore') as f:
                    log_content = f.read()
                return True, log_content
            else:
                return False, "日志文件未生成"

        except Exception as e:
            return False, f"编译失败: {str(e)}"


class ErrorParser:
    """编译错误解析器"""

    # CS错误模式
    ERROR_PATTERN = re.compile(
        r'(?P<file>[^:]+\.cs)\((?P<line>\d+),(?P<col>\d+)\):\s*error\s+(?P<code>CS\d+):\s*(?P<message>.+?)(?=\n|$)',
        re.MULTILINE
    )

    @staticmethod
    def parse_errors(log_content: str) -> List[Dict]:
        """解析编译错误"""
        errors = []

        for match in ErrorParser.ERROR_PATTERN.finditer(log_content):
            error = {
                'file': match.group('file').strip(),
                'line': int(match.group('line')),
                'column': int(match.group('col')),
                'code': match.group('code'),
                'message': match.group('message').strip()
            }
            errors.append(error)

        return errors

    @staticmethod
    def categorize_errors(errors: List[Dict]) -> Dict[str, List[Dict]]:
        """按错误类型分类"""
        categories = {}

        for error in errors:
            code = error['code']
            if code not in categories:
                categories[code] = []
            categories[code].append(error)

        return categories


class AutoFixer:
    """自动修复器"""

    def __init__(self, project_path: str):
        self.project_path = project_path
        self.fixed_files = set()

    def fix_errors(self, errors: List[Dict]) -> Tuple[int, List[str]]:
        """自动修复错误"""
        fixed_count = 0
        fix_log = []

        # 按错误类型分组
        categories = ErrorParser.categorize_errors(errors)

        print(f"\n[i] 发现 {len(errors)} 个错误，分为 {len(categories)} 类：")
        for code, errs in sorted(categories.items()):
            print(f"  - {code}: {len(errs)} 个")

        # 按错误类型修复
        for code, errs in categories.items():
            if code == "CS1519":  # Invalid token
                count, log = self._fix_cs1519(errs)
                fixed_count += count
                fix_log.extend(log)
            elif code == "CS1002":  # ; expected
                count, log = self._fix_cs1002(errs)
                fixed_count += count
                fix_log.extend(log)
            elif code in ["CS0246", "CS0103"]:  # Type not found / Name does not exist
                count, log = self._fix_missing_types(errs)
                fixed_count += count
                fix_log.extend(log)
            elif code == "CS8773":  # File-scoped namespace
                count, log = self._fix_file_scoped_namespace(errs)
                fixed_count += count
                fix_log.extend(log)
            else:
                # 其他未知错误，记录但不修复
                fix_log.append(f"[!] 未知错误类型 {code}，需要人工检查")

        return fixed_count, fix_log

    def _fix_cs1519(self, errors: List[Dict]) -> Tuple[int, List[str]]:
        """修复CS1519: Invalid token错误（可能是primary constructor遗漏）"""
        fixed_count = 0
        fix_log = []

        # 按文件分组
        files_with_errors = {}
        for error in errors:
            file_path = self._resolve_path(error['file'])
            if file_path not in files_with_errors:
                files_with_errors[file_path] = []
            files_with_errors[file_path].append(error)

        for file_path, file_errors in files_with_errors.items():
            if not os.path.exists(file_path):
                continue

            try:
                with open(file_path, 'r', encoding='utf-8') as f:
                    content = f.read()

                # 检查是否有primary constructor模式
                # 匹配：struct Name(params) 或 class Name(params)
                primary_constructor_pattern = re.compile(
                    r'((?:public|private|protected|internal)?\s+(?:readonly\s+)?(?:struct|class)\s+(\w+))\s*\(([^)]+)\)\s*\{',
                    re.MULTILINE
                )

                matches = list(primary_constructor_pattern.finditer(content))
                if matches:
                    fix_log.append(f"[>] 修复 {os.path.basename(file_path)} 中的 primary constructor")

                    # 从后往前替换，避免位置偏移
                    for match in reversed(matches):
                        # 提取信息
                        declaration = match.group(1)  # "public struct Name"
                        name = match.group(2)  # "Name"
                        params = match.group(3)  # "int x, int y"

                        # 解析参数
                        param_list = [p.strip() for p in params.split(',')]

                        # 生成字段声明和构造函数
                        fields = []
                        assignments = []

                        for param in param_list:
                            parts = param.split()
                            if len(parts) >= 2:
                                param_type = ' '.join(parts[:-1])
                                param_name = parts[-1]

                                # 字段声明
                                fields.append(f"    public {param_type} {param_name};")

                                # 构造函数赋值
                                assignments.append(f"      this.{param_name} = {param_name};")

                        # 构建新的结构
                        new_code = f"{declaration}\n  {{\n"
                        new_code += "\n".join(fields) + "\n\n"
                        new_code += f"    public {name}({params})\n    {{\n"
                        new_code += "\n".join(assignments) + "\n"
                        new_code += "    }\n"

                        # 替换原有代码（只替换声明部分，保留方法体）
                        start = match.start()
                        end = match.end() - 1  # 不包含 '{'

                        content = content[:start] + new_code + content[end+1:]

                    # 写回文件
                    with open(file_path, 'w', encoding='utf-8') as f:
                        f.write(content)

                    fixed_count += len(matches)
                    self.fixed_files.add(file_path)

            except Exception as e:
                fix_log.append(f"[X] 修复 {file_path} 失败: {str(e)}")

        return fixed_count, fix_log

    def _fix_cs1002(self, errors: List[Dict]) -> Tuple[int, List[str]]:
        """修复CS1002: ; expected错误"""
        # 这种错误通常需要人工检查
        return 0, [f"[!] CS1002 错误需要人工检查（{len(errors)} 个）"]

    def _fix_missing_types(self, errors: List[Dict]) -> Tuple[int, List[str]]:
        """修复缺失类型错误"""
        # 这种错误通常是缺少using或类型不存在，需要人工处理
        return 0, [f"[!] 类型缺失错误需要人工检查（{len(errors)} 个）"]

    def _fix_file_scoped_namespace(self, errors: List[Dict]) -> Tuple[int, List[str]]:
        """修复file-scoped namespace错误"""
        fixed_count = 0
        fix_log = []

        files_with_errors = set(self._resolve_path(e['file']) for e in errors)

        for file_path in files_with_errors:
            if not os.path.exists(file_path):
                continue

            try:
                with open(file_path, 'r', encoding='utf-8') as f:
                    lines = f.readlines()

                # 查找file-scoped namespace
                new_lines = []
                namespace_found = False

                for i, line in enumerate(lines):
                    # 匹配 namespace Xxx;
                    if re.match(r'^\s*namespace\s+[\w\.]+\s*;\s*$', line):
                        namespace_name = re.search(r'namespace\s+([\w\.]+)', line).group(1)
                        new_lines.append(f"namespace {namespace_name}\n")
                        new_lines.append("{\n")
                        namespace_found = True
                    else:
                        new_lines.append(line)

                # 如果找到了file-scoped namespace，在文件末尾添加闭合括号
                if namespace_found:
                    # 移除末尾空行
                    while new_lines and new_lines[-1].strip() == "":
                        new_lines.pop()
                    new_lines.append("}\n")

                    with open(file_path, 'w', encoding='utf-8') as f:
                        f.writelines(new_lines)

                    fixed_count += 1
                    fix_log.append(f"[>] 修复 {os.path.basename(file_path)} 的 file-scoped namespace")
                    self.fixed_files.add(file_path)

            except Exception as e:
                fix_log.append(f"[X] 修复 {file_path} 失败: {str(e)}")

        return fixed_count, fix_log

    def _resolve_path(self, relative_path: str) -> str:
        """将相对路径转换为绝对路径"""
        # Unity的错误路径通常是相对于Assets的
        if not os.path.isabs(relative_path):
            # 尝试多种可能的路径
            candidates = [
                os.path.join(self.project_path, relative_path),
                os.path.join(self.project_path, "Assets", relative_path),
                relative_path
            ]

            for path in candidates:
                if os.path.exists(path):
                    return os.path.normpath(path)

            # 如果都不存在，返回第一个候选
            return os.path.normpath(candidates[0])

        return os.path.normpath(relative_path)


def main():
    """主函数"""
    # 设置Windows控制台UTF-8编码
    if os.name == 'nt':
        import sys
        if sys.stdout.encoding != 'utf-8':
            import codecs
            sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
            sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')

    print("=" * 80)
    print("[*] Unity Compilation Error Auto-Fix System")
    print("=" * 80)
    print(f"Project Path: {PROJECT_PATH}")
    print(f"Unity Path: {UNITY_PATH}")
    print(f"Max Rounds: {MAX_ROUNDS}")
    print("=" * 80)

    # 初始化
    compiler = UnityCompiler(UNITY_PATH, PROJECT_PATH)
    fixer = AutoFixer(PROJECT_PATH)

    # 修复循环
    round_num = 0
    total_fixed = 0

    while round_num < MAX_ROUNDS:
        round_num += 1
        print(f"\n{'=' * 80}")
        print(f"[>] Round {round_num} / {MAX_ROUNDS}")
        print(f"{'=' * 80}")

        # 编译
        success, log_content = compiler.compile()

        if not success:
            print(f"[X] 编译失败: {log_content}")
            break

        # 解析错误
        errors = ErrorParser.parse_errors(log_content)

        if not errors:
            print("[OK] 编译成功！没有发现错误！")
            print(f"\n[*] 修复完成！共进行 {round_num} 轮，修复 {total_fixed} 个错误")
            break

        print(f"\n[i] 发现 {len(errors)} 个编译错误")

        # 保存错误报告
        with open(ERROR_REPORT, 'w', encoding='utf-8') as f:
            json.dump(errors, f, indent=2, ensure_ascii=False)
        print(f"[i] 错误报告已保存到: {ERROR_REPORT}")

        # 显示前5个错误
        print("\n前5个错误示例：")
        for i, error in enumerate(errors[:5], 1):
            print(f"  {i}. [{error['code']}] {os.path.basename(error['file'])}:{error['line']}")
            print(f"     {error['message'][:100]}")

        # 自动修复
        fixed_count, fix_log = fixer.fix_errors(errors)

        print(f"\n[>] 本轮修复: {fixed_count} 个问题")
        for log in fix_log:
            print(f"  {log}")

        total_fixed += fixed_count

        # 如果没有修复任何错误，说明无法自动修复，退出
        if fixed_count == 0:
            print("\n[!] 无法自动修复剩余错误，需要人工介入")
            print(f"剩余 {len(errors)} 个错误，请查看 {ERROR_REPORT} 获取详情")

            # 显示错误统计
            categories = ErrorParser.categorize_errors(errors)
            print("\n错误类型统计：")
            for code, errs in sorted(categories.items(), key=lambda x: len(x[1]), reverse=True):
                print(f"  - {code}: {len(errs)} 个")
                # 显示该类型的第一个错误示例
                example = errs[0]
                print(f"    示例: {os.path.basename(example['file'])}:{example['line']}")
                print(f"    {example['message'][:80]}...")

            break

        print(f"\n等待3秒后进入下一轮...")
        time.sleep(3)

    if round_num >= MAX_ROUNDS:
        print(f"\n[!] 达到最大轮次限制 ({MAX_ROUNDS})，停止修复")
        print(f"已修复 {total_fixed} 个错误")

    # 显示修改的文件
    if fixer.fixed_files:
        print(f"\n[i] 本次修改的文件 ({len(fixer.fixed_files)} 个):")
        for file_path in sorted(fixer.fixed_files):
            print(f"  - {os.path.relpath(file_path, PROJECT_PATH)}")

    print("\n" + "=" * 80)
    print("[*] 程序结束")
    print("=" * 80)


if __name__ == "__main__":
    main()
