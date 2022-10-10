#if UNITY_EDITOR
using UnityEditor;
#endif
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Assembly = System.Reflection.Assembly;

[ExecuteAlways]
public class MonoDodgeComponent : MonoBehaviour
{
    public Object script;
    [SerializeField]private string myCompJson;
    [HideInInspector][SerializeField]private string scriptName;
    [SerializeField] public string GUID;
    [HideInInspector][NonSerialized]public MonoDodge myComponent;
    [HideInInspector] [NonSerialized] public bool isInitialized = false;


    private void OnValidate()
    {
        LoadSerializedValues();
        if (GUID == null || GUID.Length < 2)
        {
            GUID = Guid.NewGuid().ToString();
        }
    }

    private void Awake()
    {
        //We sadly cant use this since unity is an absolute buffoon
        #if UNITY_EDITOR
        LoadSerializedValues();
        #endif
    }
    
    void Start()
    {
        LoadSerializedValues();
    }

    private void OnDrawGizmos() => myComponent?.OnDrawGizmos();

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (Application.isPlaying) myComponent?.OnRenderImage(src, dest);
    }

    public T  GetInstance<T>() where T : MonoDodge
    {
        if(myComponent == null) Debug.Log("My dodge instance is null, but it was still requested!", gameObject);
        return myComponent as T;
    }

    private bool hasAwoken = false;
    private bool hasStarted = false;

    void Update()
    {
        if (!hasAwoken)
        {
            if(Application.isPlaying) myComponent?.Awake();
            hasAwoken = true;
        }

        if (!hasStarted)
        {
            if(Application.isPlaying) myComponent?.Start();
            hasStarted = true;
        }
        #if UNITY_EDITOR
                if (myComponent == null && script != null)
                {
                    MonoScript mscript = (MonoScript)script;
                    scriptName = mscript.name;
                    Type scriptType = Assembly.GetExecutingAssembly().GetType(scriptName);
                    MonoDodge dodge = (MonoDodge)Activator.CreateInstance(scriptType);
                    dodge.SetMDC(this);
                    myComponent = dodge;
                    SerializeComponent();
                }
        #endif
        if (Application.isPlaying && hasStarted) myComponent?.Update();
    }

    public void LoadSerializedValues()
    {
        if (!ReferenceResolver.isInitialized) ReferenceResolver.Initialize();
        if (scriptName != null)
        {
            Type scriptType = Assembly.GetExecutingAssembly().GetType(scriptName);
            MonoDodge dodge = (MonoDodge)Activator.CreateInstance(scriptType);
            dodge.SetMDC(this);
            myComponent = dodge;
            Dictionary<string, JObject> serializedVars = new Dictionary<string, JObject>();
            JObject saved = JObject.Parse(myCompJson);
            JArray arr = saved.GetValue("Fields") as JArray;
            for (int i = 0; i < arr.Count; i++)
            {
                JObject var = arr[i] as JObject;
                serializedVars.Add((string)var.GetValue("Name"), var);
            }

            foreach (FieldInfo var in myComponent.GetType().GetFields())
            {
                if (var.IsPublic)
                {
                    if (serializedVars.TryGetValue(var.Name, out JObject savedata))
                    {
                        if (savedata.ContainsKey("Value"))
                        {
                            JToken jvalue = savedata.GetValue("Value");
                            if ((jvalue as JObject) != null)
                            {
                                dynamic v = Convert.ChangeType(JsonSerializer.Deserialize(jvalue as JObject, var.FieldType,
                                    gameObject), var.FieldType);
                                var.SetValue(myComponent, v);
                            }
                            else
                            {
                                //Debug.LogWarning("Encountered invalid json: " + savedata.ToString()); its probebly safe to ignore this
                            }
                        }
                        else
                        {
                            Debug.Log("Could not deserealize " + var.Name + ". JSON: " + savedata.ToString());
                        }
                    }
                    else
                    {
                        Debug.LogWarning("A field was found on the instance of " + myComponent.GetType().Name + ", but it wasn't serialized to json! VAR: " + var.Name);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Script name is null!", gameObject);
        }
        isInitialized = true;
    }

    public void SerializeComponent()
    {
        if (myComponent != null)
        {
            JObject obj = new JObject();
            JArray varArr = new JArray();
            foreach (FieldInfo var in myComponent.GetType().GetFields())
            {
                if (var.IsPublic)
                {
                    JObject varObj = new JObject();
                    varObj.Add("Name", var.Name);
                    varObj.Add("Value", JsonSerializer.Serialize(var.GetValue(myComponent),var.FieldType));
                    varArr.Add(varObj);
                }
            }

            obj.Add("Fields", varArr);
            myCompJson = obj.ToString(Formatting.Indented);
        }
    }
}
