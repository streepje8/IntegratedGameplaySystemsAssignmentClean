#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using static MonoDodge;

#pragma warning disable CS0618

[CustomEditor(typeof(MonoDodgeComponent))]
public class MonoDodgeComponentEditor : Editor
{
    SerializedProperty myScript;

    void OnEnable()
    {
        // Fetch the objects from the MyScript script to display in the inspector
        myScript = serializedObject.FindProperty("script");
    }
    
    public override void OnInspectorGUI()
    {
        MonoDodgeComponent script = (MonoDodgeComponent)target;
        if (script.script == null)
        {
            EditorGUILayout.PropertyField(myScript);
            EditorGUILayout.LabelField("Please add a C# script here!");
        }
        else
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(myScript);
            GUI.enabled = wasEnabled;
            MonoDodge instance = script.myComponent;
            if (script.isInitialized)
            {
                foreach (FieldInfo inf in instance.GetType().GetFields())
                {
                    if (inf.IsPublic)
                    {
                        switch (inf.FieldType.Name)
                        {
                            case "Single":
                                inf.SetValue(instance,
                                    EditorGUILayout.FloatField(inf.Name, (float)inf.GetValue(instance)));
                                break;
                            case "Int32":
                                inf.SetValue(instance, EditorGUILayout.IntField(inf.Name, (int)inf.GetValue(instance)));
                                break;
                            case "String":
                                inf.SetValue(instance,
                                    EditorGUILayout.TextField(inf.Name, (string)inf.GetValue(instance)));
                                break;
                            case "Vector2":
                                inf.SetValue(instance,
                                    EditorGUILayout.Vector2Field(inf.Name, (Vector2)inf.GetValue(instance)));
                                break;
                            case "Vector3":
                                inf.SetValue(instance,
                                    EditorGUILayout.Vector3Field(inf.Name, (Vector3)inf.GetValue(instance)));
                                break;
                            case "Vector4":
                                inf.SetValue(instance,
                                    EditorGUILayout.Vector4Field(inf.Name, (Vector4)inf.GetValue(instance)));
                                break;
                            case "Color":
                                inf.SetValue(instance,
                                    EditorGUILayout.ColorField(inf.Name, (Color)inf.GetValue(instance)));
                                break;
                            case "GameObject":
                                System.Object value = inf.GetValue(instance);
                                inf.SetValue(instance,
                                    EditorGUILayout.ObjectField(inf.Name, (UnityEngine.Object)value, inf.FieldType));
                                break;
                            case "LayerMask":
                                inf.SetValue(instance,
                                    LayerMaskField(inf.Name, (LayerMask)inf.GetValue(instance)));
                                break;
                            case "Boolean":
                                inf.SetValue(instance,
                                    EditorGUILayout.Toggle(inf.Name, (bool)inf.GetValue(instance)));
                                break;
                            case "EditorLabel":
                                GUILayout.Label(inf.Name.Replace("_DOTDOT", ":").Replace("_", " "),
                                    EditorStyles.boldLabel);
                                break;
                            case "List`1":
                                Rect rect = EditorGUILayout.BeginVertical();
                                GUILayout.Label(inf.Name.Substring(0,1).ToUpper() + inf.Name.Substring(1));
                                EditorGUI.indentLevel++;
                                string a = inf.FieldType.ToString();
                                string listtypename = a.Substring(a.IndexOf('[') + 1, a.LastIndexOf(']') - a.IndexOf('[') - 1);
                                Type t = ExtensionsAndUtil.GetTypeByName(listtypename);
                                Type listType = typeof(List<>);
                                Type listTypeSpecific = listType.MakeGenericType(t);
                                dynamic list = Convert.ChangeType(inf.GetValue(instance),listTypeSpecific);
                                if (list == null)
                                {
                                    list = Convert.ChangeType(Activator.CreateInstance(listTypeSpecific),listTypeSpecific);
                                    inf.SetValue(instance,list);
                                }
                                int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", list.Count));
                                while (newCount < list.Count)
                                    list.RemoveAt( list.Count - 1 );
                                while (newCount > list.Count)
                                {
                                    dynamic v = GetDefaultValue(t);
                                    list.Add(v);
                                }

                                for(int i = 0; i < list.Count; i++)
                                {
                                    list[i] = DrawField("Element " + i,list[i], t);
                                }
                                EditorGUI.indentLevel--;
                                EditorGUILayout.EndVertical();
                                GUI.Box(rect, GUIContent.none);
                                break;
                        }
                        if (inf.FieldType.IsSubclassOf(typeof(Component)))
                        {
                            System.Object value = inf.GetValue(instance);
                            inf.SetValue(instance,
                                EditorGUILayout.ObjectField(inf.Name, (UnityEngine.Object)value, inf.FieldType));
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("Waiting for the monododge to initialize...");
            }
        }
        serializedObject.ApplyModifiedProperties();
        script.SerializeComponent();
    }

    private object DrawField(string name, object o, Type type)
    {
        switch (type.Name)
        {
            case "Single":
                return EditorGUILayout.FloatField(name, (float)o);
            case "Int32":
                return EditorGUILayout.IntField(name, (int)o);
            case "String":
                return
                    EditorGUILayout.TextField(name, (string)o);
            case "Vector2":
                return
                    EditorGUILayout.Vector2Field(name, (Vector2)o);
            case "Vector3":
                return
                    EditorGUILayout.Vector3Field(name, (Vector3)o);
            case "Vector4":
                return
                    EditorGUILayout.Vector4Field(name, (Vector4)o);
            case "Color":
                return
                    EditorGUILayout.ColorField(name, (Color)o);
            case "GameObject":
                System.Object value = o;
                return EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, type);
            case "LayerMask":
                return LayerMaskField(name, (LayerMask)o);
            case "Boolean":
                return EditorGUILayout.Toggle(name, (bool)o);
            case "EditorLabel":
                GUILayout.Label(name.Replace("_DOTDOT", ":").Replace("_", " "),
                    EditorStyles.boldLabel);
                break;
            case "List`1":
                GUILayout.Label(name);
                Rect rect = EditorGUILayout.BeginVertical();
                EditorGUI.indentLevel++;
                string a = type.ToString();
                string listtypename = a.Substring(a.IndexOf('[') + 1, a.LastIndexOf(']') - a.IndexOf('[') - 1);
                Type t = ExtensionsAndUtil.GetTypeByName(listtypename);
                Type listType = typeof(List<>);
                Type listTypeSpecific = listType.MakeGenericType(t);
                dynamic list = Convert.ChangeType(o,listTypeSpecific);
                if (list == null)
                {
                    list = Convert.ChangeType(Activator.CreateInstance(listTypeSpecific),listTypeSpecific);
                }
                int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt( list.Count - 1 );
                while (newCount > list.Count)
                {
                    dynamic v = GetDefaultValue(t);
                    list.Add(v);
                }
                for(int i = 0; i < list.Count; i++)
                {
                    list[i] = DrawField("Element " + i,list[i], t);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                GUI.Box(rect, GUIContent.none);
                return list;
        }
        if (type.IsSubclassOf(typeof(Component)))
            return EditorGUILayout.ObjectField(name, (UnityEngine.Object)o, type);
        return null;
    }

    //Taken From https://stackoverflow.com/questions/2490244/default-value-of-a-type-at-runtime
    public static object GetDefaultValue(Type t)
    {
        if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
            return Activator.CreateInstance(t);
        else
            return null;
    }
    
    //From https://answers.unity.com/questions/42996/how-to-create-layermask-field-in-a-custom-editorwi.html
    private static LayerMask LayerMaskField( string label, LayerMask layerMask) {
        List<string> layers = new List<string>();
        List<int> layerNumbers = new List<int>();
 
        for (int i = 0; i < 32; i++) {
            string layerName = LayerMask.LayerToName(i);
            if (layerName != "") {
                layers.Add(layerName);
                layerNumbers.Add(i);
            }
        }
        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++) {
            if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }
        maskWithoutEmpty = EditorGUILayout.MaskField( label, maskWithoutEmpty, layers.ToArray());
        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++) {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        layerMask.value = mask;
        return layerMask;
    }
}
#endif