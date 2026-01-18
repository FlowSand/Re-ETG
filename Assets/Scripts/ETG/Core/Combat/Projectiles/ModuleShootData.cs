// Decompiled with JetBrains decompiler
// Type: ModuleShootData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class ModuleShootData
  {
    public bool onCooldown;
    public bool needsReload;
    public int numberShotsFired;
    public int numberShotsFiredThisBurst;
    public int numberShotsActiveReload;
    public float chargeTime;
    public bool chargeFired;
    public ProjectileModule.ChargeProjectile lastChargeProjectile;
    public float activeReloadDamageModifier = 1f;
    public float alternateAngleSign = 1f;
    public BeamController beam;
    public int beamKnockbackID;
    public float angleForShot;
  }

