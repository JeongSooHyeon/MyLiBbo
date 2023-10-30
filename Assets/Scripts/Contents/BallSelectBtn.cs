using UnityEngine;

public class BallSelectBtn : MonoBehaviour {
   
    public int myIdx;
    [SerializeField] UILabel myPrice;
    [SerializeField] GameObject[] BtnObjSets;
    [SerializeField] GameObject AdsBtn;
    public BallViewControl ballView;
    public ShopState myState;

    private void Start()
    {
        SetData();
    }

    void OnClick()
    {
        SoundManager.instance.TapSound();
        switch (myState)
        {
            case ShopState.Buy:
            case ShopState.UnUse:
            case ShopState.Use:
                ballView.BallSkillCall(myIdx, myState);
                break;
            case ShopState.PreUse:
                ballView.BallSkillCall(myIdx, ShopState.PreUse);
                break;
        }
    }

    public void SetSelectBtn(int state)
    {
        ResetBtnState();
        myState = (ShopState)state;
        if (myState == ShopState.PreUse)
            BtnObjSets[2].SetActive(true);
        else
            BtnObjSets[state].SetActive(true);

        if (DataManager.instance.GetCharBallList(myIdx) == (int)ShopState.Buy)
            AdsBtn.SetActive(DataManager.instance.GetCharBallPreViewList(myIdx) < 1);
        else
            AdsBtn.SetActive(false);   
    }

    public void SetData()
    {
        SetSelectBtn((int)myState);

        int priceNum = 0, houLockNum = 0;
        string spriteName = "";

        priceNum = CharBallDataManager.instance.GetCBData(myIdx).price;
        houLockNum = CharBallDataManager.instance.GetCBData(myIdx).housingLock;
        spriteName = CharBallDataManager.instance.GetCBData(myIdx).spriteName;
        if (houLockNum == 0)
            myPrice.text = string.Format("{0:N0}", priceNum);
    }

    public void ResetBtnState()
    {
        for (int i=0; i< BtnObjSets.Length; ++i)
        {
            BtnObjSets[i].SetActive(false);
        }
    }

    public void SelectBallBtn()
    {
        SetSelectBtn((int)ShopState.Use);
    }

    public void ClickPreCharBall()
    {
        LobbyManager.instance.CallCommonPup((int)CommonState.PreBall, myIdx);
    }
}
