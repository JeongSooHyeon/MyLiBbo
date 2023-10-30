using UnityEngine;

public class SideWall : MonoBehaviour
{
    public bool isFirst;
    void OnTriggerEnter2D(Collider2D cd)
    {
        if (cd.gameObject.CompareTag("Brick"))
        {
            if (BrickManager.instance.moreCnt_ < 1 && !isFirst)
            {
                isFirst = true;
                if (!BrickManager.instance.continueThisStage
                    && DataManager.instance.GetCurStageMode() != GameMode.STAGEBOSS
                    && (!UIManager.instance.continueTodayAD || DataManager.instance.GetCoin() >= 300))
                {
                    if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                    {
                        for (int i = 0; i < BrickManager.instance.bundleList.Count; i++)
                        {
                            BrickManager.instance.bundleList[i].tPos.enabled = false;
                            BallManager.instance.SetFire(false);
                        }
                    }
                    UIManager.instance.Continue(3);
                }
                else if (DataManager.instance.CurGameMode == (int)GameMode.CLASSIC
                    || (GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
                {
                    if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                    {
                        for (int i = 0; i < BrickManager.instance.bundleList.Count; i++)
                        {
                            BrickManager.instance.bundleList[i].tPos.enabled = false;
                            BallManager.instance.SetFire(false);
                        }
                        BallManager.instance.ShootingResetBall(false);
                    }
                    BrickManager.instance.SetBrickGray(2);
                }
                else
                {
                    BrickManager.instance.SetBrickGray(1);
                }
            }
            else
            {
                UIManager.instance.Result();
                DataManager.instance.SetSaveOnemoreCnt(0);
            }
            BrickManager.instance.lastMoreCheck = cd.GetComponentInParent<BrickCount>();
        }

        if (cd.gameObject.CompareTag("Item"))
        {
            Items item_ = cd.gameObject.GetComponent<Items>();
            if (item_ == null) return;
            if (item_.itemType == Items.ItemInfo.Ball) BallManager.instance.PlusBall(1);
            else if (item_.itemType == Items.ItemInfo.Hole_In || item_.itemType == Items.ItemInfo.Hole_Out) BrickManager.instance.DeleteHoleItem();
            item_.BottomTrue();
        }
    }
}
