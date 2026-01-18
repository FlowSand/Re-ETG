using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class GameUIHeartController : MonoBehaviour, ILevelLoadedListener
    {
        public dfSprite heartSpritePrefab;
        public dfSprite damagedHeartAnimationPrefab;
        public Vector3 damagedPrefabOffset;
        public dfSprite armorSpritePrefab;
        public dfSprite damagedArmorAnimationPrefab;
        public Vector3 damagedArmorPrefabOffset;
        public dfSprite crestSpritePrefab;
        public dfSprite damagedCrestAnimationPrefab;
        public List<dfSprite> extantHearts;
        public List<dfSprite> extantArmors;
        public string fullHeartSpriteName;
        public string halfHeartSpriteName;
        public string emptyHeartSpriteName;
        public bool IsRightAligned;
        private dfPanel m_panel;
        private string m_currentFullHeartName;
        private string m_currentHalfHeartName;
        private string m_currentEmptyHeartName;
        private string m_currentArmorName;

        public dfPanel Panel => this.m_panel;

        private void Awake()
        {
            this.m_currentFullHeartName = this.fullHeartSpriteName;
            this.m_currentHalfHeartName = this.halfHeartSpriteName;
            this.m_currentEmptyHeartName = this.emptyHeartSpriteName;
            this.m_currentArmorName = this.armorSpritePrefab.SpriteName;
            this.m_panel = this.GetComponent<dfPanel>();
            this.extantHearts = new List<dfSprite>();
            this.extantArmors = new List<dfSprite>();
        }

        private void Start()
        {
            foreach (Object component in this.GetComponents<Collider>())
                Object.Destroy(component);
        }

        public void BraveOnLevelWasLoaded()
        {
            if (this.extantHearts != null)
            {
                for (int index = 0; index < this.extantHearts.Count; ++index)
                {
                    if (!(bool) (Object) this.extantHearts[index])
                    {
                        this.extantHearts.RemoveAt(index);
                        --index;
                    }
                }
            }
            if (this.extantArmors == null)
                return;
            for (int index = 0; index < this.extantArmors.Count; ++index)
            {
                if (!(bool) (Object) this.extantArmors[index])
                {
                    this.extantArmors.RemoveAt(index);
                    --index;
                }
            }
        }

        public void UpdateScale()
        {
            for (int index = 0; index < this.extantHearts.Count; ++index)
            {
                dfSprite extantHeart = this.extantHearts[index];
                if ((bool) (Object) extantHeart)
                {
                    Vector2 sizeInPixels = extantHeart.SpriteInfo.sizeInPixels;
                    extantHeart.Size = sizeInPixels * Pixelator.Instance.CurrentTileScale;
                }
            }
            for (int index = 0; index < this.extantArmors.Count; ++index)
            {
                dfSprite extantArmor = this.extantArmors[index];
                if ((bool) (Object) extantArmor)
                {
                    Vector2 sizeInPixels = extantArmor.SpriteInfo.sizeInPixels;
                    extantArmor.Size = sizeInPixels * Pixelator.Instance.CurrentTileScale;
                }
            }
        }

        public void AddArmor()
        {
            GameObject gameObject = Object.Instantiate<GameObject>(this.armorSpritePrefab.gameObject, this.transform.position, Quaternion.identity);
            gameObject.transform.parent = this.transform.parent;
            gameObject.layer = this.gameObject.layer;
            dfSprite component = gameObject.GetComponent<dfSprite>();
            if (this.IsRightAligned)
                component.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Right;
            Vector2 sizeInPixels = component.SpriteInfo.sizeInPixels;
            component.Size = sizeInPixels * Pixelator.Instance.CurrentTileScale;
            component.IsInteractive = false;
            if (!this.IsRightAligned)
            {
                float num1 = this.extantHearts.Count <= 0 ? 0.0f : (this.extantHearts[0].Width + Pixelator.Instance.CurrentTileScale) * (float) this.extantHearts.Count;
                float num2 = (component.Width + Pixelator.Instance.CurrentTileScale) * (float) this.extantArmors.Count;
                component.RelativePosition = this.m_panel.RelativePosition + new Vector3(num1 + num2, 0.0f, 0.0f);
            }
            else
            {
                component.RelativePosition = this.m_panel.RelativePosition - new Vector3(component.Width, 0.0f, 0.0f);
                for (int index = 0; index < this.extantArmors.Count; ++index)
                {
                    dfSprite extantArmor = this.extantArmors[index];
                    if ((bool) (Object) extantArmor)
                    {
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor, true, false, true);
                        extantArmor.RelativePosition = extantArmor.RelativePosition + new Vector3((float) (-1.0 * ((double) component.Width + (double) Pixelator.Instance.CurrentTileScale)), 0.0f, 0.0f);
                        GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantArmor);
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                    }
                }
                for (int index = 0; index < this.extantHearts.Count; ++index)
                {
                    dfSprite extantHeart = this.extantHearts[index];
                    if ((bool) (Object) extantHeart)
                    {
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantHeart, true, false, true);
                        extantHeart.RelativePosition = extantHeart.RelativePosition + new Vector3((float) (-1.0 * ((double) component.Width + (double) Pixelator.Instance.CurrentTileScale)), 0.0f, 0.0f);
                        GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantHeart);
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantHeart, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                    }
                }
            }
            this.extantArmors.Add(component);
            GameUIRoot.Instance.AddControlToMotionGroups((dfControl) component, !this.IsRightAligned ? DungeonData.Direction.WEST : DungeonData.Direction.EAST);
        }

        public void RemoveArmor()
        {
            if (this.extantArmors.Count <= 0)
                return;
            dfSprite dfSprite = this.damagedArmorAnimationPrefab;
            dfSprite extantArmor1 = this.extantArmors[this.extantArmors.Count - 1];
            if ((bool) (Object) extantArmor1)
            {
                if (extantArmor1.SpriteName == this.crestSpritePrefab.SpriteName)
                    dfSprite = this.damagedCrestAnimationPrefab;
                if ((Object) dfSprite != (Object) null)
                {
                    GameObject gameObject = Object.Instantiate<GameObject>(dfSprite.gameObject);
                    gameObject.transform.parent = this.transform.parent;
                    gameObject.layer = this.gameObject.layer;
                    dfSprite component = gameObject.GetComponent<dfSprite>();
                    component.BringToFront();
                    extantArmor1.Parent.AddControl((dfControl) component);
                    extantArmor1.Parent.BringToFront();
                    component.ZOrder = extantArmor1.ZOrder - 1;
                    component.RelativePosition = extantArmor1.RelativePosition + this.damagedArmorPrefabOffset;
                    this.m_panel.AddControl((dfControl) component);
                }
            }
            float width = this.extantArmors[0].Width;
            if ((bool) (Object) extantArmor1)
            {
                GameUIRoot.Instance.RemoveControlFromMotionGroups((dfControl) extantArmor1);
                Object.Destroy((Object) this.extantArmors[this.extantArmors.Count - 1]);
            }
            this.extantArmors.RemoveAt(this.extantArmors.Count - 1);
            if (!this.IsRightAligned)
                return;
            for (int index = 0; index < this.extantArmors.Count; ++index)
            {
                dfSprite extantArmor2 = this.extantArmors[index];
                if ((bool) (Object) extantArmor2)
                {
                    GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor2, true, false, true);
                    extantArmor2.RelativePosition = extantArmor2.RelativePosition + new Vector3(width + Pixelator.Instance.CurrentTileScale, 0.0f, 0.0f);
                    GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantArmor2);
                    GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor2, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                }
            }
            for (int index = 0; index < this.extantHearts.Count; ++index)
            {
                dfSprite extantHeart = this.extantHearts[index];
                if ((bool) (Object) extantHeart)
                {
                    GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantHeart, true, false, true);
                    extantHeart.RelativePosition = extantHeart.RelativePosition + new Vector3(width + Pixelator.Instance.CurrentTileScale, 0.0f, 0.0f);
                    GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantHeart);
                    GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantHeart, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                }
            }
        }

        private void ClearAllArmor()
        {
            if (this.extantArmors.Count <= 0)
                return;
            while (this.extantArmors.Count > 0)
                this.RemoveArmor();
        }

        public dfSprite AddHeart()
        {
            int count = this.extantArmors.Count;
            this.ClearAllArmor();
            GameObject gameObject = Object.Instantiate<GameObject>(this.heartSpritePrefab.gameObject, this.transform.position, Quaternion.identity);
            gameObject.transform.parent = this.transform.parent;
            gameObject.layer = this.gameObject.layer;
            dfSprite component = gameObject.GetComponent<dfSprite>();
            if (this.IsRightAligned)
                component.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Right;
            Vector2 sizeInPixels = component.SpriteInfo.sizeInPixels;
            component.Size = sizeInPixels * Pixelator.Instance.CurrentTileScale;
            component.IsInteractive = false;
            if (!this.IsRightAligned)
            {
                float x = (component.Width + Pixelator.Instance.CurrentTileScale) * (float) this.extantHearts.Count;
                component.RelativePosition = this.m_panel.RelativePosition + new Vector3(x, 0.0f, 0.0f);
            }
            else
            {
                component.RelativePosition = this.m_panel.RelativePosition - new Vector3(component.Width, 0.0f, 0.0f);
                for (int index = 0; index < this.extantHearts.Count; ++index)
                {
                    dfSprite extantHeart = this.extantHearts[index];
                    if ((bool) (Object) extantHeart)
                    {
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantHeart, true, false, true);
                        extantHeart.RelativePosition = extantHeart.RelativePosition + new Vector3((float) (-1.0 * ((double) component.Width + (double) Pixelator.Instance.CurrentTileScale)), 0.0f, 0.0f);
                        GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantHeart);
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantHeart, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                    }
                }
                for (int index = 0; index < this.extantArmors.Count; ++index)
                {
                    dfSprite extantArmor = this.extantArmors[index];
                    if ((bool) (Object) extantArmor)
                    {
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor, true, false, true);
                        extantArmor.RelativePosition = extantArmor.RelativePosition + new Vector3((float) (-1.0 * ((double) component.Width + (double) Pixelator.Instance.CurrentTileScale)), 0.0f, 0.0f);
                        GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantArmor);
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                    }
                }
            }
            this.extantHearts.Add(component);
            GameUIRoot.Instance.AddControlToMotionGroups((dfControl) component, !this.IsRightAligned ? DungeonData.Direction.WEST : DungeonData.Direction.EAST);
            for (int index = 0; index < count; ++index)
                this.AddArmor();
            return component;
        }

        public void RemoveHeart()
        {
            if (this.extantHearts.Count <= 0)
                return;
            float width = this.extantHearts[0].Width;
            dfSprite extantHeart1 = this.extantHearts[this.extantHearts.Count - 1];
            if ((bool) (Object) extantHeart1)
            {
                GameUIRoot.Instance.RemoveControlFromMotionGroups((dfControl) extantHeart1);
                Object.Destroy((Object) extantHeart1);
            }
            this.extantHearts.RemoveAt(this.extantHearts.Count - 1);
            if (this.IsRightAligned)
            {
                for (int index = 0; index < this.extantHearts.Count; ++index)
                {
                    dfSprite extantHeart2 = this.extantHearts[index];
                    if ((bool) (Object) extantHeart2)
                    {
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantHeart2, true, false, true);
                        extantHeart2.RelativePosition = extantHeart2.RelativePosition + new Vector3(width + Pixelator.Instance.CurrentTileScale, 0.0f, 0.0f);
                        GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantHeart2);
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantHeart2, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                    }
                }
            }
            else if (this.extantArmors != null && this.extantArmors.Count > 0 && this.extantHearts.Count > 0)
            {
                for (int index = 0; index < this.extantArmors.Count; ++index)
                {
                    float x = this.extantHearts[0].Size.x + Pixelator.Instance.CurrentTileScale;
                    dfSprite extantArmor = this.extantArmors[index];
                    if ((bool) (Object) extantArmor)
                    {
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor, true, false, true);
                        dfSprite dfSprite = extantArmor;
                        dfSprite.RelativePosition = dfSprite.RelativePosition - new Vector3(x, 0.0f, 0.0f);
                        GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantArmor);
                        GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                    }
                }
            }
            this.ClearAllArmor();
        }

        public void UpdateHealth(HealthHaver hh)
        {
            float f1 = hh.GetCurrentHealth();
            float maxHealth = hh.GetMaxHealth();
            float f2 = hh.Armor;
            if (hh.NextShotKills)
            {
                f1 = 0.5f;
                f2 = 0.0f;
            }
            int num1 = Mathf.CeilToInt(maxHealth);
            int num2 = Mathf.CeilToInt(f2);
            if (this.extantHearts.Count < num1)
            {
                for (int count = this.extantHearts.Count; count < num1; ++count)
                {
                    dfSprite dfSprite = this.AddHeart();
                    if ((double) (count + 1) > (double) f1)
                        dfSprite.SpriteName = (double) Mathf.FloorToInt(f1) == (double) f1 || (double) f1 + 1.0 <= (double) (count + 1) ? this.m_currentEmptyHeartName : this.m_currentHalfHeartName;
                }
            }
            else if (this.extantHearts.Count > num1)
            {
                while (this.extantHearts.Count > num1)
                    this.RemoveHeart();
            }
            if (this.extantArmors.Count < num2)
            {
                for (int count = this.extantArmors.Count; count < num2; ++count)
                    this.AddArmor();
            }
            else if (this.extantArmors.Count > num2)
            {
                while (this.extantArmors.Count > num2)
                    this.RemoveArmor();
            }
            int index1 = Mathf.FloorToInt(f1);
            for (int index2 = 0; index2 < this.extantHearts.Count; ++index2)
            {
                dfSprite extantHeart = this.extantHearts[index2];
                if ((bool) (Object) extantHeart)
                {
                    if (index2 < index1)
                        extantHeart.SpriteName = this.m_currentFullHeartName;
                    else if (index2 != index1 || (double) f1 - (double) index1 <= 0.0)
                    {
                        if (extantHeart.SpriteName == this.m_currentFullHeartName || extantHeart.SpriteName == this.m_currentHalfHeartName)
                        {
                            GameObject gameObject = Object.Instantiate<GameObject>(this.damagedHeartAnimationPrefab.gameObject);
                            gameObject.transform.parent = this.transform.parent;
                            gameObject.layer = this.gameObject.layer;
                            dfSprite component = gameObject.GetComponent<dfSprite>();
                            component.BringToFront();
                            extantHeart.Parent.AddControl((dfControl) component);
                            extantHeart.Parent.BringToFront();
                            component.ZOrder = extantHeart.ZOrder - 1;
                            component.RelativePosition = extantHeart.RelativePosition + this.damagedPrefabOffset;
                        }
                        extantHeart.SpriteName = this.m_currentEmptyHeartName;
                    }
                }
            }
            if ((double) f1 - (double) index1 > 0.0 && this.extantHearts != null && this.extantHearts.Count > 0)
            {
                dfSprite extantHeart = this.extantHearts[index1];
                if ((bool) (Object) extantHeart)
                {
                    if (extantHeart.SpriteName == this.m_currentFullHeartName)
                    {
                        GameObject gameObject = Object.Instantiate<GameObject>(this.damagedHeartAnimationPrefab.gameObject);
                        gameObject.transform.parent = this.transform.parent;
                        gameObject.layer = this.gameObject.layer;
                        dfSprite component = gameObject.GetComponent<dfSprite>();
                        component.BringToFront();
                        extantHeart.Parent.AddControl((dfControl) component);
                        extantHeart.Parent.BringToFront();
                        component.ZOrder = extantHeart.ZOrder - 1;
                        component.RelativePosition = extantHeart.RelativePosition + this.damagedPrefabOffset;
                    }
                    extantHeart.SpriteName = this.m_currentHalfHeartName;
                }
            }
            this.ProcessHeartSpriteModifications(hh.gameActor as PlayerController);
            if (hh.HasCrest && (double) f2 > 0.0)
            {
                for (int index3 = 0; index3 < this.extantArmors.Count; ++index3)
                {
                    dfSprite extantArmor = this.extantArmors[index3];
                    if ((bool) (Object) extantArmor)
                    {
                        if (index3 < this.extantArmors.Count - 1)
                        {
                            if (extantArmor.SpriteName != this.m_currentArmorName)
                            {
                                extantArmor.SpriteName = this.m_currentArmorName;
                                extantArmor.Color = this.armorSpritePrefab.Color;
                                dfPanel motionGroupParent = GameUIRoot.Instance.GetMotionGroupParent((dfControl) extantArmor);
                                motionGroupParent.Width -= Pixelator.Instance.CurrentTileScale;
                                motionGroupParent.Height -= Pixelator.Instance.CurrentTileScale;
                                extantArmor.RelativePosition = extantArmor.RelativePosition.WithY(0.0f);
                            }
                        }
                        else if (extantArmor.SpriteName != this.crestSpritePrefab.SpriteName)
                        {
                            extantArmor.SpriteName = this.crestSpritePrefab.SpriteName;
                            extantArmor.Color = this.crestSpritePrefab.Color;
                            dfPanel motionGroupParent = GameUIRoot.Instance.GetMotionGroupParent((dfControl) extantArmor);
                            motionGroupParent.Width += Pixelator.Instance.CurrentTileScale;
                            motionGroupParent.Height += Pixelator.Instance.CurrentTileScale;
                            extantArmor.RelativePosition = extantArmor.RelativePosition.WithY(Pixelator.Instance.CurrentTileScale);
                        }
                    }
                }
            }
            else
            {
                for (int index4 = 0; index4 < this.extantArmors.Count; ++index4)
                {
                    dfSprite extantArmor = this.extantArmors[index4];
                    if ((bool) (Object) extantArmor)
                    {
                        if (extantArmor.SpriteName != this.m_currentArmorName)
                        {
                            extantArmor.SpriteName = this.m_currentArmorName;
                            dfPanel motionGroupParent = GameUIRoot.Instance.GetMotionGroupParent((dfControl) extantArmor);
                            motionGroupParent.Width -= Pixelator.Instance.CurrentTileScale;
                            motionGroupParent.Height -= Pixelator.Instance.CurrentTileScale;
                            extantArmor.RelativePosition = extantArmor.RelativePosition.WithY(0.0f);
                            GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor, true, false, true);
                            GameUIRoot.Instance.UpdateControlMotionGroup((dfControl) extantArmor);
                            GameUIRoot.Instance.TransitionTargetMotionGroup((dfControl) extantArmor, GameUIRoot.Instance.IsCoreUIVisible(), false, true);
                        }
                        extantArmor.Color = this.armorSpritePrefab.Color;
                        extantArmor.RelativePosition = extantArmor.RelativePosition.WithY(0.0f);
                    }
                }
            }
            for (int index5 = 0; index5 < this.extantHearts.Count; ++index5)
            {
                dfSprite extantHeart = this.extantHearts[index5];
                if ((bool) (Object) extantHeart)
                    extantHeart.Size = extantHeart.SpriteInfo.sizeInPixels * Pixelator.Instance.CurrentTileScale;
            }
            for (int index6 = 0; index6 < this.extantArmors.Count; ++index6)
            {
                dfSprite extantArmor = this.extantArmors[index6];
                if ((bool) (Object) extantArmor)
                    extantArmor.Size = extantArmor.SpriteInfo.sizeInPixels * Pixelator.Instance.CurrentTileScale;
            }
        }

        private void ProcessHeartSpriteModifications(PlayerController associatedPlayer)
        {
            bool flag = false;
            if ((bool) (Object) associatedPlayer)
            {
                if (associatedPlayer.HealthAndArmorSwapped)
                {
                    this.m_currentFullHeartName = "heart_shield_full_001";
                    this.m_currentHalfHeartName = "heart_shield_half_001";
                    this.m_currentEmptyHeartName = "heart_shield_empty_001";
                    this.m_currentArmorName = "armor_shield_heart_idle_001";
                    flag = true;
                }
                else if ((bool) (Object) associatedPlayer.CurrentGun && associatedPlayer.CurrentGun.IsUndertaleGun)
                {
                    this.m_currentFullHeartName = "heart_full_yellow_001";
                    this.m_currentHalfHeartName = "heart_half_yellow_001";
                    flag = true;
                }
            }
            if (flag)
                return;
            this.m_currentFullHeartName = this.fullHeartSpriteName;
            this.m_currentHalfHeartName = this.halfHeartSpriteName;
            this.m_currentEmptyHeartName = this.emptyHeartSpriteName;
            this.m_currentArmorName = this.armorSpritePrefab.SpriteName;
        }
    }

