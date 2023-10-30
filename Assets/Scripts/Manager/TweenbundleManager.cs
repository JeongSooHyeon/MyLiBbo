using UnityEngine;

public class TweenbundleManager : MonoBehaviour
{
    [SerializeField] LobbyTween[] bundleObj;
    [SerializeField] GameObject backBG;
    //[SerializeField] TweenPosition backBG2;

    public void ShowBundleObject(UIState state_, bool isTrue)
    {
        if (state_ != UIState.Tutorial && state_ != UIState.ShootingTuto) backBG.SetActive(isTrue);
        for (int i = 0; i < bundleObj.Length; ++i)
        {
            if (bundleObj[i].state_ == state_)
            {
                if (bundleObj[i].isAnimTween) bundleObj[i].SetAnimPlay(isTrue);
                else bundleObj[i].gameObject.SetActive(isTrue);
            }
        }
    }
}
