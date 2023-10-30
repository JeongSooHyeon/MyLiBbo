using UnityEngine;

public class ShootingItemBtn : MonoBehaviour
{
    [SerializeField] ShootingItemControl itemControl;
    [SerializeField] GameObject[] button_obj;
    [SerializeField] UILabel[] labelList;
    public int level;
    public int money;
    public InstanceItem item;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] TweenAlpha label_A;
    [SerializeField] TweenPosition label_P;
    [SerializeField] UILabel label_;

    private void Start()
    {
        level = 1;
        ObjUpdate();
        LabelUpdate();
    }

    private void Update()
    {
        ObjUpdate();
    }

    void ObjUpdate()
    {
        button_obj[0].SetActive(money <= BallManager.instance.money);
        button_obj[1].SetActive(!button_obj[0].activeSelf);
        bc.enabled = (money <= BallManager.instance.money);
    }

    void LabelUpdate()
    {
        labelList[0].text =  "LV. " + UIManager.instance.TextChange(level);
        labelList[1].text = "x" + UIManager.instance.TextChange(money);
        labelList[2].text = labelList[0].text;
        labelList[3].text = labelList[1].text;
    }

    void OnClick()
    {
        SoundManager.instance.TapSound();
        switch (item)
        {
            case InstanceItem.PlusBall:
                BallManager.instance.GetMoney(-money);
                BallManager.instance.UsePlusItem();
                itemControl.LevelUp(0);
                break;
            case InstanceItem.AttackUp:
                BallManager.instance.GetMoney(-money);
                BallManager.instance.UseAttackUp(level);
                itemControl.LevelUp(1);
                break;
            case InstanceItem.FireUp:
                if (BallManager.instance.fireTime >= 25) return;
                BallManager.instance.GetMoney(-money);
                BallManager.instance.UseFireUp();
                itemControl.LevelUp(2);
                break;
            case InstanceItem.ProjUp:
                BallManager.instance.GetMoney(-money);
                BallManager.instance.UseProjSpeedUp();
                itemControl.LevelUp(3);
                break;
        }
        LabelUpdate();
        LabelCallTPTS();
    }

    void LabelCallTPTS()
    {
        label_A.PlayForward();
        label_P.PlayForward();
    }

    public void ResetLabel()
    {
        label_A.ResetToBeginning();
        label_P.ResetToBeginning();
    }
}
