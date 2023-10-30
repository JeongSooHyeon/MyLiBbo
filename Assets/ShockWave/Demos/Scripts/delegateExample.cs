using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class delegateExample : MonoBehaviour {

    public static void printName(ShockWave_WorldSpace TSW)
    {
        print(TSW.gameObject.name);
    }

    public static void printPosition(ShockWave_WorldSpace TSW)
    {
        print(TSW.gameObject.transform.position);
    }

    public static void reset(ShockWave_WorldSpace TSW)
    {
        TSW.t = 0f;
    }
}
