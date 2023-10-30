using UnityEngine;

public class PuzzleSetItemControl : MonoBehaviour
{
    [SerializeField] HousingSetControl[] HousingList;
    [SerializeField] GameObject[] btns;
    [SerializeField] GameObject[] test_btns;
    public int idx;
    public HousingState state;
    [HideInInspector] public int rewardCount;

    void Start()
    {
        test_btns[0].SetActive(DataManager.instance.isTestMode);
        idx = DataManager.instance.HousingStateList.FindLastIndex(t => t >= 0);
        initData();
        BtnSet();
        StateCheck();
        NewSignCheck();
        HousingSetControl.housingCount = HousingList.Length;
    }

    public void initData()
    {
        for(int i=0; i< HousingList.Length; ++i)
        {
            HousingList[i].gameObject.SetActive(false);
            HousingList[i].myHousingIdx = i;
            HousingList[i].hIdx = DataManager.instance.GetHousingStateList(i);
            HousingList[i].SetData();
            BtnSet();
        }
        HousingList[idx].gameObject.SetActive(true);
    }

    public void ClickOpen()
    {
        if (HousingList[idx].hIdx < HousingList[idx].HousingObjects.Length)
        {
            HousingList[idx].hIdx++;
        }
        HousingList[idx].SetData();
        BtnSet();
        StateCheck();
    }

    public void ClickReset()
    {
        idx = 0;
        DataManager.instance.SetHousingStateList(idx, 0);
        for (int i = 0; i < DataManager.instance.stageChapterStateList.Count; i++)
        {
            if (DataManager.instance.GetChapterStateList(i) == StageStarState.complete)
                DataManager.instance.SetChapterStateList(i, 1);
        }
        for (int i = 1; i < DataManager.instance.HousingStateList.Count; i++)
        {
            DataManager.instance.SetHousingStateList(i, -1);
        }
        initData();
        BtnSet();
    }

    public void ClickPrev()
    {
        idx--;
        initData();
        BtnSet();
        StateCheck();
        SoundManager.instance.TapSound();
    }

    public void ClickNext()
    {
        idx++;
        initData();
        BtnSet();
        StateCheck();
        SoundManager.instance.TapSound();
    }

    public void BtnSet()
    {
        btns[1].SetActive(idx != HousingList.Length - 1);
        btns[0].SetActive(idx > 0);
    }

    public void StateCheck()
    {
        state = HousingList[idx].state;
        if (state == HousingState.getreward)
        {
            if (LobbyManager.instance.state == LobbyState.Housing)
                LobbyManager.instance.OpenHousing(idx);
        }
    }

    public void NewSignCheck()
    {
        rewardCount = 0;
        
        for (int i = 0; i < HousingList.Length; i++)
        {
            if (HousingList[i].state == HousingState.getreward)
                rewardCount++;
        }

        LobbyManager.instance.SetHousingNewSign(rewardCount != 0);
    }
}
