using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MultiplayerARPG;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LanguageManager))]
public class LanguageManagerImportExport : MonoBehaviour
{
#if UNITY_EDITOR
    private const char NEW_LINE = '\n';
    private const string QUOTE = "\"";
    private const string COMMA = ",";
    private const char XL_EQUALS = '=';
    private const char XL_ESCAPE = '\'';
    private const char XL_PLUS = '+';
    private const char XL_MINUS = '-';
    private const string FILE_PREFIX = "language_";

    public string languageKey;
    [TextArea(10, 10)] public string text;

    public bool useJsonNotCsv = false;
    public bool escapeExcelFormulas = true;

    private LanguageManager languageManager;

    private LanguageManager LanguageManager
    {
        get
        {
            if (languageManager == null)
                languageManager = GetComponent<LanguageManager>();
            return languageManager;
        }
    }

    public void OnClickExport()
    {
        text = LanguageToString();
    }

    public void OnClickImport()
    {
        LanguageManager.languageList.Add(LanguageFromString(text));
    }

    public void OnClickExportToFile()
    {
        string path;
        if (useJsonNotCsv)
            path = EditorUtility.SaveFilePanelInProject("Exporting JSON", FILE_PREFIX + languageKey, "json", "Export...");
        else
            path = EditorUtility.SaveFilePanelInProject("Exporting CSV", FILE_PREFIX + languageKey, "csv", "Export...");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, LanguageToString());
            AssetDatabase.Refresh();
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            EditorGUIUtility.PingObject(asset);
            if (EditorUtility.DisplayDialog("View File?", "Do you want to open your exported file " + path + " in an external application?", "Yes", "No"))
                AssetDatabase.OpenAsset(asset);
        }
    }

    public void OnClickImportFromFile()
    {
        string path;
        if (useJsonNotCsv)
            path = EditorUtility.OpenFilePanelWithFilters("Importing JSON", "Assets/", new [] {"JSON", "json"});
        else
            path = EditorUtility.OpenFilePanelWithFilters("Importing CSV", "Assets/", new [] {"CSV", "csv"});
        if (!string.IsNullOrEmpty(path))
        {
            // string contents = File.ReadAllText(path);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); 
            using (var sr = new StreamReader(fs))
            {
                string contents = sr.ReadToEnd();
                LanguageManager.languageList.Add(LanguageFromString(contents));
                sr.Close();
            }
        }
    }

    private string LanguageToString()
    {
        var language = LanguageManager.languageList.Find(x => x.languageKey.Equals(languageKey));
        if (language == null)
        {
            Debug.LogWarning("No Language found with key:" + languageKey);
        }

        if (useJsonNotCsv)
        {
            return JsonUtility.ToJson(language, true);
        }
        else
        {
            return LanguageToCsv(language, escapeExcelFormulas);
        }
    }

    private Language LanguageFromString(string data)
    {
        if (useJsonNotCsv)
        {
            return JsonUtility.FromJson<Language>(data);
        }
        else
        {
            return LanguageFromCsv(data, languageKey, escapeExcelFormulas);
        }
    }

    private static string LanguageToCsv(Language language, bool escapeExcelFormulas)
    {
        var builder = new StringBuilder();

        foreach (var data in language.dataList)
        {
            string key = data.key.Replace(QUOTE, QUOTE+QUOTE);
            string value = data.value.Replace(QUOTE, QUOTE+QUOTE);
            if (escapeExcelFormulas)
            {
                key = EscapeExcelFormulas(key);
                value = EscapeExcelFormulas(value);
            }

            builder.Append(QUOTE).Append(key).Append(QUOTE).Append(COMMA).Append(QUOTE).Append(value)
                .Append(QUOTE).Append(NEW_LINE);
        }

        return builder.ToString();
    }

    private static Language LanguageFromCsv(string data, string langKey, bool escapeExcelFormulas)
    {
        var language = new Language { languageKey = langKey, dataList = new List<LanguageData>() };
        var reader = new CsvReader(data);
        while (true)
        {
            if (!reader.CanRead())
                break;
            var tempKey = reader.Read();
            if (!reader.CanRead())
                break;
            var tempValue = reader.Read();
            if (escapeExcelFormulas)
            {
                tempKey = UnescapeExcelFormulas(tempKey);
                tempValue = UnescapeExcelFormulas(tempValue);
            }
            var tempData = new LanguageData { key = tempKey, value = tempValue };
            language.dataList.Add(tempData);
        }
        return language;
    }

    private static string EscapeExcelFormulas(string str)
    {
        char first = str[0];
        if (first == XL_EQUALS || first == XL_PLUS || first == XL_MINUS)
        {
            return XL_ESCAPE + str;
        }
        return str;
    }

    private static string UnescapeExcelFormulas(string str)
    {
        char first = str[0];
        char second = str[1];
        if (first == XL_ESCAPE && (second == XL_EQUALS || second == XL_PLUS || second == XL_MINUS))
        {
            return str.Substring(1);
        }
        return str;
    }
#endif
}