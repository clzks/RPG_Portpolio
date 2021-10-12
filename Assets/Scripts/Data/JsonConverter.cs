using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
public static class JsonConverter<T> where T : class, IData
{
    // dataPath는 지정 경로에서 불러오는 것(PersistentDataPath)
    // dataPath가 없으면 StreamingAssets 에서 불러온다 (파일 이름은 ClassName + Data.json 으로 통일된다)
    public static async UniTask<Dictionary<int, T>> GetJsonToDictionaryKeyId(string dataPath)
    {
        string text = string.Empty;
        text = await LoadJsonTextFromDataPath(dataPath);

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
        text = await LoadJsonTextFromDataPath(dataPath);

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
        var filePath = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", className + "Data.json");
#endif 
        var text = await LoadJsonString(filePath);

        return text;
    }
    private static async UniTask<string> LoadJsonTextFromDataPath(string dataPath)
    {
        var filePath = Path.Combine(Application.persistentDataPath, dataPath);
        var text = await LoadJsonString(filePath);
        return text;
    }
    private static async UniTask<string> LoadJsonString(string url)
    {
        FileInfo info = new FileInfo(url);
        if(false == info.Exists)
        {
            return string.Empty;
        }

        UnityWebRequest www = UnityWebRequest.Get($"{url}");
        www.timeout = 3;

        while (!www.isDone)
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("data arrived");
            }
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

    public static void WriteJson(string path, T data)
    {
        var filePath = Path.Combine(Application.persistentDataPath, path);

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static void WriteJson(T data)
    {
#if UNITY_EDITOR
        var filePath = Path.Combine(Application.streamingAssetsPath, typeof(T).Name + ".json");
#elif UNITY_ANDROID
        var filePath = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", typeof(T).Name + "Data.json");
#endif 

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public static async UniTask<T> LoadJson()
    {
        string text = string.Empty;

#if UNITY_EDITOR
        var filePath = Path.Combine(Application.streamingAssetsPath, typeof(T).Name + ".json");
#elif UNITY_ANDROID
        var filePath = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", typeof(T).Name + "Data.json");
#endif 
        text = await LoadJsonString(filePath);

        if(text.Equals(string.Empty))
        {
            return null;
        }

        T value = JObject.Parse(text).ToObject<T>();
        return value;
    }

    public static async UniTask<T> LoadJson(string path)
    {
        string text = string.Empty;
        text = await LoadJsonTextFromDataPath(path);

        T value = JObject.Parse(text).ToObject<T>();
        return value;
    }
}


public interface IData
{
    int GetId();
    string GetName();
}