using UnityEngine;

public class HousingSetControl : MonoBehaviour
{
    public HousingObjectControl[] HousingObjects;
    public GameObject fin_obj;
    [SerializeField] UILabel RewardCount;
    public int myHousingIdx;
    public HousingState state;
    public int hIdx;
    public UISlider prograss;
    public static int housingCount;
    [SerializeField] GameObject lockObj;
    [SerializeField] GameObject noticeObj;

    void ResetData()
    {
        for (int i = 0; i < HousingObjects.Length; i++)
            HousingObjects[i].CheckObj();
        if (hIdx <= 10 && hIdx >= 0)
            RewardCount.text = string.Format("{0} / {1}", hIdx, HousingObjects.Length);
        else if (hIdx < 0) RewardCount.text = string.Format("0 / {0}", HousingObjects.Length);
        else RewardCount.text = string.Format("{0} / {0}", HousingObjects.Length);
        prograss.value = hIdx * 0.1f;
        DataManager.instance.SetHousingStateList(myHousingIdx, hIdx);
    }

    public void SetData()
    {
        ResetData();
        state = DataManager.instance.GetHousingState(myHousingIdx);
        fin_obj.SetActive(state == HousingState.complete);
        switch (state)
        {
            case HousingState.collect:
                if (hIdx >= HousingObjects.Length)
                {
                    state = HousingState.getreward;
                    if (myHousingIdx + 1 < housingCount)
                    DataManager.instance.SetHousingStateList(myHousingIdx + 1, 0);
                }
                break;
            case HousingState.getreward:
                break;
            case HousingState.complete:
                break;
            case HousingState.none:
                break;
        }
        SetLock();
        SetNotice();
    }

    public void SetLock()
    {
        if (myHousingIdx != 0)
            lockObj.SetActive(DataManager.instance.curStageIdx() <= 200 * myHousingIdx);
    }

    public void SetNotice()
    {
        if (myHousingIdx != 0)
            noticeObj.SetActive(hIdx <= 0 && !lockObj.activeSelf);
        else noticeObj.SetActive(hIdx <= 0);
    }
}
