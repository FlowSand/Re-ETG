using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class NebulaRegisterer : MonoBehaviour
  {
    private Renderer m_renderer;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new NebulaRegisterer__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void Update()
    {
      if (!(bool) (Object) this.m_renderer)
        return;
      if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW)
      {
        this.m_renderer.enabled = false;
      }
      else
      {
        if (this.m_renderer.enabled)
          return;
        this.m_renderer.enabled = true;
      }
    }
  }

