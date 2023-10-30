using UnityEngine;

public class BallNCharAniControl : MonoBehaviour
{
    public Animator myAni;
    public string aniPara;

    public void SetPlay()
    {
        myAni.SetTrigger(aniPara);
    }
}
