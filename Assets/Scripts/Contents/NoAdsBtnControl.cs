using UnityEngine;

public class NoAdsBtnControl : MonoBehaviour
{
    [SerializeField] GameObject[] btnStateObjs;
    [SerializeField] BoxCollider2D myBtn;
    [SerializeField] UILabel ChargeText;

    public void SetData(bool isUse)
    {
        for (int i = 0; i < btnStateObjs.Length; ++i)
        {
            btnStateObjs[i].SetActive(false);
        }
        myBtn.enabled = !(isUse);
        int btnIdx = isUse ? 1 : 0;
        btnStateObjs[btnIdx].SetActive(true);
        ChargeText.text = DataManager.instance.GetProductPrice(12);
    }
}
