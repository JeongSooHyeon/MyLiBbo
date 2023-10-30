using UnityEngine;

public class BallViewControl : MonoBehaviour
{
    [SerializeField] BallSkillInfoPupControl ballSkill;
    [SerializeField] BallSelectBtn[] ballBtnArr;
    [SerializeField] UIScrollView myPanel;
    [SerializeField] UIToggle myToggle;

    // Start is called before the first frame update
    void Start()
    {
        initList();
    }

    void initList()
    {
        if (DataManager.instance.PreBallSprite > 0)
        {
            DataManager.instance.SetCharBallList(DataManager.instance.BallSprite, (int)ShopState.UnUse);
            ResetBallSelectBtns(DataManager.instance.PreBallSprite, true);
        }
        else
        {
            DataManager.instance.SetCharBallList(DataManager.instance.BallSprite, (int)ShopState.Use);
            ResetBallSelectBtns(DataManager.instance.BallSprite);
        }
    }

    public void ResetScroll()
    {
        SoundManager.instance.TapSound();
        if (myToggle.value)
            myPanel.SetDragAmount(0, 0, false);
        initList();
    }

    public void ResetBallSelectBtns(int bIdx, bool isPre = false, bool isBuy = false)
    {
        for (int i = 0; i < ballBtnArr.Length; ++i)
        {
            if (ballBtnArr[i].myState == ShopState.Use)
                SetUnUsed(i);

            if (isPre)
            {
                if (ballBtnArr[i].myIdx == bIdx)
                {
                    DataManager.instance.PreBallSprite = bIdx;
                    DataManager.instance.SetCharBallList(bIdx, (int)ShopState.PreUse);
                }
            }
            else
            {
                if (!isBuy)
                {
                    if (DataManager.instance.PreBallSprite > 0) return;

                    if (ballBtnArr[i].myIdx == bIdx)
                    {
                        DataManager.instance.BallSprite = bIdx;
                        DataManager.instance.SetCharBallList(bIdx, (int)ShopState.Use);
                    }
                }
                else
                {
                    if (ballBtnArr[i].myIdx == bIdx)
                    {
                        DataManager.instance.BallSprite = bIdx;
                        if (DataManager.instance.charBallList.Contains((int)ShopState.PreUse))
                            DataManager.instance.SetCharBallList(i, (int)ShopState.UnUse);
                        else
                            DataManager.instance.SetCharBallList(bIdx, (int)ShopState.Use);
                    }
                }
            }
            ballBtnArr[i].SetSelectBtn(DataManager.instance.GetCharBallList(i));
        }
    }

    public void SetCharBallPreData()
    {
        ballSkill.myState = ShopState.PreUse;
        ballSkill.SetBallState();
    }

    void SetUnUsed(int i)
    {
        DataManager.instance.SetCharBallList(i, (int)ShopState.UnUse);
    }

    public void BallSkillCall(int bId, ShopState myState, bool isPupCall = true)
    {
        ballSkill.bIdx = bId;
        ballSkill.myState = myState;
        if (isPupCall) ballSkill.CallPupTPTS();
        else ballSkill.SetBallState();
    }

    public void SetShopCloudData()
    {
        if (DataManager.instance.PreBallSprite == 0 && !DataManager.instance.charBallList.Contains(2))
        {
            if (DataManager.instance.BallSprite > 0)
                DataManager.instance.SetCharBallList(0, 1);

            DataManager.instance.SetCharBallList(DataManager.instance.BallSprite, 2);
        }

        if (DataManager.instance.PreBallSprite > 0)
        {
            DataManager.instance.SetCharBallList(DataManager.instance.BallSprite, 1);
        }
        ResetLocalData();
    }
    void ResetLocalData()
    {
        for(int i=0; i< ballBtnArr.Length; ++i)
        {
            if (i == 0 && DataManager.instance.GetCharBallList(i) == 0 && DataManager.instance.BallSprite != 0) DataManager.instance.SetCharBallList(i, 1);
            ballBtnArr[i].myState = (ShopState)DataManager.instance.GetCharBallList(i);
            ballBtnArr[i].SetData();
        }
    }

    public void HousingBallBtnSet(int idx)
    {
        ballBtnArr[idx].myState = (ShopState)DataManager.instance.GetCharBallList(idx);
        ballBtnArr[idx].SetData();
    }
}
