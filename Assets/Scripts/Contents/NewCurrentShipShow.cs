using UnityEngine;

public class NewCurrentShipShow : MonoBehaviour
{
    [SerializeField] Animator charAnim;
    [SerializeField] SetCharacterCostume[] chearCostume;
    [SerializeField] SetCharacterCostume[] comboCostume_1;
    [SerializeField] SetCharacterCostume[] comboCostume_2;
    public int monsterCount;

    private void Start()
    {
        SetMonsterCount();
    }

    public void SetComboAnim()
    {
        if (BrickManager.instance.saveComboNum < 1) return;

        string charStr = "";
        if (DataManager.instance.CurGameMode != (int)GameMode.BALL100)
        {
            float charBlend = 0;
            if (BrickManager.instance.saveComboNum == 1) charBlend = 0;
            else if (BrickManager.instance.saveComboNum > 1 && BrickManager.instance.saveComboNum < 4) charBlend = 1;
            else charBlend = 2;
            if (BrickManager.instance.saveComboNum >= 1) ComboSound(charBlend);
            switch (monsterCount % 2)
            {
                case 0:
                    charStr = "Kkabin_0";
                    break;
                case 1:
                    charStr = "Kkomi_0";
                    break;
            }
            charStr += charBlend;
            charAnim.SetTrigger(charStr);
            //charAnim.SetFloat("CharBlend", charBlend);
            BrickManager.instance.saveComboNum = 0;
        }
    }

    void SetMonsterCount()
    {
        if (DataManager.instance.PreBallSprite > 0)
        {
            monsterCount = DataManager.instance.PreBallSprite;
        }
        else
        {
            monsterCount = DataManager.instance.BallSprite;
        }

        if ((GameMode)DataManager.instance.CurGameMode != GameMode.STAGE
            && (GameMode)DataManager.instance.CurGameMode != GameMode.STAGEBOSS) monsterCount = DataManager.instance.BallSprite;

        int charNum = monsterCount % 2;
        chearCostume[charNum].SetCostume(monsterCount);
        if (charNum == 0)
        {
            comboCostume_1[0].SetCostume(monsterCount);
            comboCostume_1[1].SetCostume(monsterCount + 1);
            comboCostume_2[0].SetCostume(monsterCount);
            comboCostume_2[1].SetCostume(monsterCount + 1);
        }
        else
        {
            comboCostume_1[0].SetCostume(monsterCount - 1);
            comboCostume_1[1].SetCostume(monsterCount);
            comboCostume_2[0].SetCostume(monsterCount - 1);
            comboCostume_2[1].SetCostume(monsterCount);
        }
    }

    public void ComboSound(float idx)
    {
        switch (idx)
        {
            case 0:
                SoundManager.instance.ChangeEffects(16);
                break;
            case 1:
                SoundManager.instance.ChangeEffects(17);
                break;
            case 2:
                SoundManager.instance.ChangeEffects(18);
                break;
            default:
                SoundManager.instance.ChangeEffects(16);
                break;
        }
    }
}
