using UnityEngine;
using System.Collections.Generic;

public class GaChaManager : MonoBehaviour
{
    public static GaChaManager instance;
    public List<GaChaBall> gaChaBall;
    GameObject getBall;
    [SerializeField]
    UILabel ballName_;
    [SerializeField]
    TweenPosition getballpop = null, downBall_ = null, noBallPop_ = null, drawBallBG_ = null, checkPop = null, noBallBG = null, getballBG = null;
    [SerializeField]
    TweenScale drawBallScale_, buyPopScale_, noballScale_, getballScale_;
    [SerializeField]
    TweenRotation stick_;
    [SerializeField]
    UISprite drawBallpop, noball = null, gachaBall_ = null, getDownBall_ = null;
    [SerializeField]
    UIButton getButton_;
    int ballnum = 0;
    [SerializeField]
    GameObject[] gachaBundle;
    void Awake()
    {
        instance = this;
    }

    public void gaChaBundle()
    {
        int idx = Random.Range(0, 2);
        gachaBundle[idx].SetActive(true);
    }

    public void DrawKick()
    {
        if (DataManager.instance.GetCoin() >= 5)
        {
            DataManager.instance.SetCoin(-5);
            UIManager.instance.SetMoney();
            KickPlay(1.5f);
            GaChaGet();
        }
        else
        {
            noball.spriteName = "Title_Money";
            DrawBallPOP_(true);
        }
    }

    void KickPlay(float power)
    {
        GaChaGet();
        for (int i = 0; i < gaChaBall.Count; ++i)
        {
            int iDX = Random.Range(-3, 4);
            gaChaBall[i].GetComponent<GaChaBall>().rd.AddForce(new Vector2(iDX, power), ForceMode2D.Impulse);
        }
    }

    public void BallDraw()
    {
        if (getBall != null)
        {
            if (DataManager.instance.GetCoin() >= 100)
            {
                if (getBall.CompareTag("GaChaLD")) GetTheBall(0);
                if (getBall.CompareTag("GaChaNM")) GetTheBall(1);
                if (getBall.CompareTag("GaChaUQ")) GetTheBall(2);

                if (getBall.CompareTag("GaChaNM")) getDownBall_.spriteName = "Normal_Ball";
                else getDownBall_.spriteName = "Legend_Ball";
                getBall.SetActive(false);
                GaChaGet();
                Rotation_();
                GetButtonOff(false);
                downBall_.transform.localPosition = new Vector2(0, -190);
                DataManager.instance.SetCoin(-100);
                UIManager.instance.SetMoney();
            }
            else
            {
                noball.spriteName = "Title_Money";
                DrawBallPOP_(true);
            }
        }
        else
        {
            noballPOP(true);
        }
    }

    public void DrawBallPOP_(bool isPlay)
    {
        if (isPlay)
        {
            checkPop.PlayForward();
            drawBallBG_.PlayForward();
            if (drawBallScale_ != null)
            {
                drawBallScale_.onFinished.Clear();
                checkPop.onFinished.Clear();
                checkPop.onFinished.Add(new EventDelegate(drawBallScale_.PlayForward));
            }
        }
        else
        {
            drawBallBG_.PlayReverse();
            if (drawBallScale_ != null)
            {
                drawBallScale_.onFinished.Clear();
                checkPop.onFinished.Clear();
                drawBallScale_.PlayReverse();
                drawBallScale_.onFinished.Add(new EventDelegate(checkPop.PlayReverse));
            }
        }
    } 

    public void GetButtonOff(bool isbool)
    {
        if(isbool) getButton_.isEnabled = true;
        else getButton_.isEnabled = false;
    }

    public void noballPOP(bool isPlay)
    {
        if (isPlay)
        {
            noBallPop_.PlayForward();
            noBallBG.PlayForward();
            if (noballScale_ != null)
            {
                noballScale_.onFinished.Clear();
                noBallPop_.onFinished.Clear();
                noBallPop_.onFinished.Add(new EventDelegate(noballScale_.PlayForward));
            }
        }
        else
        {
            noBallBG.PlayReverse();
            if (noballScale_ != null)
            {
                noballScale_.onFinished.Clear();
                noBallPop_.onFinished.Clear();
                noballScale_.PlayReverse();
                noballScale_.onFinished.Add(new EventDelegate(noBallPop_.PlayReverse));
            }
        }
    }

    public void GetballInfoOn()
    {
        SoundManager.instance.ChangeEffects(7);
        getballpop.PlayForward();
        getballBG.PlayForward();
        if (getballScale_ != null)
        {
            getballScale_.onFinished.Clear();
            getballpop.onFinished.Clear();
            getballpop.onFinished.Add(new EventDelegate(getballScale_.PlayForward));
        }
    }

    public void GetballInfoOff()
    {
        getballBG.PlayReverse();
        if (getballScale_ != null)
        {
            getballScale_.onFinished.Clear();
            getballpop.onFinished.Clear();
            getballScale_.PlayReverse();
            getballScale_.onFinished.Add(new EventDelegate(getballpop.PlayReverse));
        }
        KickPlay(3);
        GetButtonOff(true);
        downBall_.transform.localPosition = new Vector2(-10000, -10000);
    }

    public void GetTheBall(int i)
    {
        switch (i)
        {
            case 0:
                ballnum = Random.Range(18, 20);
                drawBallpop.spriteName = "Legend";
                break;
            case 1:
                ballnum = Random.Range(1, 13);
                drawBallpop.spriteName = "Normal";
                break;
            case 2:
                ballnum = Random.Range(13, 18);
                drawBallpop.spriteName = "Unique";
                break;
        }
        ballName_.text = BallDataManager.instance.BallDataList[ballnum].bTag;
        gachaBall_.spriteName = BallDataManager.instance.BallDataList[ballnum].bName;
    }

    public void Rotation_()
    {
        SoundManager.instance.ChangeEffects(6);
        stick_.PlayForward();
        stick_.ResetToBeginning();
        Invoke("downBall", 0.5f);
    }

    public void downBall()
    {
        stick_.transform.localEulerAngles = Vector3.zero;
        downBall_.PlayForward();
        downBall_.ResetToBeginning();
        Invoke("GetballInfoOn", 1);
    }

    public void GaChaGet(GameObject go = null)
    {
        getBall = go;
    }
}
