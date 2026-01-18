// Decompiled with JetBrains decompiler
// Type: GalaxyManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Galaxy.Api;
using System;
using UnityEngine;

#nullable disable

[DisallowMultipleComponent]
public class GalaxyManager : MonoBehaviour
  {
    private static GalaxyManager s_instance;
    private static bool s_EverInialized;
    private bool m_bInitialized;
    private GlobalAuthListener m_authListener;

    private static GalaxyManager Instance
    {
      get
      {
        return GalaxyManager.s_instance ?? new GameObject(nameof (GalaxyManager)).AddComponent<GalaxyManager>();
      }
    }

    public static bool Initialized
    {
      get => GalaxyManager.Instance.m_bInitialized;
      private set
      {
        GalaxyManager.Instance.m_bInitialized = value;
        if (!value)
          return;
        GalaxyManager.s_EverInialized = true;
      }
    }

    private void Awake()
    {
      if ((UnityEngine.Object) GalaxyManager.s_instance != (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      else
      {
        GalaxyManager.s_instance = this;
        if (GalaxyManager.s_EverInialized)
          throw new Exception("Tried to Initialize the Galaxy API twice in one session!");
        UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
        try
        {
          GalaxyInstance.Init("48944359584830756", "3847f0113681121feddcd75acdcfcde13320be288b24f33b003821c9e776737d");
          this.m_authListener = (GlobalAuthListener) new GalaxyManager.AuthListener();
          GalaxyInstance.User().SignIn();
        }
        catch (Exception ex1)
        {
          Debug.LogError((object) ex1);
          try
          {
            Debug.LogError((object) "GalaxyManager failed to start; attempting shut down.");
            GalaxyInstance.Shutdown();
          }
          catch (Exception ex2)
          {
            Debug.LogError((object) ex1);
          }
        }
      }
    }

    private void OnEnable()
    {
      if ((UnityEngine.Object) GalaxyManager.s_instance == (UnityEngine.Object) null)
        GalaxyManager.s_instance = this;
      if (this.m_bInitialized)
        ;
    }

    private void OnDestroy()
    {
      if ((UnityEngine.Object) GalaxyManager.s_instance != (UnityEngine.Object) this)
        return;
      GalaxyManager.s_instance = (GalaxyManager) null;
      if (this.m_bInitialized)
        ;
      GalaxyInstance.Shutdown();
    }

    private void Update()
    {
      GalaxyInstance.ProcessData();
      if (this.m_bInitialized)
        ;
    }

    public class AuthListener : GlobalAuthListener
    {
      public override void OnAuthSuccess()
      {
        Debug.Log((object) "Auth success!");
        GalaxyManager.Initialized = true;
      }

      public override void OnAuthFailure(IAuthListener.FailureReason failureReason)
      {
        Debug.LogFormat("Auth failed! {0}", (object) failureReason);
      }

      public override void OnAuthLost() => Debug.LogFormat("Auth lost!");
    }
  }

