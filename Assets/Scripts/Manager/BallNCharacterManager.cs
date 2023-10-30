using UnityEngine;
[System.Serializable]
public struct BallItemData
{
    public int bId;
    public string bImg;
    public Color bColor;
    public float bAlpha;
    public string bDes;
    public int bPrice;
    public int bLockCondition;
}

[System.Serializable]
public struct CharacterData
{
    public int mId;
    public string mDes;
    public int mPrice;
    public int mLockCondition;
    public CharaterSkilltype cSkill;
}

[System.Serializable]
public struct CostumeData
{
    public int accNum;
    public int bodyNum;
    public int headNum;
    public int earNum;
    public int armNum;
}

public class BallNCharacterManager : MonoBehaviour
{
    public static BallNCharacterManager instance = null;
    [SerializeField] TextAsset[] myDatas;

    public BallItemData[] ballItemList;
    public CharacterData[] charDataList;
    public CostumeData[] costumeDataList;

    void Awake()
    {
        if (instance == null)
            instance = this;
        Init();
    }
    // Use this for initialization
    void Init()
    {
        if (ballItemList.Length == 0)
            ballItemList = new BallItemData[DataManager.maxShopBall];

        if (charDataList.Length == 0)
            charDataList = new CharacterData[DataManager.maxShopMonster];

        if (costumeDataList.Length == 0)
            costumeDataList = new CostumeData[DataManager.costumeMaxIdx];

        string[] lines = myDatas[0].text.Split('\n');
        if (lines.Length == 0)
            Debug.Log("text data is nothing!!");
        else
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                string[] txtD = lines[i].Split(',');
                BallItemData item = setBallDataItem(txtD);
                ballItemList[i] = item;
            }
        }

        string[] lines2 = myDatas[1].text.Split('\n');
        if (lines2.Length == 0)
            Debug.Log("text data is nothing!!");
        else
        {
            for (int i = 0; i < lines2.Length; ++i)
            {
                string[] txtD = lines2[i].Split(',');
                CharacterData item = setCharDataItem(txtD);
                charDataList[i] = item;
            }
        }

        string[] lines3 = myDatas[2].text.Split('\n');
        if (lines3.Length == 0)
            Debug.Log("text data is nothing!!");
        else
        {
            for (int i = 0; i < lines3.Length; ++i)
            {
                string[] txtD = lines3[i].Split(',');
                CostumeData item = setCostumeDataItem(txtD);
                costumeDataList[i] = item;
            }
        }
    }

    BallItemData setBallDataItem(string[] tData)
    {
        BallItemData Bdata = new BallItemData();
        int.TryParse(tData[0], out Bdata.bId);
        Bdata.bImg = tData[1];
        Bdata.bColor = NGUIText.ParseColor(tData[2]);
        Bdata.bDes = tData[4];
        float.TryParse(tData[3], out Bdata.bAlpha);
        int.TryParse(tData[5], out Bdata.bPrice);
        int.TryParse(tData[6], out Bdata.bLockCondition);

        return Bdata;
    }

    CharacterData setCharDataItem(string[] tData)
    {
        CharacterData Bdata = new CharacterData();
        int.TryParse(tData[0], out Bdata.mId);
        Bdata.mDes = tData[1];
        int.TryParse(tData[2], out Bdata.mPrice);
        int.TryParse(tData[3], out Bdata.mLockCondition);
        Bdata.cSkill = (CharaterSkilltype)int.Parse(tData[4].Replace("\r", ""));

        return Bdata;
    }

    CostumeData setCostumeDataItem(string[] tData)
    {
        CostumeData bData = new CostumeData();
        int.TryParse(tData[1], out bData.accNum);
        int.TryParse(tData[2], out bData.bodyNum);
        int.TryParse(tData[3], out bData.headNum);
        int.TryParse(tData[4], out bData.earNum);
        int.TryParse(tData[5], out bData.armNum);

        return bData;
    }

    public BallItemData GetBallData(int bIdx)
    {
        return ballItemList[bIdx];
    }

    public CharacterData GetCharData(int cIdx)
    {
        return charDataList[cIdx];
    }

    public string GetBallImgData(int bIdx)
    {
        return ballItemList[bIdx].bImg;
    }
}
