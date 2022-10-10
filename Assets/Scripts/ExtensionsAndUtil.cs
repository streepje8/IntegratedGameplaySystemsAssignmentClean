using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtensionsAndUtil
{
    //stolen from https://stackoverflow.com/questions/20008503/get-type-by-name
    public static Type GetTypeByName(string name)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var tt = assembly.GetType(name);
            if (tt != null)
            {
                return tt;
            }
        }

        return null;
    }
}
