using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;
using Assets.Scripts.Town.Data;
using System;

public class DataLoader : MonoBehaviour
{
    public static DataLoader Instance { get; private set; }
    public List<Item> Items { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Scene 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("DataLoader Start method called.");
        LoadData();
    }

    void LoadData()
    {
        Debug.Log("Loading data...");
        Items = ReadCSV<Item>("Assets/Scripts/CSV/Items.csv");

        // 데이터 출력 (ID가 1부터 시작)
        if (Items != null)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                Debug.Log($"Item: ID = {item.item_id}, Name = {item.item_name}, Description = {item.item_description}");
            }
        }
        else
        {
            Debug.Log("No items loaded.");
        }
    }

    private List<T> ReadCSV<T>(string filePath) where T : class, new()
    {
        List<T> dataList = new List<T>();

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string header = reader.ReadLine();
                string[] headers = header.Split(',');

                string line;
                int lineNumber = 1;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    Debug.Log($"Reading line {lineNumber}: {line}");
                    string[] fields = line.Split(',');
                    T data = new T();
                    bool skip = false;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var field = typeof(T).GetField(headers[i]);
                        if (field == null)
                        {
                            Debug.LogWarning($"Field {headers[i]} not found in {typeof(T)}");
                            continue;
                        }

                        try
                        {
                            if (field.FieldType == typeof(int))
                            {
                                if (int.TryParse(fields[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                                {
                                    field.SetValue(data, intValue);
                                }
                                else
                                {
                                    Debug.LogWarning($"Defaulting field {headers[i]} with value {fields[i]} to 0 on line {lineNumber}");
                                    field.SetValue(data, 0);
                                }
                            }
                            else if (field.FieldType == typeof(float))
                            {
                                if (float.TryParse(fields[i], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float floatValue))
                                {
                                    field.SetValue(data, floatValue);
                                }
                                else
                                {
                                    Debug.LogWarning($"Defaulting field {headers[i]} with value {fields[i]} to 0.0 on line {lineNumber}");
                                    field.SetValue(data, 0.0f);
                                }
                            }
                            else if (field.FieldType == typeof(bool))
                            {
                                if (bool.TryParse(fields[i], out bool boolValue))
                                {
                                    field.SetValue(data, boolValue);
                                }
                                else
                                {
                                    field.SetValue(data, fields[i] == "1" || fields[i].ToLower() == "true");
                                }
                            }
                            else
                            {
                                field.SetValue(data, fields[i]);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Unexpected error parsing field {headers[i]} with value {fields[i]} on line {lineNumber}: {ex.Message}");
                            skip = true;
                            break;
                        }
                    }
                    if (!skip)
                    {
                        dataList.Add(data);
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"CSV file not found at path: {filePath}");
        }

        return dataList;
    }
}
