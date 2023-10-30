//This script is Just used for the Click Demo2.
//this script is pretty basic...so there is not a lot of comments.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateShockWave_OnClick2 : MonoBehaviour {

    private ShockWave SW;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            SW = ShockWave.Get();
            SW.SetPosition(Input.mousePosition,true);
            SW.radius = Random.Range(0.05f,0.2f);
            SW.amplitude = Random.Range(0.05f,0.2f);
            SW.waveSize = Random.Range(0.05f,0.2f);

        }
    }


    public void DestoryAll()
    {
        ShockWave.DestoryAll();
    }

}
