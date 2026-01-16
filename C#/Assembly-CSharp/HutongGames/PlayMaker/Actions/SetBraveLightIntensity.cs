// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetBraveLightIntensity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Handles light intensity for Brave lights.")]
[ActionCategory(".Brave")]
public class SetBraveLightIntensity : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Specify lights to control; if empty, this action will affect all lights on its owner.")]
  public ShadowSystem[] specifyLights;
  [HutongGames.PlayMaker.Tooltip("New light intensity after the transition.")]
  public FsmFloat intensity;
  [HutongGames.PlayMaker.Tooltip("Duraiton of the transition.")]
  public FsmFloat transitionTime;
  private float[] m_startIntensity;
  private Color[] m_startColors;
  private Color[] m_endColors;
  private float m_timer;
  private SceneLightManager[] m_lightManagers;
  private Material[] m_materials;

  public bool IsKeptAction { get; set; }

  public override void Reset()
  {
    this.specifyLights = new ShadowSystem[0];
    this.intensity = (FsmFloat) 1f;
    this.transitionTime = (FsmFloat) 0.0f;
  }

  public override void OnEnter()
  {
    if (this.specifyLights.Length == 0)
    {
      this.specifyLights = this.Owner.gameObject.GetComponentsInChildren<ShadowSystem>();
      if (this.specifyLights.Length == 0)
      {
        this.Finish();
        return;
      }
    }
    this.m_lightManagers = new SceneLightManager[this.specifyLights.Length];
    for (int index = 0; index < this.specifyLights.Length; ++index)
      this.m_lightManagers[index] = this.specifyLights[index].GetComponent<SceneLightManager>();
    this.m_materials = new Material[this.specifyLights.Length];
    for (int index = 0; index < this.specifyLights.Length; ++index)
      this.m_materials[index] = this.specifyLights[index].GetComponent<Renderer>().material;
    if ((double) this.transitionTime.Value <= 0.0)
    {
      for (int index = 0; index < this.specifyLights.Length; ++index)
      {
        this.specifyLights[index].uLightIntensity = this.intensity.Value;
        if ((bool) (Object) this.m_lightManagers[index])
        {
          Color validColor = this.m_lightManagers[index].validColors[Random.Range(0, this.m_lightManagers[index].validColors.Length)];
          this.m_materials[index].SetColor("_TintColor", validColor);
        }
        else
          this.m_materials[index].SetColor("_TintColor", Color.white);
      }
      this.Finish();
    }
    else
    {
      this.m_timer = 0.0f;
      this.m_startIntensity = (float[]) null;
      this.m_startColors = (Color[]) null;
      this.m_endColors = (Color[]) null;
    }
  }

  public override void OnUpdate()
  {
    if (this.m_startIntensity == null)
    {
      this.m_startIntensity = new float[this.specifyLights.Length];
      this.m_startColors = new Color[this.specifyLights.Length];
      this.m_endColors = new Color[this.specifyLights.Length];
      for (int index = 0; index < this.specifyLights.Length; ++index)
      {
        this.m_startIntensity[index] = this.specifyLights[index].uLightIntensity;
        this.m_startColors[index] = this.m_materials[index].GetColor("_TintColor");
        this.m_endColors[index] = (double) this.intensity.Value > 0.0 ? (!(bool) (Object) this.m_lightManagers[index] ? Color.white : this.m_lightManagers[index].validColors[Random.Range(0, this.m_lightManagers[index].validColors.Length)]) : new Color(0.5f, 0.5f, 0.5f, 1f);
      }
      this.m_timer = 0.0f;
    }
    else
    {
      this.m_timer += BraveTime.DeltaTime;
      for (int index = 0; index < this.specifyLights.Length; ++index)
      {
        this.specifyLights[index].uLightIntensity = Mathf.Lerp(this.m_startIntensity[index], this.intensity.Value, this.m_timer / this.transitionTime.Value);
        this.m_materials[index].SetColor("_TintColor", Color.Lerp(this.m_startColors[index], this.m_endColors[index], this.m_timer / this.transitionTime.Value));
      }
      if ((double) this.m_timer < (double) this.transitionTime.Value)
        return;
      this.Finish();
    }
  }

  public override void OnExit()
  {
    for (int index = 0; index < this.specifyLights.Length; ++index)
    {
      this.specifyLights[index].uLightIntensity = this.intensity.Value;
      this.m_materials[index].SetColor("_TintColor", this.m_endColors[index]);
    }
  }

  public new void Finish()
  {
    if (!this.IsKeptAction)
      base.Finish();
    else
      this.Finished = true;
  }
}
