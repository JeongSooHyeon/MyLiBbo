using UnityEngine;

public class ChargeManager : MonoBehaviour
{
    public static ChargeManager intance;

    [SerializeField]
    TweenPosition buyPop = null,buyPopbg = null, resultbuyPop = null, buypopBG = null;
    [SerializeField]
    TweenScale buypopScale_, buyPopScale;
    [SerializeField]
    public GameObject coinEffect;
    int money;

    void Awake()
    {
        intance = this;
    }

    public void BuyTween(bool istrue, int num = -1)
    {
        money = num;
        if (istrue)
        {
            buyPop.PlayForward();
            buyPopbg.PlayForward();
            if (buyPopScale != null)
            {
                buyPopScale.onFinished.Clear();
                buyPop.onFinished.Clear();
                buyPop.onFinished.Add(new EventDelegate(buyPopScale.PlayForward));
            }
        }
        else
        {
            buyPopbg.PlayReverse();
            if (buyPopScale != null)
            {
                buyPopScale.onFinished.Clear();
                buyPop.onFinished.Clear();
                buyPopScale.PlayReverse();
                buyPopScale.onFinished.Add(new EventDelegate(buyPop.PlayReverse));
            }
        }
    }

    public void ReBuyTween(bool istrue, int num = -1)
    {
        money = num;
        if (istrue)
        {
            GaChaManager.instance.DrawBallPOP_(false);
            resultbuyPop.PlayForward();
            buypopBG.PlayForward();
            if (buypopScale_ != null)
            {
                buypopScale_.onFinished.Clear();
                resultbuyPop.onFinished.Clear();
                resultbuyPop.onFinished.Add(new EventDelegate(buypopScale_.PlayForward));
            }
        }
        else
        {
            buypopBG.PlayReverse();
            if (buypopScale_ != null)
            {
                buypopScale_.onFinished.Clear();
                resultbuyPop.onFinished.Clear();
                buypopScale_.PlayReverse();
                buypopScale_.onFinished.Add(new EventDelegate(resultbuyPop.PlayReverse));
            }
        }
    }

    public void BuySelec()
    {
        if(money > -1)
        {

        }
    }

    public void ReBuySelec()
    {
        if (money > -1)
        {

        }
    }
    public void CoinEffect()
    {
        coinEffect.gameObject.SetActive(true);
        SoundManager.instance.ChangeEffects(8);
        Invoke("CoinEffectoff", 3);
    }
    void CoinEffectoff()
    {
        coinEffect.gameObject.SetActive(false);
    }
}
