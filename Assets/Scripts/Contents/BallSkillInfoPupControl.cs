using UnityEngine;

public class BallSkillInfoPupControl : PopupBase
{
    [SerializeField] GameObject[] myStateBtnObj;
    [SerializeField] UILabel myPriceTxt;
    [SerializeField] GameObject[] ballPrefabs;
    [SerializeField] GameObject[] AdsBtns;
    [SerializeField] Vector3[] AdsBtnPos;
    public int bIdx;
    public ShopState myState;
    public GameObject myContents;
    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        if (myContents != null) Destroy(myContents);
        LobbyManager.instance.curPopup = this;
        SetBallState();
        
        string priceNum = "";

        myContents = Instantiate(ballPrefabs[bIdx], transform);
        priceNum = string.Format("{0:N0}", CharBallDataManager.instance.GetCBData(bIdx).price);
        myContents.transform.localScale = Vector3.one;
        myContents.GetComponent<BallNCharAniControl>().SetPlay();
        int posIdx = (priceNum == "0") ? 1 : 0;
        if (myState == ShopState.Buy)
        {
            myPriceTxt.text = priceNum;
            if(priceNum == "0")
                myStateBtnObj[0].SetActive(false);
        }
        AdsBtns[0].transform.localPosition = AdsBtnPos[posIdx];
        AdsBtns[1].transform.localPosition = AdsBtnPos[posIdx];
    }

    public void SetBallState()
    {
        for (int i = 0; i < myStateBtnObj.Length; ++i)
        {
            myStateBtnObj[i].SetActive(false);
        }
        if(myState == ShopState.PreUse)
            myStateBtnObj[2].SetActive(true);
        else
            myStateBtnObj[(int)myState].SetActive(true);

        AdsBtns[0].SetActive(myState == ShopState.Buy && DataManager.instance.GetCharBallPreViewList(bIdx) == 0);
        AdsBtns[1].SetActive(myState == ShopState.Buy && DataManager.instance.GetCharBallPreViewList(bIdx) != 0);
    }

    public void ClickBuyBtn()
    {
        SoundManager.instance.TapSound();

        int priceNum = 0, commonState = 0;

        priceNum = CharBallDataManager.instance.GetCBData(bIdx).price;
        commonState = (int)CommonState.buyBall;

        if (DataManager.instance.GetCoin() >= priceNum)
        {
            LobbyManager.instance.CallCommonPup(commonState, priceNum, bIdx);
        }
        else
        {
            ClosePupTPTS();
            LobbyManager.instance.CallCommonPup((int)CommonState.coinLack);
        }   
    }

    public void ClickUseBtn()
    {
        SoundManager.instance.TapSound();

        if (DataManager.instance.PreBallSprite > 0)
        {
            ClosePupTPTS();
            return;
        }

        LobbyManager.instance.SetBallSelect(bIdx);
  
        myState = ShopState.Use;
        for (int i = 0; i < myStateBtnObj.Length; ++i)
        {
            myStateBtnObj[i].SetActive(false);
        }
        myStateBtnObj[(int)myState].SetActive(true);
    }

    public override void ClosePupTPTS()
    {
        SoundManager.instance.TapSound();
        base.ClosePupTPTS();
        Destroy(myContents);
        LobbyManager.instance.curPopup = null;
    }

    public void ClickPreCharBall()
    {
        LobbyManager.instance.CallCommonPup((int)CommonState.PreBall, bIdx);
    }
}
