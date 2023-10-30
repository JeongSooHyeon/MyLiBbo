using UnityEngine;

public class PreloaderAnimEnd : MonoBehaviour
{
    private void Start()
    {
        Invoke("NextStep", 3f);
    }

    public void NextStep()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            PreloadControl.instance.CheckLongTimeReward();
        }
        else
        {
            //if (Application.isEditor)
            //    PreloadControl.instance.CheckLongTimeReward();
            //else if (PlayerPrefs.HasKey("GoogleLogin_"))
            //{
            //    if (!PlayerPrefsElite.GetBoolean("GoogleLogin_"))
            //        PreloadControl.instance.CheckLongTimeReward();
            //}
            PreloadControl.instance.CheckLongTimeReward();
        }
    }
}
