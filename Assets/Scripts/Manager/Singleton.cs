using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>, new()
{
    private static readonly object _lock = new object();
    private static T _instance = null;
    public static T Get()
    {
        lock (_lock)
        {
            if (null == _instance)
            {
                T[] objs = FindObjectsOfType<T>();

                if (objs.Length > 0)
                {
                    _instance = objs[0];
                }

                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }

                if (_instance == null)
                {
                    string goName = typeof(T).ToString();
                    GameObject go = GameObject.Find(goName);
                    if (go == null)
                    {
                        go = new GameObject(goName);
                    }
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }
}
