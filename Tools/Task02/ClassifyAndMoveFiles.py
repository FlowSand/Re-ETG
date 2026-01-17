#!/usr/bin/env python3
"""
Task-02: 命名空间分类脚本
分类2,323个根目录C#文件到ETG.Core.*命名空间
"""

import os
import re
from pathlib import Path
from typing import Tuple, Optional, List
from dataclasses import dataclass
import json

@dataclass
class FileClassification:
    """文件分类结果"""
    source_path: Path
    namespace: str
    target_directory: str
    class_name: str
    inheritance: Optional[str]
    confidence: str  # 'high', 'medium', 'low'
    reason: str

class NamespaceClassifier:
    """命名空间分类器"""

    def __init__(self, root_path: str):
        self.root_path = Path(root_path)
        self.classifications = []

        # 命名空间 -> 相对目录路径映射
        self.namespace_map = {
            'ETG.Core.Actors.Player': 'Core/Actors/Player',
            'ETG.Core.Actors.Enemy': 'Core/Actors/Enemy',
            'ETG.Core.Actors.Companions': 'Core/Actors/Companions',
            'ETG.Core.Actors.Behaviors': 'Core/Actors/Behaviors',
            'ETG.Core.Items.Guns': 'Core/Items/Guns',
            'ETG.Core.Items.Passive': 'Core/Items/Passive',
            'ETG.Core.Items.Active': 'Core/Items/Active',
            'ETG.Core.Items.Pickups': 'Core/Items/Pickups',
            'ETG.Core.Combat.Projectiles': 'Core/Combat/Projectiles',
            'ETG.Core.Combat.Damage': 'Core/Combat/Damage',
            'ETG.Core.Combat.Effects': 'Core/Combat/Effects',
            'ETG.Core.Dungeon.Generation': 'Core/Dungeon/Generation',
            'ETG.Core.Dungeon.Rooms': 'Core/Dungeon/Rooms',
            'ETG.Core.Dungeon.Interactables': 'Core/Dungeon/Interactables',
            'ETG.Core.UI.Ammonomicon': 'Core/UI/Ammonomicon',
            'ETG.Core.UI.HUD': 'Core/UI/HUD',
            'ETG.Core.UI.Menus': 'Core/UI/Menus',
            'ETG.Core.VFX.Animation': 'Core/VFX/Animation',
            'ETG.Core.VFX.Particles': 'Core/VFX/Particles',
            'ETG.Core.VFX.Rendering': 'Core/VFX/Rendering',
            'ETG.Core.Audio.Integration': 'Core/Audio/Integration',
            'ETG.Core.Systems.Management': 'Core/Systems/Management',
            'ETG.Core.Systems.Data': 'Core/Systems/Data',
            'ETG.Core.Systems.Utilities': 'Core/Systems/Utilities',
            'ETG.Core.Core.Framework': 'Core/Core/Framework',
            'ETG.Core.Core.Interfaces': 'Core/Core/Interfaces',
            'ETG.Core.Core.Enums': 'Core/Core/Enums',
        }

    def classify_file(self, file_path: Path) -> FileClassification:
        """分类单个C#文件"""
        try:
            content = file_path.read_text(encoding='utf-8-sig', errors='ignore')
        except Exception as e:
            print(f"警告: 无法读取文件 {file_path.name}: {e}")
            return self._create_fallback_classification(file_path, str(e))

        # 提取类名和继承关系
        class_match = re.search(
            r'^\s*(?:public|internal|private|protected)?\s*(?:abstract|sealed|static|partial)?\s*(?:class|struct|enum|interface)\s+(\w+)(?:\s*:\s*([^{]+?))?(?:\s*where|\s*{)',
            content, re.MULTILINE
        )

        if not class_match:
            return self._create_fallback_classification(file_path, '未找到类定义')

        class_name = class_match.group(1)
        inheritance = class_match.group(2).strip() if class_match.group(2) else None

        # 应用分类规则
        namespace, reason, confidence = self._determine_namespace(
            file_path.name, class_name, inheritance, content
        )

        return FileClassification(
            source_path=file_path,
            namespace=namespace,
            target_directory=self.namespace_map[namespace],
            class_name=class_name,
            inheritance=inheritance,
            confidence=confidence,
            reason=reason
        )

    def _determine_namespace(
        self, filename: str, class_name: str,
        inheritance: Optional[str], content: str
    ) -> Tuple[str, str, str]:
        """确定命名空间（优先级规则）"""

        # === 优先级1: 文件名模式 ===
        if filename.endswith('Manager.cs') and not 'Ak' in filename:
            return 'ETG.Core.Systems.Management', f'文件名模式: *Manager', 'high'

        if 'Database' in filename or (filename.endswith('Data.cs') and 'static' in content[:1000]):
            return 'ETG.Core.Systems.Data', f'文件名模式: Database/StaticData', 'high'

        if filename.startswith('Ammonom'):
            return 'ETG.Core.UI.Ammonomicon', f'文件名模式: Ammonom*', 'high'

        if filename.startswith('Ak') or 'Audio' in filename:
            return 'ETG.Core.Audio.Integration', f'文件名模式: Ak*/Audio', 'high'

        if any(kw in filename for kw in ['Shrine', 'Shop', 'NPC']):
            return 'ETG.Core.Dungeon.Interactables', f'文件名模式: Shrine/Shop/NPC', 'high'

        if filename.startswith('Actionbars'):
            return 'ETG.Core.UI.HUD', f'文件名模式: Actionbars*', 'high'

        # === 优先级2: 继承层次 ===
        if inheritance:
            inheritance_clean = inheritance.split(',')[0].strip()

            # 玩家控制器
            if 'PlayerController' in inheritance:
                return 'ETG.Core.Actors.Player', f'继承: PlayerController', 'high'

            # 敌人/角色
            if 'AIActor' in inheritance:
                return 'ETG.Core.Actors.Enemy', f'继承: AIActor', 'high'
            if 'GameActor' in inheritance:
                return 'ETG.Core.Actors.Enemy', f'继承: GameActor', 'high'

            # 物品系统
            if inheritance_clean == 'Gun':
                return 'ETG.Core.Items.Guns', f'继承: Gun', 'high'
            if 'PassiveItem' in inheritance:
                return 'ETG.Core.Items.Passive', f'继承: PassiveItem', 'high'
            if 'ActiveItem' in inheritance:
                return 'ETG.Core.Items.Active', f'继承: ActiveItem', 'high'
            if 'PlayerItem' in inheritance:
                return 'ETG.Core.Items.Active', f'继承: PlayerItem', 'high'
            if 'PickupObject' in inheritance:
                return 'ETG.Core.Items.Pickups', f'继承: PickupObject', 'high'

            # 战斗系统
            if 'Projectile' in inheritance:
                return 'ETG.Core.Combat.Projectiles', f'继承: Projectile', 'high'

            # 行为系统
            if 'Behavior' in inheritance:
                return 'ETG.Core.Actors.Behaviors', f'继承: *Behavior', 'high'

            # 地牢系统
            if 'DungeonPlaceableBehaviour' in inheritance:
                return 'ETG.Core.Dungeon.Interactables', f'继承: DungeonPlaceableBehaviour', 'high'
            if 'SpecificIntroDoer' in inheritance:
                return 'ETG.Core.Dungeon.Interactables', f'继承: SpecificIntroDoer', 'high'
            if 'RobotRoomFeature' in inheritance:
                return 'ETG.Core.Dungeon.Rooms', f'继承: RobotRoomFeature', 'high'

            # UI系统
            if any(ui_class in inheritance for ui_class in ['dfControl', 'dfMarkupTag', 'dfGestureBase']):
                return 'ETG.Core.UI.HUD', f'继承: dfControl/UI组件', 'high'

            # 系统/数据
            if 'ChallengeModifier' in inheritance:
                return 'ETG.Core.Systems.Data', f'继承: ChallengeModifier', 'high'

            # 核心框架
            if 'BraveBehaviour' in inheritance:
                return 'ETG.Core.Core.Framework', f'继承: BraveBehaviour', 'high'
            if 'TimeInvariantMonoBehaviour' in inheritance:
                return 'ETG.Core.Core.Framework', f'继承: TimeInvariantMonoBehaviour', 'high'

        # === 优先级3: 关键字分析 ===
        # 战斗相关
        if 'Projectile' in class_name or 'Projectile' in content[:2000]:
            return 'ETG.Core.Combat.Projectiles', f'关键字: Projectile', 'medium'

        if any(kw in class_name for kw in ['Effect', 'Buff', 'Debuff', 'Status']):
            return 'ETG.Core.Combat.Effects', f'关键字: Effect/Buff', 'medium'

        # VFX相关
        if any(kw in class_name for kw in ['VFX', 'Particle', 'Animation', 'Sprite']) and not 'Controller' in class_name:
            return 'ETG.Core.VFX.Animation', f'关键字: VFX/Animation', 'medium'

        # 行为系统
        if 'Behavior' in class_name:
            return 'ETG.Core.Actors.Behaviors', f'关键字: Behavior', 'medium'

        # 地牢相关
        if any(kw in class_name for kw in ['Room', 'Dungeon', 'Floor']) and 'Controller' in class_name:
            return 'ETG.Core.Dungeon.Rooms', f'关键字: Room/Dungeon Controller', 'medium'

        if class_name.startswith('Prototype') and 'Dungeon' in class_name:
            return 'ETG.Core.Dungeon.Generation', f'关键字: PrototypeDungeon*', 'medium'

        # Data类分类（更细致）
        if class_name.endswith('Data'):
            # 战斗相关Data
            if any(kw in class_name for kw in ['Knockback', 'Reload', 'Ammunition', 'Projectile']):
                return 'ETG.Core.Combat.Damage', f'关键字: Combat Data', 'medium'
            # 物品相关Data
            if any(kw in class_name for kw in ['Gun', 'Item', 'Pickup']):
                return 'ETG.Core.Items.Guns', f'关键字: Item Data', 'medium'
            # 地牢相关Data
            if any(kw in class_name for kw in ['Boss', 'Challenge', 'Floor', 'Loot', 'Reward', 'Encounter']):
                return 'ETG.Core.Systems.Data', f'关键字: Game Data', 'medium'
            # 玩家/存档相关Data
            if any(kw in class_name for kw in ['Player', 'Save', 'MidGame', 'Run']):
                return 'ETG.Core.Systems.Data', f'关键字: Save Data', 'medium'
            # 其他Data类
            if not inheritance:
                return 'ETG.Core.Systems.Data', f'关键字: Data类', 'medium'

        # UI相关
        if any(kw in class_name for kw in ['UI', 'ViewModel', 'Menu']):
            return 'ETG.Core.UI.HUD', f'关键字: UI', 'medium'

        # 物品推测
        if 'Item' in class_name and not inheritance:
            return 'ETG.Core.Items.Passive', f'关键字: Item (推测Passive)', 'low'

        # === 优先级4: 类型回退 ===
        if inheritance:
            inheritance_clean = inheritance.split(',')[0].strip()

            # Attribute类
            if 'Attribute' in inheritance:
                return 'ETG.Core.Core.Framework', f'回退: Attribute', 'medium'

            # ScriptableObject（如果前面没匹配到）
            if 'ScriptableObject' in inheritance:
                return 'ETG.Core.Systems.Data', f'回退: ScriptableObject', 'medium'

            # 通用MonoBehaviour（如果前面没匹配到）
            if inheritance_clean == 'MonoBehaviour':
                return 'ETG.Core.Systems.Utilities', f'回退: MonoBehaviour', 'low'

            # Script基类
            if inheritance_clean == 'Script':
                return 'ETG.Core.Systems.Utilities', f'回退: Script', 'low'

        content_lower = content[:500].lower()

        # Interface
        if 'interface' in content_lower and class_name.startswith('I'):
            return 'ETG.Core.Core.Interfaces', f'回退: Interface', 'medium'

        # Enum
        if 'enum' in content_lower or re.search(r'^\s*public\s+enum\s+' + class_name, content, re.MULTILINE):
            return 'ETG.Core.Core.Enums', f'回退: Enum', 'medium'

        # 默认
        return 'ETG.Core.Systems.Utilities', f'回退: 未知 (需审核)', 'low'

    def _create_fallback_classification(self, path: Path, reason: str):
        """创建回退分类"""
        return FileClassification(
            source_path=path,
            namespace='ETG.Core.Systems.Utilities',
            target_directory=self.namespace_map['ETG.Core.Systems.Utilities'],
            class_name='Unknown',
            inheritance=None,
            confidence='low',
            reason=f'错误: {reason}'
        )

    def classify_all_root_files(self) -> List[FileClassification]:
        """分类所有根目录C#文件"""
        root_cs_files = [
            f for f in self.root_path.glob('*.cs')
            if f.is_file()
        ]

        print(f"找到 {len(root_cs_files)} 个根目录C#文件")

        for i, cs_file in enumerate(root_cs_files, 1):
            if i % 100 == 0:
                print(f"进度: {i}/{len(root_cs_files)}")

            classification = self.classify_file(cs_file)
            self.classifications.append(classification)

        return self.classifications

    def generate_report(self, output_path: Path):
        """生成分类报告"""
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write("# Task-02: 命名空间分类报告\n\n")
            f.write(f"**分类文件总数:** {len(self.classifications)}\n\n")

            # 按命名空间分组
            by_namespace = {}
            for c in self.classifications:
                by_namespace.setdefault(c.namespace, []).append(c)

            f.write("## 分类汇总\n\n")
            f.write("| 命名空间 | 文件数 | 高置信 | 中置信 | 低置信 |\n")
            f.write("|---------|-------|--------|--------|--------|\n")

            for ns in sorted(by_namespace.keys()):
                files = by_namespace[ns]
                high = sum(1 for c in files if c.confidence == 'high')
                med = sum(1 for c in files if c.confidence == 'medium')
                low = sum(1 for c in files if c.confidence == 'low')
                f.write(f"| {ns} | {len(files)} | {high} | {med} | {low} |\n")

            # 低置信度文件需要审核
            f.write("\n## 需要人工审核的文件（低置信度）\n\n")
            low_conf = [c for c in self.classifications if c.confidence == 'low']
            f.write(f"**低置信度文件数:** {len(low_conf)}\n\n")

            for c in sorted(low_conf, key=lambda x: x.source_path.name):
                f.write(f"- **{c.source_path.name}** → `{c.namespace}`\n")
                f.write(f"  - 类名: {c.class_name}\n")
                f.write(f"  - 继承: {c.inheritance or '无'}\n")
                f.write(f"  - 原因: {c.reason}\n\n")

            # 完整分类表
            f.write("\n## 完整分类表\n\n")
            f.write("| 文件 | 类名 | 命名空间 | 置信度 | 原因 |\n")
            f.write("|------|------|----------|--------|------|\n")

            for c in sorted(self.classifications, key=lambda x: (x.namespace, x.source_path.name)):
                f.write(f"| {c.source_path.name} | {c.class_name} | {c.namespace} | {c.confidence} | {c.reason} |\n")

        print(f"[OK] 分类报告已生成: {output_path}")

    def save_to_json(self, output_path: Path):
        """保存分类结果到JSON"""
        data = [
            {
                'file': str(c.source_path),
                'namespace': c.namespace,
                'target_dir': c.target_directory,
                'class_name': c.class_name,
                'inheritance': c.inheritance,
                'confidence': c.confidence,
                'reason': c.reason
            }
            for c in self.classifications
        ]

        with open(output_path, 'w', encoding='utf-8') as f:
            json.dump(data, f, indent=2, ensure_ascii=False)

        print(f"[OK] 分类结果已保存: {output_path}")


def main():
    """主函数"""
    import sys

    # 配置路径
    etg_root = Path(r"D:\Github\Re-ETG\Assets\Scripts\ETG")
    docs_dir = Path(r"D:\Github\Re-ETG\Docs")

    if not etg_root.exists():
        print(f"错误: 路径不存在: {etg_root}")
        sys.exit(1)

    print("=" * 60)
    print("Task-02: 文件命名空间分类")
    print("=" * 60)
    print()

    # 创建分类器
    print("初始化分类器...")
    classifier = NamespaceClassifier(str(etg_root))

    # 执行分类
    print("\n开始分类文件...")
    classifications = classifier.classify_all_root_files()

    print(f"\n[OK] 分类完成！共分类 {len(classifications)} 个文件")

    # 生成报告
    report_path = docs_dir / "Namespace_Classification_Report.md"
    json_path = docs_dir / "classification.json"

    print("\n生成分类报告...")
    classifier.generate_report(report_path)
    classifier.save_to_json(json_path)

    # 统计信息
    by_confidence = {}
    for c in classifications:
        by_confidence[c.confidence] = by_confidence.get(c.confidence, 0) + 1

    print("\n分类统计:")
    print(f"  高置信度: {by_confidence.get('high', 0)} 个文件")
    print(f"  中置信度: {by_confidence.get('medium', 0)} 个文件")
    print(f"  低置信度: {by_confidence.get('low', 0)} 个文件")

    print(f"\n请查看报告: {report_path}")
    print("重点审核低置信度文件的分类是否正确。")
    print("\n下一步: 运行 MigrateFiles.py 执行文件迁移")


if __name__ == '__main__':
    main()
