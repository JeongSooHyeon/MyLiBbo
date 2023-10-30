using UnityEngine;
using System.Collections;

public class LobbyTween : MonoBehaviour
{
    public UIState state_;
    Animator anim_;
    public bool isAnimTween;
    TweenScale ts_;
    void Start()
    {
        anim_ = GetComponent<Animator>();
        if(anim_ != null) isAnimTween = true;
    }

    private void OnEnable() {
        if(ts_ == null) ts_ = GetComponent<TweenScale>();
        if(ts_!= null)
        {
            ts_.ResetToBeginning();
            ts_.PlayForward();

            StartCoroutine("DelayTSPlay", ts_.duration + 0.05f);
        }
    }

    public void SetAnimPlay(bool isTrue)
    {
        if(anim_ != null)
        {
            if(isTrue) anim_.SetTrigger("On");
            else anim_.SetTrigger("Off");
        }
    }
    
    IEnumerator DelayTSPlay(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        if (transform.localScale.x < 1 && ts_ != null)
        {
            ts_.ResetToBeginning();
            ts_.PlayForward();
        }
    }
}
