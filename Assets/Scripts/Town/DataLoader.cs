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
        LoadData();
    }

    void LoadData()
    {
        string fileName = "Items";
        string path = Application.streamingAssetsPath + "/" + fileName + ".csv";
        Items = ReadCSV<Item>(path);

        // 데이터 출력 (ID가 1부터 시작)
        if (Items != null)
        {
            //Debug.Log($"Loaded {Items.Count} items.");
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                //Debug.Log($"Item: ID = {item.item_id}, Name = {item.item_name}, Description = {item.item_description}");
            }
        }
        else
        {
            Debug.Log("No items loaded.");
        }
    }

    public Item GetItemById(int id)
    {
        return Items.Find(item => item.item_id == id);
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
                    string[] fields = line.Split(',');
                    T data = new T();
                    bool skip = false;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var property = typeof(T).GetProperty(headers[i]);
                        if (property == null)
                        {
                            Debug.LogWarning($"Property {headers[i]} not found in {typeof(T)}");
                            continue;
                        }

                        try
                        {
                            if (property.PropertyType == typeof(int))
                            {
                                if (int.TryParse(fields[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                                {
                                    property.SetValue(data, intValue);
                                }
                                else
                                {
                                    Debug.LogWarning($"Defaulting field {headers[i]} with value {fields[i]} to 0 on line {lineNumber}");
                                    property.SetValue(data, 0);
                                }
                            }
                            else if (property.PropertyType == typeof(float))
                            {
                                if (float.TryParse(fields[i], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float floatValue))
                                {
                                    property.SetValue(data, floatValue);
                                }
                                else
                                {
                                    Debug.LogWarning($"Defaulting field {headers[i]} with value {fields[i]} to 0.0 on line {lineNumber}");
                                    property.SetValue(data, 0.0f);
                                }
                            }
                            else if (property.PropertyType == typeof(bool))
                            {
                                if (bool.TryParse(fields[i], out bool boolValue))
                                {
                                    property.SetValue(data, boolValue);
                                }
                                else
                                {
                                    property.SetValue(data, fields[i] == "1" || fields[i].ToLower() == "true");
                                }
                            }
                            else
                            {
                                property.SetValue(data, fields[i]);
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
