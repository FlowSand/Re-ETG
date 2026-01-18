using FullInspector.Internal;
using System;
using System.IO;
using UnityEngine;

#nullable disable
namespace FullInspector
{
  public class fiSettings
  {
    public static bool PrettyPrintSerializedJson;
    public static CommentType DefaultCommentType = CommentType.Info;
    public static bool ForceDisplayInlineObjectEditor;
    public static bool EnableAnimation = true;
    public static bool ForceSaveAllAssetsOnSceneSave;
    public static bool ForceSaveAllAssetsOnRecompilation;
    public static bool ForceRestoreAllAssetsOnRecompilation;
    public static bool AutomaticReferenceInstantation;
    public static bool InspectorAutomaticReferenceInstantation = true;
    public static bool InspectorRequireShowInInspector;
    public static bool SerializeAutoProperties = true;
    public static bool EmitWarnings;
    public static bool EmitGraphMetadataCulls;
    public static float MinimumFoldoutHeight = 80f;
    public static bool EnableOpenScriptButton = true;
    public static bool ForceDisableMultithreadedSerialization;
    public static float LabelWidthPercentage = 0.45f;
    public static float LabelWidthOffset = 30f;
    public static float LabelWidthMax = 600f;
    public static float LabelWidthMin;
    public static int DefaultPageMinimumCollectionLength = 20;
    public static string RootDirectory = "Assets/FullInspector2/";
    public static string RootGeneratedDirectory;

    static fiSettings()
    {
      foreach (fiSettingsProcessor assemblyInstance in fiRuntimeReflectionUtility.GetAssemblyInstances<fiSettingsProcessor>())
        assemblyInstance.Process();
      if (fiUtility.IsEditor)
        fiSettings.EnsureRootDirectory();
      if (fiSettings.RootGeneratedDirectory == null)
        fiSettings.RootGeneratedDirectory = fiSettings.RootDirectory.TrimEnd('/') + "_Generated/";
      if (!fiUtility.IsEditor || fiDirectory.Exists(fiSettings.RootGeneratedDirectory))
        return;
      Debug.Log((object) ("Creating directory at " + fiSettings.RootGeneratedDirectory));
      fiDirectory.CreateDirectory(fiSettings.RootGeneratedDirectory);
    }

    private static void EnsureRootDirectory()
    {
      if (fiSettings.RootDirectory != null && fiDirectory.Exists(fiSettings.RootDirectory))
        return;
      Debug.Log((object) $"Failed to find FullInspector root directory at \"{fiSettings.RootDirectory}\"; running scan to find it.");
      string directoryPathByName = fiSettings.FindDirectoryPathByName("Assets", "FullInspector2");
      if (directoryPathByName == null)
      {
        Debug.LogError((object) "Unable to locate \"FullInspector2\" directory. Please make sure that Full Inspector is located within \"FullInspector2\"");
      }
      else
      {
        string path = directoryPathByName.Replace('\\', '/').TrimEnd('/') + (object) '/';
        fiSettings.RootDirectory = path;
        Debug.Log((object) $"Found FullInspector at \"{path}\". Please add the following code to your project in a non-Editor folder:\n\n{fiSettings.FormatCustomizerForNewPath(path)}");
      }
    }

    private static string FormatCustomizerForNewPath(string path)
    {
      return $"using FullInspector;{Environment.NewLine}{Environment.NewLine}public class UpdateFullInspectorRootDirectory : fiSettingsProcessor {{{Environment.NewLine}    public void Process() {{{Environment.NewLine}        fiSettings.RootDirectory = \"{path}\";{Environment.NewLine}    }}{Environment.NewLine}}}{Environment.NewLine}";
    }

    private static string FindDirectoryPathByName(string currentDirectory, string targetDirectory)
    {
      targetDirectory = Path.GetFileName(targetDirectory);
      foreach (string directory in fiDirectory.GetDirectories(currentDirectory))
      {
        if (Path.GetFileName(directory) == targetDirectory)
          return directory;
        string directoryPathByName = fiSettings.FindDirectoryPathByName(directory, targetDirectory);
        if (directoryPathByName != null)
          return directoryPathByName;
      }
      return (string) null;
    }
  }
}
