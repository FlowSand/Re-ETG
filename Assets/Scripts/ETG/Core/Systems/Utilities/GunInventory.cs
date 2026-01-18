using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class GunInventory
    {
        public bool GunChangeForgiveness;
        public List<GunClass> m_gunClassOverrides = new List<GunClass>();
        private GameActor m_owner;
        private Gun m_currentGun;
        private OverridableBool m_dualWielding = new OverridableBool(false);
        private Gun m_currentSecondaryGun;
        private int m_maxGuns = -1;
        public OverridableBool GunLocked = new OverridableBool(false);
        private List<Gun> m_guns;
        private Dictionary<Gun, float> m_gunStowedTime;
        private List<float> m_perGunDrainData;

        public GunInventory(GameActor owner)
        {
            this.m_owner = owner;
            this.m_guns = new List<Gun>();
            this.m_perGunDrainData = new List<float>();
            this.m_gunStowedTime = new Dictionary<Gun, float>();
        }

        public Gun CurrentGun => this.ForceNoGun ? (Gun) null : this.m_currentGun;

        public Gun CurrentSecondaryGun
        {
            get
            {
                if (!this.DualWielding)
                    return (Gun) null;
                return this.ForceNoGun ? (Gun) null : this.m_currentSecondaryGun;
            }
        }

        public event GunInventory.OnGunChangedEvent OnGunChanged;

        public GameActor Owner => this.m_owner;

        public void SetDualWielding(bool value, string reason)
        {
            bool flag = this.m_dualWielding.Value;
            Gun currentSecondaryGun = !flag ? (Gun) null : this.CurrentSecondaryGun;
            this.m_dualWielding.SetOverride(reason, value);
            if (!flag || this.m_dualWielding.Value || !(bool) (Object) currentSecondaryGun)
                return;
            if (!currentSecondaryGun.IsPreppedForThrow)
                currentSecondaryGun.CeaseAttack(false);
            currentSecondaryGun.OnPrePlayerChange();
            currentSecondaryGun.gameObject.SetActive(false);
        }

        public bool DualWielding => this.m_dualWielding.Value;

        public List<Gun> AllGuns => this.m_guns;

        public int GunCountModified
        {
            get
            {
                int gunCountModified = 0;
                for (int index = 0; index < this.m_guns.Count; ++index)
                {
                    if (!this.m_guns[index].name.StartsWith("ArtfulDodger"))
                        ++gunCountModified;
                }
                return gunCountModified;
            }
        }

        public int maxGuns
        {
            get => this.m_maxGuns;
            set => this.m_maxGuns = value;
        }

        public bool ForceNoGun { get; set; }

        public bool ContainsGun(int gunID)
        {
            for (int index = 0; index < this.m_guns.Count; ++index)
            {
                if (this.m_guns[index].PickupObjectId == gunID)
                    return true;
            }
            return false;
        }

        public int ContainsGunOfClass(GunClass targetClass, bool respectsOverrides)
        {
            int num = 0;
            if (respectsOverrides && this.m_gunClassOverrides.Contains(targetClass))
                return 0;
            for (int index = 0; index < this.m_guns.Count; ++index)
            {
                if (this.m_guns[index].gunClass == targetClass)
                    ++num;
            }
            return num;
        }

        public void RegisterGunClassOverride(GunClass overridden)
        {
            if (this.m_gunClassOverrides.Contains(overridden))
                return;
            this.m_gunClassOverrides.Add(overridden);
        }

        public void DeregisterGunClassOverride(GunClass overridden)
        {
            this.m_gunClassOverrides.Remove(overridden);
        }

        public void HandleAmmoDrain(float percentAmmoDrain)
        {
            for (int index = 0; index < this.m_guns.Count; ++index)
            {
                if (this.m_guns[index].AdjustedMaxAmmo > 0 && this.m_guns[index].ammo > 0)
                {
                    this.m_perGunDrainData[index] += percentAmmoDrain;
                    int amt = Mathf.FloorToInt((float) this.m_guns[index].AdjustedMaxAmmo * this.m_perGunDrainData[index]);
                    if (amt >= 1)
                    {
                        float num = (float) amt / (float) this.m_guns[index].AdjustedMaxAmmo;
                        this.m_perGunDrainData[index] -= num;
                        this.m_guns[index].LoseAmmo(amt);
                    }
                }
            }
        }

        public void ClearAmmoDrain()
        {
            for (int index = 0; index < this.m_guns.Count; ++index)
                this.m_perGunDrainData[index] = 0.0f;
        }

        public void FrameUpdate()
        {
            for (int index = 0; index < this.AllGuns.Count; ++index)
            {
                if ((Object) this.AllGuns[index] == (Object) this.CurrentGun)
                {
                    this.m_gunStowedTime[this.AllGuns[index]] = 0.0f;
                }
                else
                {
                    Dictionary<Gun, float> gunStowedTime;
                    Gun allGun;
                    (gunStowedTime = this.m_gunStowedTime)[allGun = this.AllGuns[index]] = gunStowedTime[allGun] + BraveTime.DeltaTime;
                    if ((double) this.m_gunStowedTime[this.AllGuns[index]] > 2.0 * (double) this.AllGuns[index].reloadTime)
                    {
                        this.AllGuns[index].ForceImmediateReload();
                        this.m_gunStowedTime[this.AllGuns[index]] = -1000f;
                    }
                }
            }
        }

        public Gun AddGunToInventory(Gun gun, bool makeActive = false)
        {
            if ((bool) (Object) gun && gun.ShouldBeDestroyedOnExistence(!(this.m_owner is PlayerController)))
                return (Gun) null;
            Gun ownedCopy = this.GetOwnedCopy(gun);
            if ((Object) ownedCopy != (Object) null)
            {
                ownedCopy.GainAmmo(gun);
                return ownedCopy;
            }
            if (!gun.name.StartsWith("ArtfulDodger") && this.maxGuns > 0 && this.GunCountModified >= this.maxGuns)
            {
                if (!(this.m_owner is PlayerController))
                    return (Gun) null;
                Gun currentGun = this.m_owner.CurrentGun;
                this.RemoveGunFromInventory(currentGun);
                currentGun.DropGun();
            }
            Gun gunForAdd = this.CreateGunForAdd(gun);
            gunForAdd.HasBeenPickedUp = true;
            gunForAdd.HasProcessedStatMods = gun.HasProcessedStatMods;
            gunForAdd.CopyStateFrom(gun);
            this.m_guns.Add(gunForAdd);
            this.m_perGunDrainData.Add(0.0f);
            this.m_gunStowedTime.Add(gunForAdd, 0.0f);
            if (this.m_guns.Count == 1)
            {
                this.m_currentGun = this.m_guns[0];
                this.ChangeGun(0, true);
            }
            if (makeActive)
            {
                this.ChangeGun(this.m_guns.Count - 1 - this.m_guns.IndexOf(this.m_currentGun), true);
                gunForAdd.HandleSpriteFlip(this.m_owner.SpriteFlipped);
            }
            if (this.m_owner is PlayerController)
                (this.m_owner as PlayerController).stats.RecalculateStats(this.m_owner as PlayerController);
            return gunForAdd;
        }

        public Gun GetTargetGunWithChange(int amt)
        {
            if (this.m_guns.Count == 0)
                return (Gun) null;
            int num = this.m_guns.IndexOf(this.m_currentGun) + amt;
            while (num < 0)
                num += this.m_guns.Count;
            return this.m_guns[num % this.m_guns.Count];
        }

        public void SwapDualGuns()
        {
            if (!this.DualWielding || !(bool) (Object) this.m_currentSecondaryGun || !(bool) (Object) this.m_currentGun)
                return;
            Gun currentGun = this.m_currentGun;
            Gun currentSecondaryGun = this.m_currentSecondaryGun;
            this.m_currentGun = this.m_currentSecondaryGun;
            this.m_currentSecondaryGun = currentGun;
            this.m_currentGun.OnEnable();
            this.m_currentSecondaryGun.OnEnable();
            this.m_currentGun.HandleSpriteFlip(this.m_currentGun.CurrentOwner.SpriteFlipped);
            this.m_currentSecondaryGun.HandleSpriteFlip(this.m_currentSecondaryGun.CurrentOwner.SpriteFlipped);
            if (this.OnGunChanged == null)
                return;
            this.OnGunChanged(currentGun, this.m_currentGun, currentSecondaryGun, this.CurrentSecondaryGun, false);
        }

        public void ChangeGun(int amt, bool newGun = false, bool overrideGunLock = false)
        {
            if (this.m_guns.Count == 0 || (Object) this.m_currentGun != (Object) null && this.m_currentGun.UnswitchableGun || this.GunLocked.Value && !overrideGunLock)
                return;
            Gun currentGun = this.m_currentGun;
            Gun currentSecondaryGun = this.m_currentSecondaryGun;
            if ((Object) this.m_currentGun != (Object) null && !this.ForceNoGun)
            {
                if (!this.m_currentGun.IsPreppedForThrow)
                    this.CurrentGun.CeaseAttack(false);
                this.m_currentGun.OnPrePlayerChange();
                this.m_currentGun.gameObject.SetActive(false);
            }
            if (this.DualWielding && (bool) (Object) this.CurrentSecondaryGun)
            {
                if (!this.CurrentSecondaryGun.IsPreppedForThrow)
                    this.CurrentSecondaryGun.CeaseAttack(false);
                this.CurrentSecondaryGun.OnPrePlayerChange();
                this.CurrentSecondaryGun.gameObject.SetActive(false);
            }
            int num = this.m_guns.IndexOf(this.m_currentGun) + amt;
            while (num < 0)
                num += this.m_guns.Count;
            int index = num % this.m_guns.Count;
            this.m_currentGun = this.m_guns[index];
            this.m_currentGun.gameObject.SetActive(true);
            if (this.DualWielding)
            {
                if (this.m_guns.Count <= 1)
                    this.m_currentSecondaryGun = (Gun) null;
                if (((Object) this.m_currentSecondaryGun == (Object) null || (Object) this.m_currentSecondaryGun == (Object) this.m_currentGun) && this.m_guns.Count > 1)
                    this.m_currentSecondaryGun = this.m_guns[(index + 1) % this.m_guns.Count];
                if ((bool) (Object) this.CurrentSecondaryGun)
                    this.CurrentSecondaryGun.gameObject.SetActive(true);
            }
            if (this.OnGunChanged == null)
                return;
            this.OnGunChanged(currentGun, this.m_currentGun, currentSecondaryGun, this.CurrentSecondaryGun, newGun);
        }

        public Gun CreateGunForAdd(Gun gunPrototype)
        {
            GameObject gObj = Object.Instantiate<GameObject>(gunPrototype.gameObject);
            gObj.name = gunPrototype.name;
            Gun component = gObj.GetComponent<Gun>();
            if (!component.enabled)
                component.enabled = true;
            component.prefabName = !(gunPrototype.prefabName == string.Empty) ? gunPrototype.prefabName : gunPrototype.name;
            Transform gunPivot = this.m_owner.GunPivot;
            IGunInheritable[] interfaces = gObj.GetInterfaces<IGunInheritable>();
            if (interfaces != null)
            {
                for (int index = 0; index < interfaces.Length; ++index)
                    interfaces[index].InheritData(gunPrototype);
            }
            gObj.transform.parent = gunPivot;
            if ((Object) component.PrimaryHandAttachPoint != (Object) null)
                gObj.transform.localPosition = -component.PrimaryHandAttachPoint.localPosition;
            gObj.SetActive(false);
            component.Initialize(this.m_owner);
            if (!gunPrototype.HasBeenPickedUp && gunPrototype.ArmorToGainOnPickup > 0)
                this.m_owner.healthHaver.Armor += (float) gunPrototype.ArmorToGainOnPickup;
            if (!gunPrototype.HasBeenPickedUp && !component.InfiniteAmmo)
            {
                int num = Mathf.CeilToInt((float) component.AdjustedMaxAmmo / (float) component.GetBaseMaxAmmo() * (float) component.ammo);
                if (num > component.ammo)
                    component.GainAmmo(num - component.ammo);
                else if (num < component.ammo)
                    component.LoseAmmo(component.ammo - num);
            }
            if ((bool) (Object) component && component.DefaultModule != null && (bool) (Object) this.m_owner && this.m_owner is AIActor && component.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                component.DefaultModule.projectiles = new List<Projectile>();
                component.DefaultModule.projectiles.Add(component.DefaultModule.GetChargeProjectile(1000f).Projectile);
                component.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            }
            return component;
        }

        public void DestroyGun(Gun g)
        {
            this.RemoveGunFromInventory(g);
            Object.Destroy((Object) g.gameObject);
        }

        public void DestroyCurrentGun()
        {
            Gun currentGun = this.m_currentGun;
            if (!((Object) currentGun != (Object) null))
                return;
            this.RemoveGunFromInventory(currentGun);
            Object.Destroy((Object) currentGun.gameObject);
        }

        public void DestroyAllGuns()
        {
            for (int index = 0; index < this.m_guns.Count; index = index - 1 + 1)
            {
                Gun gun = this.m_guns[index];
                this.RemoveGunFromInventory(gun);
                Object.Destroy((Object) gun.gameObject);
            }
            this.GunLocked.ClearOverrides();
        }

        public void RemoveGunFromInventory(Gun gun)
        {
            Gun ownedCopy = this.GetOwnedCopy(gun);
            if ((Object) ownedCopy == (Object) null)
            {
                Debug.Log((object) $"Removing unknown gun {gun.gunName} from player inventory!");
            }
            else
            {
                bool flag1 = ((Object) ownedCopy == (Object) this.CurrentGun || (Object) ownedCopy == (Object) this.CurrentSecondaryGun) && this.DualWielding;
                bool flag2 = flag1 && (Object) ownedCopy == (Object) this.CurrentGun;
                int index = this.m_guns.IndexOf(ownedCopy);
                int num = this.m_guns.IndexOf(this.m_currentGun);
                if (flag1)
                {
                    if (flag2)
                    {
                        this.m_currentGun = this.m_currentSecondaryGun;
                        this.m_currentSecondaryGun = (Gun) null;
                        this.m_currentGun.OnEnable();
                        this.m_dualWielding.ClearOverrides();
                        this.ChangeGun(0);
                    }
                }
                else if (index == num && this.m_guns.Count > 1)
                    this.ChangeGun(-1, overrideGunLock: true);
                else if (index == num)
                    this.m_currentGun = (Gun) null;
                this.m_guns.RemoveAt(index);
                this.m_perGunDrainData.RemoveAt(index);
                this.m_gunStowedTime.Remove(ownedCopy);
                if (!(this.m_owner is PlayerController))
                    return;
                (this.m_owner as PlayerController).stats.RecalculateStats(this.m_owner as PlayerController);
            }
        }

        private Gun GetOwnedCopy(Gun w)
        {
            Gun ownedCopy = (Gun) null;
            for (int index = 0; index < this.m_guns.Count; ++index)
            {
                if (this.m_guns[index].PickupObjectId == w.PickupObjectId)
                {
                    ownedCopy = this.m_guns[index];
                    break;
                }
            }
            return ownedCopy;
        }

        public delegate void OnGunChangedEvent(
            Gun previous,
            Gun current,
            Gun previousSecondary,
            Gun currentSecondary,
            bool newGun);
    }

