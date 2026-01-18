using UnityEngine;

#nullable disable

public class BulletThatCanKillThePast : PassiveItem
  {
    private void Start()
    {
      this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
      if (!this.m_pickedUp)
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
      Shader.SetGlobalFloat("_MapActive", 0.0f);
    }

    protected override void Update()
    {
      base.Update();
      if (this.m_pickedUp || this.gameObject.layer == LayerMask.NameToLayer("Unpixelated"))
        return;
      this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      SimpleSpriteRotator[] componentsInChildren = this.GetComponentsInChildren<SimpleSpriteRotator>();
      if (componentsInChildren != null)
      {
        for (int index = 0; index < componentsInChildren.Length; ++index)
          Object.Destroy((Object) componentsInChildren[index].gameObject);
      }
      GameManager.Instance.PrimaryPlayer.PastAccessible = true;
      Shader.SetGlobalFloat("_MapActive", 1f);
      base.Pickup(player);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<BulletThatCanKillThePast>().m_pickedUpThisRun = true;
      debrisObject.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
      return debrisObject;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

