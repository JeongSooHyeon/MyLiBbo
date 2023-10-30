using UnityEngine;

public class ShopControl : MonoBehaviour {
    [SerializeField] chargeBtnClick[] JewelBtnArr;
    [SerializeField] chargeBtnClick[] PackageBtnArr;
    [SerializeField] PreAimAdsBtn preAimBtn;
    [SerializeField] NoAdsBtnControl noAdsBtn;
    [SerializeField] UIScrollView pachageScrollView;
    [SerializeField] UIScrollView jewelScrollView;
    [SerializeField] UIToggle myJewelBtn;
    [SerializeField] TextAsset PriceDatas;
    string[] IOSPriceArr = new string[13];

    void Awake()
    {
        string[] lines = PriceDatas.text.Split('\n');
        if (lines.Length == 0)
            Debug.Log("text data is nothing!!");
        else
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                string[] txtD = lines[i].Split(',');
                IOSPriceArr[i] = txtD[0].Replace("\r", "");
            }
        }

#if UNITY_IOS
        for (int i = 0; i < ShopPriceArr.Length; ++i)
        {
            ShopPriceArr[i].SetTerm(IOSPriceArr[i]);
        }
#endif
    }

    void Start () {
        initShop();
        ChangeAimState(DataManager.instance.SweetAimUse);
    }
	
    void initShop()
    {
        for (int i = 0; i < JewelBtnArr.Length; ++i)
        {
            JewelBtnArr[i].pId = i;
            JewelBtnArr[i].chargeCount = DataManager.instance.ChargeJewelList[i];
            JewelBtnArr[i].SetData();
        }

        for (int i = 0; i < PackageBtnArr.Length; ++i)
        {
            PackageBtnArr[i].pId = i;
            PackageBtnArr[i].chargeCount = DataManager.instance.ChargeJewelPackageList[i].jewelCount;
            PackageBtnArr[i].chargeCount_2 = DataManager.instance.ChargeJewelPackageList[i].cJewelCount;
            PackageBtnArr[i].SetData();
        }

        noAdsBtn.SetData(DataManager.instance.NoAdsUse);
    }

    public void ChangeAimState(bool isUse)
    {
        preAimBtn.SetData(isUse);
    }

    public void ChangeNoAdsState(bool isUse)
    {
        noAdsBtn.SetData(isUse);
    }

    public void ClickBuyAim()
    {
        LobbyManager.instance.CallCommonPup((int)CommonState.SweetAimBuy);
    }

    public void ClickBuyNoads()
    {
        LobbyManager.instance.CallCommonPup((int)CommonState.NoadsBuy);
    }

    public void SetPackageBtnData(int pid)
    {
        PackageBtnArr[pid].SetData();
    }

    public void SetPackageBtnDataCatId(int cid)
    {
        for(int i=0; i< PackageBtnArr.Length; ++i)
        {
            if(DataManager.instance.ChargeJewelPackageList[i].cId == cid)
                PackageBtnArr[i].SetData();
        }
    }
    public void CallBtnSound()
    {
        if (LobbyManager.instance.state == LobbyState.Shop)
            SoundManager.instance.TapSound();
    }

    public void ResetScrollPos()
    { 
        if (UIToggle.current.value)
        {
            jewelScrollView.SetDragAmount(0, 0, false);
            pachageScrollView.SetDragAmount(0, 0, false);
        }
        ChangeAimState(DataManager.instance.SweetAimUse);
    }

    public void CallJewelChargeView()
    {
        myJewelBtn.value = true;
    }
}
