using System;
using System.Reflection;
using UnityEngine;

#nullable disable

public class dfClipboardHelper
  {
    private static PropertyInfo m_systemCopyBufferProperty;

    private static PropertyInfo GetSystemCopyBufferProperty()
    {
      if (dfClipboardHelper.m_systemCopyBufferProperty == null)
      {
        dfClipboardHelper.m_systemCopyBufferProperty = typeof (GUIUtility).GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
        if (dfClipboardHelper.m_systemCopyBufferProperty == null)
          throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
      }
      return dfClipboardHelper.m_systemCopyBufferProperty;
    }

    public static string clipBoard
    {
      get
      {
        try
        {
          return (string) dfClipboardHelper.GetSystemCopyBufferProperty().GetValue((object) null, (object[]) null);
        }
        catch
        {
          return string.Empty;
        }
      }
      set
      {
        try
        {
          dfClipboardHelper.GetSystemCopyBufferProperty().SetValue((object) null, (object) value, (object[]) null);
        }
        catch
        {
        }
      }
    }
  }

