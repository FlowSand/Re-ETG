// Decompiled with JetBrains decompiler
// Type: DoNothingActiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class DoNothingActiveItem : PlayerItem
  {
    public override bool CanBeUsed(PlayerController user) => base.CanBeUsed(user);

    protected override void DoEffect(PlayerController user)
    {
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

