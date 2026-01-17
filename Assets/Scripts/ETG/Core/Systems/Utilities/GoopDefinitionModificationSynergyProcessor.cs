// Decompiled with JetBrains decompiler
// Type: GoopDefinitionModificationSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class GoopDefinitionModificationSynergyProcessor : MonoBehaviour
    {
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public bool MakesGoopIgnitable;
      public bool ChangesGoopDefinition;
      public GoopDefinition ChangedDefinition;
      private BasicBeamController m_beam;
      private GoopModifier m_gooper;
      private static Dictionary<GoopDefinition, GoopDefinition> m_modifiedGoops = new Dictionary<GoopDefinition, GoopDefinition>();

      public void Awake()
      {
        this.m_gooper = this.GetComponent<GoopModifier>();
        int count = -1;
        if (!PlayerController.AnyoneHasActiveBonusSynergy(this.RequiredSynergy, out count))
          return;
        if (this.MakesGoopIgnitable && (bool) (Object) this.m_gooper)
        {
          if (!GoopDefinitionModificationSynergyProcessor.m_modifiedGoops.ContainsKey(this.m_gooper.goopDefinition))
          {
            GoopDefinition goopDefinition = Object.Instantiate<GoopDefinition>(this.m_gooper.goopDefinition);
            goopDefinition.CanBeIgnited = true;
            GoopDefinitionModificationSynergyProcessor.m_modifiedGoops.Add(this.m_gooper.goopDefinition, goopDefinition);
          }
          this.m_gooper.goopDefinition = GoopDefinitionModificationSynergyProcessor.m_modifiedGoops[this.m_gooper.goopDefinition];
        }
        if (!this.ChangesGoopDefinition || !(bool) (Object) this.m_gooper)
          return;
        this.m_gooper.goopDefinition = this.ChangedDefinition;
      }
    }

}
