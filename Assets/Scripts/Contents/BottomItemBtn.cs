using UnityEngine;

public class BottomItemBtn : MonoBehaviour
{
    [SerializeField] InstanceItem itemState;
    [SerializeField] GameObject[] showObj;
    [SerializeField] UILabel[] cntLabel;
    int itemCnt;
    public BoxCollider2D col2d_;

    bool isDouble;

    void Start()
    {
        col2d_ = GetComponent<BoxCollider2D>();
        if (itemState != InstanceItem.AdBall)
        {
            itemCnt = DataManager.instance.GetInstanceItemList(itemState);
            cntLabel[0].text = string.Format("x{0}", itemCnt);
            cntLabel[1].text = string.Format("x{0}", itemCnt);
           
        }
        else
        {
            itemCnt = 1;
        }
        SetBottomShow();
    }

    private void OnEnable()
    {
        if (itemState == InstanceItem.DoubleBall) isDouble = false;
    }

    public void SetBottomShow()
    {
        if (itemState == InstanceItem.DoubleBall && isDouble)
        {
            if(DataManager.instance.GetCoin() < 200)
            {
                showObj[3].SetActive(false);
                showObj[5].SetActive(true);
            }
            return;
        }
        if (itemCnt > 0)
        {
            showObj[3].SetActive(true);
            showObj[0].SetActive(true);
            showObj[4].SetActive(true);
            showObj[1].SetActive(false);
            showObj[2].SetActive(false);
            showObj[5].SetActive(false);
            col2d_.enabled = true;
        }
        else
        {
            showObj[0].SetActive(false);
            showObj[4].SetActive(false);
            showObj[2].SetActive(true);
            if (DataManager.instance.GetCoin() < 200)
            {
                showObj[3].SetActive(false);
                showObj[5].SetActive(true);
                col2d_.enabled = false;
            }
            else
            {
                showObj[3].SetActive(true);
                showObj[1].SetActive(true);
                col2d_.enabled = true;
            }
        }
        cntLabel[0].text = string.Format("x{0}", itemCnt);
        cntLabel[1].text = string.Format("x{0}", itemCnt);
    }

    public void plusBallAd()
    {
        showObj[3].SetActive(false);
        showObj[2].SetActive(true);
        col2d_.enabled = false;
        BallManager.instance.AdPlusBall();
    }

    void OnClick()
    {
        {
            switch (itemState)
            {
                case InstanceItem.DoubleBall:
                    SoundManager.instance.ChangeEffects(6);
                    showObj[3].SetActive(false);
                    showObj[2].SetActive(true);

                    col2d_.enabled = false;
                    isDouble = true;
                    BallManager.instance.ItemPlusBall();
                    cntLabel[1].text = string.Format("x{0}", itemCnt - 1);

                    if (itemCnt - 1 > 0)
                    {
                        showObj[5].SetActive(false);
                        showObj[4].SetActive(true);
                    }
                    else
                    {
                        showObj[5].SetActive(true);
                        showObj[4].SetActive(false);
                    }
                    break;
                case InstanceItem.AllBlockDamage:
                    SoundManager.instance.ChangeEffects(12);
                    BrickManager.instance.BottomBombBtn();
                    break;
                case InstanceItem.Undo:
                    BrickManager.instance.BottomUndoBtn();
                    break;
                case InstanceItem.AdBall:
                    UIManager.instance.ClickPlusBallAd();
                    break;
                case InstanceItem.PowerUp:
                    BallManager.instance.UsePowerUp(4);
                    break;
            }

            if (itemState != InstanceItem.AdBall)
            {
                if (itemCnt > 0)
                {
                    DataManager.instance.SetInstanceItemList(itemState, -1);
                    itemCnt = DataManager.instance.GetInstanceItemList(itemState);
                }
                else
                {
                    if (DataManager.instance.GetCoin() > 199)
                    {
                        DataManager.instance.SetFirebaseAnalyticsStageClearLog(FirebaseType.use_ruby, (int)itemState + 8);
                        DataManager.instance.SetCoin(-200);
                        UIManager.instance.SetMoney();
                    }
                }
                UIManager.instance.setBottomBtn();
            }
        }
    }

    private void Update()
    {
        showObj[2].SetActive(BallManager.instance.isDelay);
        showObj[3].SetActive(!BallManager.instance.isDelay);
        col2d_.enabled = !BallManager.instance.isDelay;
    }
}
