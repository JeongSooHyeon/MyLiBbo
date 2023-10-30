using UnityEngine;

public class StageBtnControl : MonoBehaviour {
    [SerializeField] UILabel myLabel;
    [SerializeField] UISprite mySprite;
    [SerializeField] Color[] TxtColors;
    [SerializeField] UISprite lockImg;
    [SerializeField] UISprite glowImg;
    [SerializeField] TweenAlpha bgTA;
    [SerializeField] Vector3[] LabelPoss;
    [SerializeField] string[] bgSprTxts;
    [SerializeField] string[] lockSprTxts;
    [SerializeField] UISprite[] StarBgArr;
    [SerializeField] UISprite[] StarImgArr;
    BoxCollider2D myBoxColl;
    public bool isOpened = false;
    public int myIdx = 0;
    [SerializeField] GameObject bossStage_Icon;

    public void SetState(bool isOpen, int idx, int starNum)
    {
        myIdx = idx;
        isOpened = isOpen;
        glowImg.enabled = isOpen;
        for (int i = 0; i < StarImgArr.Length; ++i)
        {
            StarImgArr[i].enabled = false;
        }
        bgTA.enabled = false;
        for (int i = 0; i < StarBgArr.Length; ++i)
        {
            StarBgArr[i].enabled = isOpened;
        }
        lockImg.enabled = !isOpened;
        int tIdx = 0, sIdx = 0, lIdx = 0;

        if ((idx % DataManager.BossTermNum) == 0)
        {
            lIdx = 1;
            sIdx = 3;
        }   
        else
            sIdx = 0;

        if (isOpened)
        {
            myBoxColl = GetComponent<BoxCollider2D>();
            tIdx = 1;
            if ((idx % DataManager.BossTermNum) == 0)
                sIdx = 4;
            else
                sIdx = 1;

           if(DataManager.instance.CurGameStage == DataManager.maxStageNum)
                bgTA.enabled = (DataManager.maxStageNum == myIdx);
            else
                bgTA.enabled = (DataManager.instance.curStageIdx() == myIdx);

            glowImg.enabled = bgTA.enabled;

            if (bgTA.enabled)
            {
                if ((idx % DataManager.BossTermNum) == 0)
                    sIdx = 5;
                else
                    sIdx = 2;
                tIdx = 2;
            }

            for (int i = 0; i < starNum; ++i)
            {
                StarImgArr[i].enabled = true;
            }
        }

        myLabel.gameObject.transform.localPosition = LabelPoss[0];
        myLabel.text = myIdx.ToString();
        mySprite.spriteName = bgSprTxts[sIdx];
        if (lockImg.enabled) lockImg.spriteName = lockSprTxts[lIdx];
        myLabel.color = TxtColors[tIdx];
        bossStage_Icon.SetActive(myIdx % 10 == 0);
    }
    
    void OnClick()
    {
        if(isOpened)
        {
            myBoxColl.enabled = false;
            SoundManager.instance.TapSound(1);
            DataManager.instance.CurGameStage = myIdx;
            DataManager.instance.GameStartCall();
        }   
    }
}
