using UnityEngine;

public class PuzzleSetControl : MonoBehaviour
{
    [SerializeField] GameObject[] housingStateObjs;
    [SerializeField] GameObject[] housingInfoObjs;
    [SerializeField] UISprite[] HousingPiecelist;
    [SerializeField] GameObject housingRewardInfo;
    [SerializeField] UILabel RewardCount;
    public int myHousingIdx;
    public HousingState housingState;
    bool isInfoView = false;
    void OnClick()
    {
        SoundManager.instance.TapSound();
    }

    void ResetData()
    {
        for(int i=0; i< housingStateObjs.Length; ++i)
        {
            housingStateObjs[i].SetActive(false);
        }
        isInfoView = false;
        housingRewardInfo.SetActive(false);
        housingInfoObjs[1].SetActive(false);
    }

    public void SetData()
    {
        ResetData();
        housingState = DataManager.instance.GetHousingState(myHousingIdx);
        int pNum = 0;
        switch (housingState)
        {
            case HousingState.collect:
                housingStateObjs[1].SetActive(true);
                pNum = DataManager.instance.GetHousingStateList(myHousingIdx) > 0 ? DataManager.instance.GetHousingStateList(myHousingIdx) : 0;
                RewardCount.text = string.Format("{0} / 10", pNum);
                break;
            case HousingState.getreward:
                pNum = 10;
                housingStateObjs[2].SetActive(true);
                break;
            case HousingState.complete:
                pNum = 10;
                housingStateObjs[3].SetActive(true);
                break;
            case HousingState.none:
                housingStateObjs[0].SetActive(true);
                break;
        }

        for (int i = 0; i < 10; ++i)
        {
            StageStarState data = DataManager.instance.GetChapterStateList((myHousingIdx * 10) + i);
            if(data == StageStarState.complete)
                HousingPiecelist[i].enabled = false;
            else
                HousingPiecelist[i].enabled = true;
        }
    }
    
    public void ClickInfoBtn()
    {
        if (isInfoView) return;
        SoundManager.instance.TapSound();
        housingRewardInfo.SetActive(true);
        housingInfoObjs[1].SetActive(true);
        isInfoView = true;
        Invoke("CloseInfoView", 1f);
    }

    void CloseInfoView()
    {
        CancelInvoke("CloseInfoView");
        isInfoView = false;
        housingRewardInfo.SetActive(false);
        housingInfoObjs[1].SetActive(false);
    }
}
