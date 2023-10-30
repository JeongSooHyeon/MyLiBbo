/*
ShockWave
Version 2.0

1. Creates and Manages the ShockWave
*/


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ShockWave : MonoBehaviour {

    /// <summary>
    /// this is the material that will store the ShockWave Shader
    /// </summary>
    public Material mat;


    /// <summary>
    /// The camera you would like this to render on
    /// </summary>
    public Camera renderCamera;

    void Awake()
    {
        //set the Shader
        mat = new Material(Shader.Find("Custom/ShockWave"));

        if (renderCamera == null)
        {
            renderCamera = GetComponent<Camera>();
        }

    }

    /// <summary>
    /// Creates and Returns the ShockWave
    /// </summary>
    static public ShockWave Get()  
    {
        ShockWave SW = Camera.main.gameObject.AddComponent<ShockWave>(); 
        return SW;
    }

    //override to allow different camera
    static public ShockWave Get(Camera ThisCamera)
    {
        
        ShockWave SW = ThisCamera.gameObject.AddComponent<ShockWave>();
        return SW;
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

//other variables that we'll need to store data
#region OtherVariables

    //for Processing01
    //the MaxRadius of the ShockWave
    private float maxRadius;
    //used to store the Max Amplitude during Reverse Shosckwave
    private float maxAmplitude;


    //for Processing02
    private AnimationCurve radiusOT;
    private AnimationCurve amplitudeOT;
    private AnimationCurve waveSizeOT;

    //for Processing01 and Processing02
    private GameObject target;

    //the Speed of the ShockWave
    private float speed;

    // current time...used in Processing02
    private float t = 0f;

    //used to determine if an Coroutine is animating the ShockWave
    private bool isAnimating = false;

#endregion

//These StartIt methods use the Processing01 coroutine to animate.
#region StartIt_usingProcessing01

    /// <summary>
    /// Starts it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="MaxRadius">Max radius.</param>
    /// <param name="Amplitude">Amplitude.</param>
    /// <param name="WaveSize">Wave size.</param>
    public void StartIt(Vector2 Position, float Speed = 1f,float MaxRadius = 1f, float Amplitude = 1f , float WaveSize = 0.2f) 
    {
        //assign values to variables
        radius = -0.2f;
        maxRadius = MaxRadius;
        speed = Speed;
        amplitude = Amplitude;
        maxAmplitude = Amplitude;
        waveSize = WaveSize;

        //Set Position
        SetPosition(Position);

        //Start Coroutine to animate
        StartCoroutine("Processing01");

    }

    /// <summary>
    /// Starts it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="MaxRadius">Max radius.</param>
    /// <param name="Amplitude">Amplitude.</param>
    /// <param name="WaveSize">Wave size.</param>
    public void StartIt(Vector3 Position, float Speed = 1f,float MaxRadius = 1f, float Amplitude = 1f , float WaveSize = 0.2f) 
    {
        //assign values to variables
        radius=  -0.2f;
        maxRadius = MaxRadius;
        speed = Speed;
        amplitude = Amplitude;
        maxAmplitude = Amplitude;
        waveSize = WaveSize;

        //Set Position
        SetPosition(Position);

        //Start Coroutine to animate
        StartCoroutine("Processing01");

    }

    /// <summary>
    /// Starts it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="IsScreenPosition">If set to <c>true</c> is screen position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="MaxRadius">Max radius.</param>
    /// <param name="Amplitude">Amplitude.</param>
    /// <param name="WaveSize">Wave size.</param>
    public void StartIt(Vector3 Position, bool IsScreenPosition, float Speed = 1f,float MaxRadius = 1f, float Amplitude = 1f , float WaveSize = 0.2f) 
    {
        //assign values to variables
        radius=  -0.2f;
        maxRadius = MaxRadius;
        speed = Speed;
        amplitude = Amplitude;
        maxAmplitude = Amplitude;
        waveSize = WaveSize;


        //Set Position
        SetPosition(Position,IsScreenPosition);


        //Start Coroutine to animate
        StartCoroutine("Processing01");

    }
        
    /// <summary>
    /// Starts it.
    /// </summary>
    /// <param name="Target">Target.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="MaxRadius">Max radius.</param>
    /// <param name="Amplitude">Amplitude.</param>
    /// <param name="WaveSize">Wave size.</param>
    public void StartIt(GameObject Target, float Speed = 1f,float MaxRadius = 1f, float Amplitude = 1f , float WaveSize = 0.2f) 
    {
        //assign values to variables
        radius=  -0.2f;
        maxRadius = MaxRadius;
        speed = Speed;
        amplitude = Amplitude;
        maxAmplitude = Amplitude;
        waveSize = WaveSize;


        //Set Position
        SetPosition(Target);

        //Start Coroutine to animate
        StartCoroutine("Processing01");

    }

#endregion

//These ReverseIt methods use the RevProcessing01 coroutine to animate....the RevProcessing01 animates in reverse.
#region ReverseIt_usingRevProcessing01

    /// <summary>
    /// Reverses it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="MaxRadius">Max radius.</param>
    /// <param name="Amplitude">Amplitude.</param>
    /// <param name="WaveSize">Wave size.</param>
    public void ReverseIt(Vector2 Position, float Speed = 1f,float MaxRadius = 1f, float Amplitude = 1f , float WaveSize = 0.2f) 
    {
        //assign values to variables
        radius =  MaxRadius;
        maxRadius = MaxRadius;
        speed = Speed;
        amplitude = 0f;
        maxAmplitude = Amplitude;
        waveSize = WaveSize;

        //set Position
        SetPosition(Position);

        //Start Coroutine to animate
        StartCoroutine("RevProcessing01");

    }

    /// <summary>
    /// Reverses it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="MaxRadius">Max radius.</param>
    /// <param name="Amplitude">Amplitude.</param>
    /// <param name="WaveSize">Wave size.</param>
    public void ReverseIt(Vector3 Position, float Speed = 1f,float MaxRadius = 1f, float Amplitude = 1f , float WaveSize = 0.2f) 
    {
        //assign values to variables
        radius =  MaxRadius;
        maxRadius = MaxRadius;
        speed = Speed;
        amplitude = 0f;
        maxAmplitude = Amplitude;
        waveSize = WaveSize;

        //set Position
        SetPosition(Position);

        //Start Coroutine to animate
        StartCoroutine("RevProcessing01");

    }

    /// <summary>
    /// Reverses it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="IsScreenPosition">If set to <c>true</c> is screen position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="MaxRadius">Max radius.</param>
    /// <param name="Amplitude">Amplitude.</param>
    /// <param name="WaveSize">Wave size.</param>
    public void ReverseIt(Vector3 Position, bool IsScreenPosition, float Speed = 1f,float MaxRadius = 1f, float Amplitude = 1f , float WaveSize = 0.2f) 
    {
        //assign values to variables
        radius=  MaxRadius;
        maxRadius = MaxRadius;
        speed = Speed;
        amplitude = 0f;
        maxAmplitude = Amplitude;
        waveSize = WaveSize;

        //set Position
        SetPosition(Position,IsScreenPosition);

        //Start Coroutine to animate
        StartCoroutine("RevProcessing01");

    }

    /// <summary>
    /// Reverses it.
    /// </summary>
    /// <param name="Target">Target.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="MaxRadius">Max radius.</param>
    /// <param name="Amplitude">Amplitude.</param>
    /// <param name="WaveSize">Wave size.</param>
    public void ReverseIt(GameObject Target, float Speed = 1f,float MaxRadius = 1f, float Amplitude = 1f , float WaveSize = 0.2f) 
    {
        //assign values to variables
        radius=  MaxRadius;
        maxRadius = MaxRadius;
        speed = Speed;
        amplitude = 0f;
        maxAmplitude = Amplitude;
        waveSize = WaveSize;

        //set Position
        SetPosition(Target);

        //Start Coroutine to animate
        StartCoroutine("RevProcessing01");

    }
#endregion

//These StartIt methods use the Processing02 coroutine to animate....Processing02 will animate based on the AnimationCurves
#region StartIt_usingProcessing02

    /// <summary>
    /// Starts it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="radiusOverTime">Radius over time.</param>
    /// <param name="amplitudeOverTime">Amplitude over time.</param>
    /// <param name="waveSizeOverTime">Wave size over time.</param>
    public void StartIt(Vector2 Position, float Speed = 1f, AnimationCurve radiusOverTime =  null, AnimationCurve amplitudeOverTime = null , AnimationCurve waveSizeOverTime = null)
    {
        //if null use the default AnimationCurves
        if (radiusOverTime == null)
        {
            radiusOverTime = defaultAnimationCurve_Radius();
        }

        if (amplitudeOverTime == null)
        {
            amplitudeOverTime = defaultAnimationCurve_Amplitude();
        }

        if (waveSizeOverTime == null)
        {
            waveSizeOverTime = defaultAnimationCurve_WaveSize();
        }

        //don't let speed be zero
        if (Speed <= 0f)
        {
            Debug.LogError("the speed should not be zero or less");
        }

        //assign values
        radiusOT = radiusOverTime;
        amplitudeOT = amplitudeOverTime;
        waveSizeOT = waveSizeOverTime;

        t = 0;
        speed = Speed;

        radius = radiusOT.Evaluate(t);
        amplitude = amplitudeOT.Evaluate(t);
        waveSize = waveSizeOT.Evaluate(t);

        //Set Position
        SetPosition(Position);

        //Start Coroutine to animate
        StartCoroutine("Processing02");

    }

    /// <summary>
    /// Starts it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="radiusOverTime">Radius over time.</param>
    /// <param name="amplitudeOverTime">Amplitude over time.</param>
    /// <param name="waveSizeOverTime">Wave size over time.</param>
    public void StartIt(Vector3 Position, float Speed = 1f, AnimationCurve radiusOverTime =  null, AnimationCurve amplitudeOverTime = null , AnimationCurve waveSizeOverTime = null)
    {
        //if null use the default AnimationCurves
        if (radiusOverTime == null)
        {
            radiusOverTime = defaultAnimationCurve_Radius();
        }

        if (amplitudeOverTime == null)
        {
            amplitudeOverTime = defaultAnimationCurve_Amplitude();
        }

        if (waveSizeOverTime == null)
        {
            waveSizeOverTime = defaultAnimationCurve_WaveSize();
        }

        //don't let speed be zero
        if (Speed <= 0f)
        {
            Debug.LogError("the speed should not be zero or less");
        }

        //assign values
        radiusOT = radiusOverTime;
        amplitudeOT = amplitudeOverTime;
        waveSizeOT = waveSizeOverTime;

        t = 0;
        speed = Speed;

        radius = radiusOT.Evaluate(t);
        amplitude = amplitudeOT.Evaluate(t);
        waveSize = waveSizeOT.Evaluate(t);

        //set Position
        SetPosition(Position);

        //Start Coroutine to animate
        StartCoroutine("Processing02");

    }

    /// <summary>
    /// Starts it.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="IsScreenPosition">If set to <c>true</c> is screen position.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="radiusOverTime">Radius over time.</param>
    /// <param name="amplitudeOverTime">Amplitude over time.</param>
    /// <param name="waveSizeOverTime">Wave size over time.</param>
    public void StartIt(Vector3 Position, bool IsScreenPosition, float Speed = 1f, AnimationCurve radiusOverTime =  null, AnimationCurve amplitudeOverTime = null , AnimationCurve waveSizeOverTime = null)
    {
        //if null use the default AnimationCurves
        if (radiusOverTime == null)
        {
            radiusOverTime = defaultAnimationCurve_Radius();
        }

        if (amplitudeOverTime == null)
        {
            amplitudeOverTime = defaultAnimationCurve_Amplitude();
        }

        if (waveSizeOverTime == null)
        {
            waveSizeOverTime = defaultAnimationCurve_WaveSize();
        }

        //don't let speed be zero
        if (Speed <= 0f)
        {
            Debug.LogError("the speed should not be zero or less");
        }

        //assign values
        radiusOT = radiusOverTime;
        amplitudeOT = amplitudeOverTime;
        waveSizeOT = waveSizeOverTime;

        t = 0;
        speed = Speed;

        radius = radiusOT.Evaluate(t);
        amplitude = amplitudeOT.Evaluate(t);
        waveSize = waveSizeOT.Evaluate(t);

        //set Position
        SetPosition(Position, IsScreenPosition);

        //Start Coroutine to animate
        StartCoroutine("Processing02");

    }
        
    /// <summary>
    /// Starts it.
    /// </summary>
    /// <param name="Target">Target.</param>
    /// <param name="Speed">Speed.</param>
    /// <param name="radiusOverTime">Radius over time.</param>
    /// <param name="amplitudeOverTime">Amplitude over time.</param>
    /// <param name="waveSizeOverTime">Wave size over time.</param>
    public void StartIt(GameObject Target, float Speed = 1f, AnimationCurve radiusOverTime =  null, AnimationCurve amplitudeOverTime = null , AnimationCurve waveSizeOverTime = null)
    {
        //if null use the default AnimationCurves
        if (radiusOverTime == null)
        {
            radiusOverTime = defaultAnimationCurve_Radius();
        }

        if (amplitudeOverTime == null)
        {
            amplitudeOverTime = defaultAnimationCurve_Amplitude();
        }

        if (waveSizeOverTime == null)
        {
            waveSizeOverTime = defaultAnimationCurve_WaveSize();
        }

        //don't let speed be zero
        if (Speed <= 0f)
        {
            Debug.LogError("the speed should not be zero or less");
        }

        //assign values
        radiusOT = radiusOverTime;
        amplitudeOT = amplitudeOverTime;
        waveSizeOT = waveSizeOverTime;

        t = 0;
        speed = Speed;

        radius = radiusOT.Evaluate(t);
        amplitude = amplitudeOT.Evaluate(t);
        waveSize = waveSizeOT.Evaluate(t);

        //set Position
        SetPosition(Target);

        //Start Coroutine to animate
        StartCoroutine("Processing02");

    }

#endregion

//These methods are used to set the position of the ShockWave
#region setPosition

    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="Position">Position.</param>
    public void SetPosition(Vector2 Position) 
    {

        Vector2 V2 = renderCamera.ScreenToViewportPoint(Position);

        mat.SetFloat("_CenterX",V2.x);
        mat.SetFloat("_CenterY",V2.y);
        mat.SetFloat("_ScreenRatio", (int)Screen.width/(float)Screen.height );

    }

    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="Position">Position.</param>
    public void SetPosition(Vector3 Position) 
    {

        Vector2 V2 = renderCamera.WorldToViewportPoint(Position);

        mat.SetFloat("_CenterX",V2.x);
        mat.SetFloat("_CenterY",V2.y);
        mat.SetFloat("_ScreenRatio", (int)Screen.height/(float)Screen.width );

    }

    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="Position">Position.</param>
    /// <param name="IsScreenPosition">If set to <c>true</c> is screen position.</param>
    public void SetPosition(Vector3 Position, bool IsScreenPosition) 
    {

        Vector2 V2;
        if (IsScreenPosition)
        {
            V2 = renderCamera.ScreenToViewportPoint(Position);
        }
        else
        {
            V2 = renderCamera.WorldToViewportPoint(Position);
        }

        mat.SetFloat("_CenterX",V2.x);
        mat.SetFloat("_CenterY",V2.y);
        mat.SetFloat("_ScreenRatio", (int)Screen.height/(float)Screen.width );

    }
        
    /// <summary>
    /// Sets the position.
    /// </summary>
    /// <param name="Target">Target.</param>
    public void SetPosition(GameObject Target) 
    {

        target = Target;

        Vector2 V2;
        V2 = renderCamera.WorldToViewportPoint(Target.transform.position);

        mat.SetFloat("_CenterX",V2.x);
        mat.SetFloat("_CenterY",V2.y);
        mat.SetFloat("_ScreenRatio", (int)Screen.height/(float)Screen.width );

    }


    //Update the position based on the target GameObject...if not Animating and the target Exists
    void FixedUpdate()
    {
        if (!isAnimating && target != null)
        {
            Vector2 V2;
            V2 = renderCamera.WorldToViewportPoint(target.transform.position);

            mat.SetFloat("_CenterX",V2.x);
            mat.SetFloat("_CenterY",V2.y);
        }
    }
#endregion

//These are the coroutines that animate the ShockWaves
#region ProcessingCoroutines

    //This will animate the ShockWave
    private IEnumerator Processing01()
    {

        isAnimating = true;

        while (radius < maxRadius * 0.995f)
        {

            if (Paused)
            {
                //do nothing
            }
            else
            {
                //move the radius to the MaxRadius using lerp
                radius = Mathf.Lerp(radius,maxRadius,Time.deltaTime * speed);

                //die down the amplitude while the radius raises
                amplitude = (1f - (Mathf.Clamp(radius,0f,maxRadius)/maxRadius)) * maxAmplitude ;

                //only Move the Shockwave if the Target Exists
                if (target != null)
                {
                    Vector2 V2;
                    V2 = renderCamera.WorldToViewportPoint(target.transform.position);

                    mat.SetFloat("_CenterX",V2.x);
                    mat.SetFloat("_CenterY",V2.y);
                }
            }

            yield return null;
        }

        //destory after processing
        Destroy(this);
    }

    //This will animate the ShockWave...but in reverse
    private IEnumerator RevProcessing01()
    {

        isAnimating = true;

        while (radius > -0.2f)
        {

            if (Paused)
            {
                //do nothing
            }
            else
            {
                //move the radius to the -0.3f using lerp
                radius = Mathf.Lerp(radius,-0.3f,Time.deltaTime * speed);

                //die down the amplitude while the radius raises
                amplitude = (1f - (Mathf.Clamp(radius,0f,maxRadius)/maxRadius)) * maxAmplitude;

                if (target != null)
                {
                    Vector2 V2;
                    V2 = renderCamera.WorldToViewportPoint(target.transform.position);

                    mat.SetFloat("_CenterX",V2.x);
                    mat.SetFloat("_CenterY",V2.y);
                }
            }

            yield return null;
        }

        //destory after processing
        Destroy(this);
    }

    //this will animate the ShockWave(s) that use AnimationCurves
    private IEnumerator Processing02()
    {

        isAnimating = true;

        while (t < 1f)
        {

            if (Paused)
            {
                //do nothing
            }
            else
            {
                t += (speed * Time.deltaTime);

                radius = radiusOT.Evaluate(t);
                amplitude = amplitudeOT.Evaluate(t);
                waveSize = waveSizeOT.Evaluate(t);

                if (target != null)
                {
                    Vector2 V2;
                    V2 = renderCamera.WorldToViewportPoint(target.transform.position);

                    mat.SetFloat("_CenterX",V2.x);
                    mat.SetFloat("_CenterY",V2.y);
                }
            }

            yield return null;
        }

        //destory after processing
        Destroy(this);
    }

#endregion

//These are the default AnimationCurves,if a NULL is passed though the Method
#region DefaultAnimationCurves

public AnimationCurve defaultAnimationCurve_Radius()
{
    AnimationCurve ac = new AnimationCurve();
    ac.AddKey(0f,0f);
    ac.AddKey(1f,1f);

    return ac;
}

public AnimationCurve defaultAnimationCurve_Amplitude()
{
    AnimationCurve ac = new AnimationCurve();
    ac.AddKey(0f,1f);
    ac.AddKey(1f,0f);

    return ac;
}

public AnimationCurve defaultAnimationCurve_WaveSize()
{
    AnimationCurve ac = new AnimationCurve();
    ac.AddKey(0f,0.2f);
    ac.AddKey(1f,0.2f);

    return ac;
}

#endregion

//These are the Methods used to pause the ShockWave(s).
#region PauseControl
    //whether or not the ShockWave is Paused
    private bool Paused = false;

    //Pause/UnPause Shockwave
    public void PauseToggle()
    {
        Paused = !Paused;
    }

    //Pause the ShockWave
    public void Pause()
    {
        Paused = true;
    }

    //UnPause the ShockWave
    public void UnPause()
    {
        Paused = false;
    }

    //Pauses All ShockWaves that exist
    static public void AllPause()
    {
        ShockWave[] AllShockWaves = GameObject.FindObjectsOfType<ShockWave>();

        for (int i =0 ; i < AllShockWaves.Length; i++)
        {
            AllShockWaves[i].Pause();
        }
    }

    //UnPauses All ShockWaves that exist
    static public void AllUnPause()
    {
        ShockWave[] AllShockWaves = GameObject.FindObjectsOfType<ShockWave>();

        for (int i =0 ; i < AllShockWaves.Length; i++)
        {
            AllShockWaves[i].UnPause();
        }
    }
#endregion


    /// <summary>
    /// Destories all ShockWaves
    /// </summary>
    static public void DestoryAll()
    {
        ShockWave[] AllShockWaves = GameObject.FindObjectsOfType<ShockWave>();

        for (int i =0 ; i < AllShockWaves.Length; i++)
        {
            Destroy(AllShockWaves[i]);
        }
    }

    /// <summary>
    /// Raises the render image event.
    /// </summary>
    /// <param name="src">Source.</param>
    /// <param name="dest">Destination.</param>
    void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        if (mat == null)
        {
            return;
        }

        if(mat != null)
        {
            
        }

        Graphics.Blit(src, dest, mat);
    }


}

