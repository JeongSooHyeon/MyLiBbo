using UnityEngine;

public class chargeBtnClick : MonoBehaviour
{
    [SerializeField] UILabel myChargeCoin;
    [SerializeField] UILabel[] ItemCountArr;
    [SerializeField] UILabel ChargeText;
    [SerializeField] GameObject[] CharObjArrs;
    [SerializeField] bool isPackage;
    public int pId;
    public int chargeCount;
    public int chargeCount_2;

    public void SetData()
    {
        if (isPackage)
        {
            myChargeCoin.text = string.Format("{0}", chargeCount);
            CharObjArrs[0].SetActive(false);
            int item_1 = DataManager.instance.ChargeJewelPackageList[pId].InstanceItem_1, item_2 = DataManager.instance.ChargeJewelPackageList[pId].InstanceItem_2;
            CharObjArrs[0].SetActive(true);
            ItemCountArr[0].text = string.Format("x{0}", item_1);
            ItemCountArr[1].text = string.Format("x{0}", item_2);
            ChargeText.text = DataManager.instance.GetProductPrice(pId + 7);
        }
        else
        {
            ChargeText.text = DataManager.instance.GetProductPrice(pId);
        }
        
    }

    void OnClick()
    {
        if (!isPackage)
            LobbyManager.instance.CallCommonPup((int)CommonState.buyCoin, pId, chargeCount);
        else
            LobbyManager.instance.CallCommonPup((int)CommonState.buyPackage, pId + 7, pId);
    }
}
