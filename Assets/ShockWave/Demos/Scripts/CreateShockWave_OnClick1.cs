//This script is Just used for Demo1.
//this script is pretty basic...so there is not a lot of comments.



using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateShockWave_OnClick1 : MonoBehaviour {

    //values
    private float MaxRadius;
    private float Speed;
    private float Amp;
    private float WS;
    private bool RevSW;

    //sliders
    public Slider MaxRadiusSlider;
    public Slider SpeedSlider;
    public Slider AmplitudeSlider;
    public Slider WaveSizeSlider;

    //texts
    public Text MaxRadiusText;
    public Text SpeedText;
    public Text AmplitudeText;
    public Text WaveSizeText;

    //reverse toggle
    public Toggle ReverseShockWave;

    public Camera thisCamera;

    //setting variables
    void FixedUpdate()
    {
        //get the default values
        MaxRadius = MaxRadiusSlider.value;
        Speed = SpeedSlider.value;
        Amp = AmplitudeSlider.value;
        WS = WaveSizeSlider.value;
        RevSW = ReverseShockWave.isOn;

        //set the text
        MaxRadiusText.text = "MaxRadius: " + (Mathf.Round(MaxRadius * 1000f) / 1000f).ToString();
        SpeedText.text = "Speed: " + (Mathf.Round(Speed * 1000f) / 1000f).ToString();
        AmplitudeText.text = "Amplitude: " + (Mathf.Round(Amp * 1000) / 1000f).ToString();
        WaveSizeText.text = "WaveSize: " + (Mathf.Round(WS * 1000) / 1000f).ToString();
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RevSW)
            {
                //both of these are valid...
                //ShockWave.Get().ReverseIt(Input.mousePosition,true,Speed,MaxRadius, Amp ,WS); //if you want to use the main camera you don't need to specify a camera
                ShockWave.Get(thisCamera).ReverseIt(Input.mousePosition, true, Speed, MaxRadius, Amp, WS);

            }
            else
            {
                //both of these are valid...
                //ShockWave.Get().StartIt(Input.mousePosition,true,Speed,MaxRadius, Amp, WS); //if you want to use the main camera you don't need to specify a camera
                ShockWave.Get(thisCamera).StartIt(Input.mousePosition, true, Speed, MaxRadius, Amp, WS);
            }
        }
	}

    public void PauseShockWaves()
    {
        ShockWave.AllPause();
    }

    public void UnPauseShockWaves()
    {
        ShockWave.AllUnPause();
    }

    public void DestoryAll()
    {
        ShockWave.DestoryAll();
    }

}
