using UnityEditor;
using System.Reflection;

public partial class CreateScriptAssetsMenu
{
    private static MethodInfo createScriptMethod = typeof(ProjectWindowUtil)
        .GetMethod("CreateScriptAsset", BindingFlags.Static | BindingFlags.NonPublic);

    static void CreateScriptAsset(string templatePath, string destName)
    {
        createScriptMethod.Invoke(null, new object[] { templatePath, destName });

        //string[] lines = File.ReadAllLines(destName);
        //foreach(var line in lines)
        //{
        //    line.Replace("SystemBehaviourTemplate", destName);
        //}
        //File.WriteAllLines(destName, lines);
    }
    
    [MenuItem("Assets/Create/SystemBehaviour")]
    public static void CreateSystemBehaviour()
    {
        CreateScriptAssetsMenu.CreateScriptAsset("Assets/Plugins/AlphaECS/Unity/ScriptTemplates/SystemBehaviourTemplate.txt", "NewSystemBehaviour.cs");
    }

    [MenuItem("Assets/Create/ComponentBehaviour")]
    public static void CreateComponentBehaviour()
    {
        CreateScriptAssetsMenu.CreateScriptAsset("Assets/Plugins/AlphaECS/Unity/ScriptTemplates/ComponentBehaviourTemplate.txt", "NewComponentBehaviour.cs");
    }
}
