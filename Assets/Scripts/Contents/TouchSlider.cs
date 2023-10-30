using UnityEngine;

public class TouchSlider : MonoBehaviour
{
    float sValue;
    bool isFire;
   void OnPress(bool isTrue)
   {
        isFire = isTrue;
        if (!isTrue)
        {
            BallManager.instance.SetFire();
            BallManager.instance.SliderPos(isTrue, sValue);
        }
    }
    public void SliderValue()
    {
        if(isFire)
        {
            sValue = UIProgressBar.current.value;
            BallManager.instance.SliderPos(isFire, sValue);
        }
      
    }
}
