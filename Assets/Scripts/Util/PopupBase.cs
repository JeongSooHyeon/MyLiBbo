using UnityEngine;

public class PopupBase : MonoBehaviour
{
    public TweenPosition BackBg;
    public TweenPosition myTp;
    public TweenScale myTs;
    public AnimationCurve[] TPAnimaCurves;
    public PopupBase myType;
    public PopupBGClick BGClick;
    public PopupType myTypeEnum;

    public virtual void CallPup()
    {
        SoundManager.instance.TapSound(0);
        myTp.animationCurve = TPAnimaCurves[0];
        myTp.PlayForward();
        if (isBackBgEmpty())
        {
            BackBg.PlayForward();
            BGClick.CurPopup = this;
        }

    }

    public virtual void ClosePup()
    {
        SoundManager.instance.TapSound(0);
        myTp.animationCurve = TPAnimaCurves[1];
        myTp.PlayReverse();
        if (isBackBgEmpty())
        {
            BackBg.PlayReverse();
            BGClick.CurPopup = null;
        }
    }

    public virtual void CallPupTPTS()
    {
        SoundManager.instance.TapSound(0);
        myTs.animationCurve = TPAnimaCurves[0];
        myTp.PlayForward();
        myTs.PlayForward();
        if (isBackBgEmpty())
        {
            BackBg.PlayForward();
            BGClick.CurPopup = this;
        }
    }

    public virtual void ClosePupTPTS()
    {
        SoundManager.instance.TapSound(0);
        myTs.animationCurve = TPAnimaCurves[1];
        myTp.PlayReverse();
        myTs.PlayReverse();
        if(isBackBgEmpty())
        {
            BackBg.PlayReverse();
            BGClick.CurPopup = null;
        }
    }

    public virtual void CallPupTS()
    {
        SoundManager.instance.TapSound(0);
        myTs.animationCurve = TPAnimaCurves[0];
        myTs.PlayForward();
    }

    public virtual void ClosePupTS()
    {
        SoundManager.instance.TapSound(0);
        myTs.animationCurve = TPAnimaCurves[1];
        myTs.PlayReverse();
    }

    bool isBackBgEmpty()
    {
        bool ret = true;
        if (myTypeEnum == PopupType.option)
            ret = false;
         if (myTypeEnum == PopupType.loading)
            ret = false;
        if(myTypeEnum == PopupType.tuto)
            ret = false;
        if (myTypeEnum == PopupType.lobbyTip)
            ret = false;
        if (myTypeEnum == PopupType.common)
            ret = false;
        if (myTypeEnum == PopupType.emptyRewardAd)
            ret = false;
        if (myTypeEnum == PopupType.dailyReward)
            ret = false;
        if (myTypeEnum == PopupType.dailyNotice)
            ret = false;
        if (myTypeEnum == PopupType.getDailyReward)
            ret = false;
        if (myTypeEnum == PopupType.housingReward)
            ret = false; 
        if (myTypeEnum == PopupType.stageReward)
            ret = false;
        return ret; 
    }
}
