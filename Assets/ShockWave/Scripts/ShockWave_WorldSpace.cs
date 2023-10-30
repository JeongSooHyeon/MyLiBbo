/*
ShockWave_WorldSpace.cs
Creates and Manages the ShockWaves (the ones in the worldspace)
*/


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ShockWave_WorldSpace : MonoBehaviour 
{

    /// <summary>
    /// this is the material that will store the ShockWave Shader
    /// </summary>
    public Material mat;

    /// <summary>
    /// The speed. if speed is zero animation will not run, and can be animated with animator
    /// </summary>
    public float speed = 1f;

    /// <summary>
    /// The t, the time for this object
    /// </summary>
    public float t = 0f;

    /// <summary>
    /// The radius over time.
    /// </summary>
    public AnimationCurve radiusOverTime;

    /// <summary>
    /// The amplitude over time.
    /// </summary>
    public AnimationCurve amplitudeOverTime;

    /// <summary>
    /// The wave size over time.
    /// </summary>
    public AnimationCurve waveSizeOverTime;

    /// <summary>
    /// Generic delegate.
    /// </summary>
    public delegate void genericDelegate(ShockWave_WorldSpace ThisShockWave);

    /// <summary>
    /// The Delegates that executes when animation is comeplete. if this is not set object will be destroyed.
    /// </summary>
    public genericDelegate OnAnimationComplete;

    void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }
            
        //set the Shader
        if (mat == null)
        {
            mat = new Material(Shader.Find("Custom/ShockWave(WorldSpace)"));
        }

        //make sure this material is an instance
        GetComponent<MeshRenderer>().material = mat;
        mat = GetComponent<MeshRenderer>().material;

        //set the radius amplitude and wavesize to zero
        radius = radiusOverTime.Evaluate(0f);
        amplitude = amplitudeOverTime.Evaluate(0f);
        waveSize = waveSizeOverTime.Evaluate(0f);
        
    }


//these are the basic variables that will be used
#region GetSetVariables

    /// <summary>
    /// the Radius of the ShockWave
    /// </summary>
    protected float _radius;
    public float radius 
    {
      get { return _radius; }
      set { 
          _radius=value;
          mat.SetFloat("_Radius",_radius);
      }
    }

    /// <summary>
    /// the Amplitude of the ShockWave
    /// AKA how much distortion the ShockWave will have.
    /// </summary>
    private float _amplitude;
    public float amplitude 
    {
        get { return _amplitude; }
        set { 
            _amplitude = value;
            mat.SetFloat("_Amplitude",_amplitude);
        }
    }

    /// <summary>
    /// the WaveSize of the ShockWave
    /// AKA the Thickness of the ShockWave.
    /// </summary>
    private float _wavesize;
    public float waveSize 
    {
        get { return _wavesize; }
        set { 
            _wavesize=value;
            mat.SetFloat("_WaveSize",_wavesize);
        }
    } 

#endregion


    /// <summary>
    /// Fixeds the update.
    /// </summary>
    void FixedUpdate()
    {
        //update the radius amplitude and waveSize
        radius = radiusOverTime.Evaluate(t);
        amplitude = amplitudeOverTime.Evaluate(t);
        waveSize = waveSizeOverTime.Evaluate(t);

        //if speed is 0, animation will not occur and t can be over written with another script/component
        if (speed == 0f)
        {
            return;
        }

        //increment t
        t += (speed * Time.deltaTime);

        //if t is over 1 then destory or execute OnAnimationComplete()
        if (t > 1f)
        {
            if (OnAnimationComplete == null)
            {
                Destroy(gameObject);
            }
            else
            {
                OnAnimationComplete(this);
            }
        }
    }


//this part is used in the editor only...allows a preview of the animation by using the slider
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
    [Range(0.0f, 1.0f)]
    public float timePreview_InEditModeOnly = 0f;
    void Update()
    {
        //not while the editor is in play mode
        if (!Application.isPlaying)
        {
            GetComponent<MeshRenderer>().material = mat;
            mat = GetComponent<MeshRenderer>().sharedMaterial;

            radius = radiusOverTime.Evaluate(timePreview_InEditModeOnly);
            amplitude = amplitudeOverTime.Evaluate(timePreview_InEditModeOnly);
            waveSize = waveSizeOverTime.Evaluate(timePreview_InEditModeOnly);
        }
    }
#endif

}

