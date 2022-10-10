using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JsonSerializer
{
    private static readonly HashSet<string> Serializables = new HashSet<string>()
    {
        "Single",
        "Int32",
        "Vector3",
        "Vector2",
        "Vector4",
        "Color",
        "LayerMask",
        "Boolean",
    };
    
    private static readonly HashSet<string> IgnoredValues = new HashSet<string>()
    {
        "Quaternion",
        "Enum"
    };
    
    public static JToken Serialize(object value, Type t)
    {
        if (value != null)
        {
            if (IgnoredValues.Contains(value.GetType().Name) || (value.GetType().BaseType != null && IgnoredValues.Contains(value.GetType().BaseType.Name)))
            {
                JObject variable = new JObject();
                variable.Add("Type","Invalid");
                return variable;
            }
            if(Serializables.Contains(value.GetType().Name))
            {
                JObject variable = new JObject();
                variable.Add("Type","Object");
                variable.Add("Value",JsonConvert.SerializeObject(value));
                return variable;
            }
            else
            {
                if (value.GetType().Name.Equals("List`1",StringComparison.Ordinal))
                {
                    JObject variable = new JObject();
                    variable.Add("Type","List");
                    JArray jarray = new JArray();
                    string a = t.ToString();
                    string listtypename = a.Substring(a.IndexOf('[') + 1, a.LastIndexOf(']') - a.IndexOf('[') - 1);
                    variable.Add("ListTypeName", listtypename);
                    Type tt = ExtensionsAndUtil.GetTypeByName(listtypename);
                    Type listType = typeof(List<>);
                    Type listTypeSpecific = listType.MakeGenericType(tt);
                    dynamic listvar = Convert.ChangeType(value,listTypeSpecific);
                    for (int i = 0; i < listvar.Count; i++)
                    {
                        jarray.Add(Serialize(listvar[i],tt));
                    }
                    variable.Add("Value",jarray);
                    return variable;
                }
                else
                {
                    //Its probebly a refrerence
                    JObject reference = new JObject();
                    reference.Add("Type","Reference");
                    reference.Add("Hash", ReferenceResolver.GetHash(value as UnityEngine.Object));
                    return reference;
                }
            }
        }
        return JsonConvert.SerializeObject(null);
    }
    
    public static object Deserialize(JObject obj, Type t, UnityEngine.Object caller = null)
    {
        if (obj == null) { Debug.LogWarning("Input object was null, deserializing to null!", caller); return null;}
        switch((string)obj.GetValue("Type"))
        {
            case "Reference":
                return ReferenceResolver.Resolve((string)obj.GetValue("Hash"), caller);
            case "Object":
                return JsonConvert.DeserializeObject((string)obj.GetValue("Value") ?? "null", t); //Works
            case "List":
                JArray arr = (JArray)obj.GetValue("Value");
                Type tt = ExtensionsAndUtil.GetTypeByName((string)obj.GetValue("ListTypeName"));
                Type listType = typeof(List<>);
                Type listTypeSpecific = listType.MakeGenericType(tt);
                dynamic listvar = Activator.CreateInstance(listTypeSpecific);
                for (int i = 0; i < arr.Count; i++)
                {
                    dynamic v = Convert.ChangeType(Deserialize((JObject)arr[i],tt,caller),tt);
                    listvar.Add(v);
                }
                return listvar;
            case "Invalid":
                return Activator.CreateInstance(t);
        }
        Debug.LogWarning("Variable had invalid type, failed deserialization! Type: " + (string)obj.GetValue("Type"));
        return null;
    }
}
