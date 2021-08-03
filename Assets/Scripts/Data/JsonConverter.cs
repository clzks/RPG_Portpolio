using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public static class JsonConverter<T> where T : class, IData
{
    public static async UniTask<Dictionary<int, T>> GetJsonToDictionaryKeyId(string dataPath)
    {
        string text = string.Empty;
        text = await LoadJsonTextFromDataPath(typeof(T).Name, dataPath);

        if (text == string.Empty)
        {
            Debug.LogWarning("Json데이터를 불러오는데 실패하였습니다");
            return null;
        }

        return ConvertJsonDataToIdDictionary(text);
    }

    public static async UniTask<Dictionary<int, T>> GetJsonToDictionaryKeyId()
    {
        string text = string.Empty;
        text = await LoadJsonTextFromStreamingAssetsPath(typeof(T).Name);

        if (text == string.Empty)
        {
            Debug.LogWarning("Json데이터를 불러오는데 실패하였습니다");
            return null;
        }

        return ConvertJsonDataToIdDictionary(text);
    }

    public static async UniTask<Dictionary<string, T>> GetJsonToDictionaryKeyName(string dataPath)
    {
        string text = string.Empty;
        text = await LoadJsonTextFromDataPath(typeof(T).Name, dataPath);

        if (text == string.Empty)
        {
            Debug.LogWarning("Json데이터를 불러오는데 실패하였습니다");
            return null;
        }

        return ConvertJsonDataToNameDictionary(text);
    }

    public static async UniTask<Dictionary<string, T>> GetJsonToDictionaryKeyName()
    {
        string text = string.Empty;
        text = await LoadJsonTextFromStreamingAssetsPath(typeof(T).Name);

        if (text == string.Empty)
        {
            Debug.LogWarning("Json데이터를 불러오는데 실패하였습니다");
            return null;
        }

        return ConvertJsonDataToNameDictionary(text);
    }

    

    private static async UniTask<string> LoadJsonTextFromStreamingAssetsPath(string className)
    {
#if UNITY_EDITOR
        var filePath = Path.Combine(Application.streamingAssetsPath, className + "Data.json");
#elif UNITY_ANDROID
        var filePath = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", className + _fileNameTermination);
#endif 
        var text = await LoadJsonString(filePath);

        return text;
    }
    private static async UniTask<string> LoadJsonTextFromDataPath(string className, string dataPath)
    {
        var filePath = Path.Combine(Application.dataPath, dataPath + className + "Data.json");
        var text = await LoadJsonString(filePath);
        return text;
    }
    private static async UniTask<string> LoadJsonString(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get($"{url}");
        while (!www.isDone)
        {
            await www.SendWebRequest();
            Debug.Log("SendWebRequest 중");
        }
        return www.downloadHandler.text;
    }
    private static Dictionary<int, T> ConvertJsonDataToIdDictionary(string text)
    {
        JArray datas = JObject.Parse(text)[typeof(T).Name] as JArray;
        Dictionary<int, T> dic = new Dictionary<int, T>();
        for (int i = 0; i < datas.Count; ++i)
        {
            var item = datas[i].ToObject<T>();
            dic.Add(item.GetId(), item);
        }
        return dic;
    }
    private static Dictionary<string, T> ConvertJsonDataToNameDictionary(string text)
    {
        JArray datas = JObject.Parse(text)[typeof(T).Name] as JArray;
        Dictionary<string, T> dic = new Dictionary<string, T>();
        for (int i = 0; i < datas.Count; ++i)
        {
            var item = datas[i].ToObject<T>();
            dic.Add(item.GetName(), item);
        }
        return dic;
    }

}


public interface IData
{
    int GetId();
    string GetName();
}