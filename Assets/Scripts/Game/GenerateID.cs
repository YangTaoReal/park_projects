using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenerateID
{
    //static Guid guid = Guid.Empty;
    public static Guid ID
    {
        get { return Guid.NewGuid();}
    }

    public static int SaveRate
    {
        get { return 5; }
    }

    //隐藏安置点
    public static Vector3 HidePos
    {
        get { return Vector3.one * 999; }
    }
}
