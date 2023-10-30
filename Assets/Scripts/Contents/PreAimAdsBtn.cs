using UnityEngine;

public class PreAimAdsBtn : MonoBehaviour
{
    [SerializeField] GameObject[] BtnStateObjs;
    [SerializeField] BoxCollider2D myBtn;
    [SerializeField] UILabel ChargeText;

    void Start()
    {
        if(DataManager.instance.SweetAimUse)
        {
            ResetAimBtn();
        }
        else
        {
            SetData(DataManager.instance.SweetAimPreUse);
        }
    }

    public void ResetAimBtn()
    {
        for (int i = 0; i < BtnStateObjs.Length; ++i)
        {
            BtnStateObjs[i].SetActive(false);
        }
        BtnStateObjs[0].SetActive(true);
        myBtn.enabled = false;
    }

    public void SetData(bool isUse)
    {
        for(int i=0; i< BtnStateObjs.Length; ++i)
        {
            BtnStateObjs[i].SetActive(false);
        }
        myBtn.enabled = !(isUse);
        int btnIdx = isUse ? 1 : 0;
        BtnStateObjs[2].SetActive(!isUse);
        BtnStateObjs[btnIdx].SetActive(true);
        ChargeText.text = DataManager.instance.GetProductPrice(6);
    }
    public void ClickPreSweetAim()
    {
        if (!DataManager.instance.SweetAimUse)
        {
            LobbyManager.instance.CallCommonPup((int)CommonState.SweetAimPre);
        }
    }
}
