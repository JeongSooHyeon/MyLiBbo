//This script is Just used for the Click Demo.
//this script is pretty basic...so there is not a lot of comments.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateShockWave_OnClick3 : MonoBehaviour {

    public float speed = 1f;
    public AnimationCurve radiusOverTime;
    public AnimationCurve amplitudeOverTime;
    public AnimationCurve waveSizeOverTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            ShockWave.Get().StartIt(Input.mousePosition,true,speed,radiusOverTime,amplitudeOverTime,waveSizeOverTime);
        }
    }


    public void DestoryAll()
    {
        ShockWave.DestoryAll();
    }

}
