#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LanguageManagerImportExport))]
public class LanguageManagerImportExportEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Export"))
        {
            (target as LanguageManagerImportExport)?.OnClickExport();
        }

        if (GUILayout.Button("Import"))
        {
            (target as LanguageManagerImportExport)?.OnClickImport();
        }

        if (GUILayout.Button("Import From File..."))
        {
            (target as LanguageManagerImportExport)?.OnClickImportFromFile();
        }
        if (GUILayout.Button("Export To File..."))
        {
            (target as LanguageManagerImportExport)?.OnClickExportToFile();
            
        }
    }
}
#endif