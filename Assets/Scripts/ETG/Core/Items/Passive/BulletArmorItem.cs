#nullable disable

public class BulletArmorItem : PassiveItem
    {
        public tk2dSpriteAnimation knightLibrary;
        private PlayerController m_player;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            this.m_player = player;
            if (!this.m_pickedUpThisRun)
                ++this.m_player.healthHaver.Armor;
            base.Pickup(player);
            GameManager.Instance.OnNewLevelFullyLoaded += new System.Action(this.GainArmorOnLevelLoad);
            this.ProcessLegendaryStatus(player);
        }

        private void ProcessLegendaryStatus(PlayerController player)
        {
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            PassiveItem passiveItem1 = (PassiveItem) null;
            PassiveItem passiveItem2 = (PassiveItem) null;
            PassiveItem passiveItem3 = (PassiveItem) null;
            PassiveItem passiveItem4 = (PassiveItem) null;
            for (int index = 0; index < player.passiveItems.Count; ++index)
            {
                if (player.passiveItems[index].DisplayName == "Gunknight Gauntlet")
                {
                    flag1 = true;
                    passiveItem3 = player.passiveItems[index];
                }
                if (player.passiveItems[index].DisplayName == "Gunknight Armor")
                {
                    flag2 = true;
                    passiveItem2 = player.passiveItems[index];
                }
                if (player.passiveItems[index].DisplayName == "Gunknight Helmet")
                {
                    flag3 = true;
                    passiveItem1 = player.passiveItems[index];
                }
                if (player.passiveItems[index].DisplayName == "Gunknight Greaves")
                {
                    flag4 = true;
                    passiveItem4 = player.passiveItems[index];
                }
            }
            if (flag1 && flag2 && flag3 && flag4)
            {
                passiveItem1.CanBeDropped = false;
                passiveItem1.CanBeSold = false;
                passiveItem2.CanBeDropped = false;
                passiveItem2.CanBeSold = false;
                passiveItem3.CanBeDropped = false;
                passiveItem3.CanBeSold = false;
                passiveItem4.CanBeDropped = false;
                passiveItem4.CanBeSold = false;
                player.OverrideAnimationLibrary = this.knightLibrary;
                player.SetOverrideShader(ShaderCache.Acquire(player.LocalShaderName));
                if (player.characterIdentity == PlayableCharacters.Eevee)
                    player.GetComponent<CharacterAnimationRandomizer>().AddOverrideAnimLibrary(this.knightLibrary);
                player.ownerlessStatModifiers.Add(new StatModifier()
                {
                    amount = -1000f,
                    modifyType = StatModifier.ModifyMethod.ADDITIVE,
                    statToBoost = PlayerStats.StatType.ReloadSpeed
                });
                player.ownerlessStatModifiers.Add(new StatModifier()
                {
                    amount = 3f,
                    modifyType = StatModifier.ModifyMethod.ADDITIVE,
                    statToBoost = PlayerStats.StatType.Curse
                });
                player.stats.RecalculateStats(player);
                if (PassiveItem.IsFlagSetForCharacter(player, typeof (BulletArmorItem)))
                    return;
                PassiveItem.IncrementFlag(player, typeof (BulletArmorItem));
            }
            else
            {
                if (!PassiveItem.IsFlagSetForCharacter(player, typeof (BulletArmorItem)))
                    return;
                PassiveItem.DecrementFlag(player, typeof (BulletArmorItem));
                player.OverrideAnimationLibrary = (tk2dSpriteAnimation) null;
                player.ClearOverrideShader();
                if (player.characterIdentity == PlayableCharacters.Eevee)
                    player.GetComponent<CharacterAnimationRandomizer>().RemoveOverrideAnimLibrary(this.knightLibrary);
                for (int index = 0; index < player.ownerlessStatModifiers.Count; ++index)
                {
                    if (player.ownerlessStatModifiers[index].statToBoost == PlayerStats.StatType.ReloadSpeed && (double) player.ownerlessStatModifiers[index].amount == -1000.0)
                    {
                        player.ownerlessStatModifiers.RemoveAt(index);
                        break;
                    }
                }
                player.stats.RecalculateStats(player);
            }
        }

        public void GainArmorOnLevelLoad() => ++this.m_player.healthHaver.Armor;

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            debrisObject.GetComponent<BulletArmorItem>().m_pickedUpThisRun = true;
            GameManager.Instance.OnNewLevelFullyLoaded -= new System.Action(this.GainArmorOnLevelLoad);
            this.ProcessLegendaryStatus(player);
            this.m_player = (PlayerController) null;
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            if (this.m_pickedUp && GameManager.HasInstance)
                GameManager.Instance.OnNewLevelFullyLoaded -= new System.Action(this.GainArmorOnLevelLoad);
            base.OnDestroy();
        }
    }

