using UnityEngine;

[System.Serializable]
public class CharacterBallData
{
    public int idx;
    public int price;
    public int housingLock;
    public string spriteName;
}

public class CharBallDataManager : MonoBehaviour
{
    public static CharBallDataManager instance;
    [SerializeField] TextAsset[] myCB_Data;

    public CharacterBallData[] CBList;
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
        if (CBList.Length == 0)
            CBList = new CharacterBallData[DataManager.maxShopNum];

        if (costumeDataList.Length == 0)
            costumeDataList = new CostumeData[DataManager.costumeMaxIdx];

        string[] lines = myCB_Data[0].text.Split('\n');
        if (lines.Length == 0)
            Debug.Log("text data is nothing!!");
        else
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                string[] txtD = lines[i].Split(',');
                CharacterBallData item = SetCBData(txtD);
                CBList[i] = item;
            }
        }

        string[] lines3 = myCB_Data[1].text.Split('\n');
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

    CharacterBallData SetCBData(string[] tData)
    {
        CharacterBallData CB_data = new CharacterBallData();
        int.TryParse(tData[0], out CB_data.idx);
        int.TryParse(tData[1], out CB_data.price);
        int.TryParse(tData[2], out CB_data.housingLock);
        CB_data.spriteName = tData[3];

        return CB_data;
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

    public CharacterBallData GetCBData(int idx)
    {
        return CBList[idx];
    }
}
