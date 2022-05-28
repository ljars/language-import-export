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
    private const string ESCAPED_NEW_LINE = @"\n";
    private const char QUOTE = '\"';
    private const string COMMA = ",";
    private const char XL_EQUALS = '=';
    private const char XL_ESCAPE = '\'';
    private const char XL_PLUS = '+';
    private const char XL_MINUS = '-';
    
    public string languageKey;
    [TextArea(10, 10)] public string text;

    [InspectorButton(nameof(OnClickExportLanguage))]
    public bool export;

    [InspectorButton(nameof(OnClickExportToFile))]
    public bool exportToFile;

    [InspectorButton(nameof(OnClickImportLanguage))]
    public bool import;

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

    public void OnClickExportLanguage()
    {
        text = LanguageToString();
    }

    public void OnClickImportLanguage()
    {
        LanguageManager.languageList.Add(LanguageFromString());
    }

    public void OnClickExportToFile()
    {
        string path = "Assets/language_" + languageKey + (useJsonNotCsv ? ".json" : ".csv");
        try
        {
            File.WriteAllText(path, LanguageToString());
            AssetDatabase.Refresh();
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            EditorGUIUtility.PingObject(asset);
            if (EditorUtility.DisplayDialog("Open File?", "Do you want to open " + path + "?", "Yes", "Cancel"))
                AssetDatabase.OpenAsset(asset);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
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

    private Language LanguageFromString()
    {
        if (useJsonNotCsv)
        {
            return JsonUtility.FromJson<Language>(text);
        }
        else
        {
            return LanguageFromCsv(text, languageKey, escapeExcelFormulas);
        }
    }

    private static string LanguageToCsv(Language language, bool escapeExcelFormulas)
    {
        var builder = new StringBuilder();

        foreach (var data in language.dataList)
        {
            builder.Append(QUOTE);
            string key = data.key;
            if (escapeExcelFormulas)
                key = EscapeExcelFormulas(key);
            builder.Append(key);
            builder.Append(QUOTE);
            builder.Append(COMMA);
            builder.Append(QUOTE);
            string value = data.value.Replace(NEW_LINE.ToString(), ESCAPED_NEW_LINE);
            if (escapeExcelFormulas)
                value = EscapeExcelFormulas(value);
            builder.Append(value);
            builder.Append(QUOTE);
            builder.Append(NEW_LINE);
        }

        return builder.ToString();
    }

    private static Language LanguageFromCsv(string data, string langKey, bool escapeExcelFormulas)
    {
        var language = new Language { languageKey = langKey, dataList = new List<LanguageData>() };
        var splitData = data.Split(NEW_LINE);
        foreach (var row in splitData)
        {
            ReadKeyValuePair(row, out var key, out var value);
            if (string.IsNullOrEmpty(key))
                continue;
            var tempData = new LanguageData();
            if (escapeExcelFormulas)
                key = UnescapeExcelFormulas(key);
            tempData.key = key;
            if (escapeExcelFormulas)
                value = UnescapeExcelFormulas(value);
            tempData.value = value.Replace(ESCAPED_NEW_LINE, NEW_LINE.ToString());
            language.dataList.Add(tempData);
        }

        return language;
    }

    private static void ReadKeyValuePair(string row, out string key, out string value)
    {
        int i = 0;
        var keyBuilder = new StringBuilder();
        var valueBuilder = new StringBuilder();

        ConsumeUntilDelimiter(row, ref i, keyBuilder);
        ConsumeUntilDelimiter(row, ref i, valueBuilder);

        key = keyBuilder.ToString();
        value = valueBuilder.ToString();
    }

    private static void ConsumeUntilDelimiter(string row, ref int i, StringBuilder builder)
    {
        bool inQuotes = false;
        if (i < row.Length && row[i] == '"')
        {
            inQuotes = true;
            i++;
        }

        while (i < row.Length)
        {
            if (row[i] == NEW_LINE)
            {
                i++;
                break;
            }
            else if (inQuotes && row[i] == '"')
            {
                i++;
                if (i < row.Length && row[i] == '"')
                {
                    builder.Append(row[i]);
                    i++;
                }
                else
                {
                    inQuotes = false;
                }

                continue;
            }
            else if (row[i] == ',' && !inQuotes)
            {
                i++;
                break;
            }

            builder.Append(row[i]);
            i++;
        }
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