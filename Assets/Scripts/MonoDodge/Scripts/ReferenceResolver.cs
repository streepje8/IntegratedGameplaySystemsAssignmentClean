using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public static class ReferenceResolver
{
    public static bool isInitialized = false;
    private static Dictionary<string, object> HashBase = new Dictionary<string, object>();

    public static void Initialize()
    {
        isInitialized = true;
        HashBase = new Dictionary<string, object>();
        List<Scene> scenes = GetLoadedScenes();
        if (scenes.Count < 1)
        {
            Debug.LogWarning("No Loaded Scene Found! Force loading the first one!!");
            SceneManager.LoadScene(0);
            scenes = GetLoadedScenes();
        }
        foreach (Scene s in scenes)
        {
            if (s.isLoaded)
            {
                foreach (GameObject obj in s.GetRootGameObjects())
                {
                    AddHashes(obj);
                }
            }
            else
            {
                Debug.LogWarning("You can only use the reference resolver in the start function!!!");
            }
        }
    }
    
    public static string GetHash(UnityEngine.Object obj)
    {
        if(!isInitialized) Initialize();
        var hash = new Hash128();
        if (obj != null)
        {
            hash.Append(obj.name);
            hash.Append(obj.GetType().FullName);
            if (obj.GetType() == typeof(MonoDodgeComponent))
            {
                return ((MonoDodgeComponent)obj).GUID;
            }
        }
        else
        {
            hash.Append(null);
        }
        return hash.ToString();
    }

    public static object Resolve(string hash, UnityEngine.Object caller = null, int attemps = 1)
    {
        if (HashBase.TryGetValue(hash, out object value))
        {
            return value;
        }
        else
        {
            if(HashBase.Keys.Count > 0) Debug.LogError("Hash: " + hash + " not found... Looked through " + HashBase.Keys.Count + " hashes. Make sure that you do not have two game objects with the same name!", caller);
            if (HashBase.Keys.Count < 1)
            {
                Debug.Log("Failed to resolve hash, is the hash base initialized? Attempting automatic backup, attempt " + attemps + "/10...");
                Initialize();
                if(attemps < 11)
                    return Resolve(hash, caller, attemps + 1);
            }

            return null;
        }
    }

    private static void AddHashes(GameObject gameObject)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            AddHashes(gameObject.transform.GetChild(i).gameObject);
        }
        foreach (Component c in gameObject.GetComponents(typeof(Component)))
        {
            string chash = GetHash(c);
            if(!HashBase.ContainsKey(chash))
                HashBase.Add(chash,c);
        }
        string hash = GetHash(gameObject);
        if(!HashBase.ContainsKey(hash))
            HashBase.Add(hash,gameObject);
    }

    private static List<Scene> GetLoadedScenes()
    {
        int countLoaded = SceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[countLoaded];
 
        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }

        return new List<Scene>(loadedScenes);
    }
}
