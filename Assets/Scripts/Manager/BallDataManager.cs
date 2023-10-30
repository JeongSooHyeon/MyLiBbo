using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class BallData
{
    public int bIdx;  //공 아이디
    public int bGrade; //공 등급
    public string bName;  //공 이름
    public int bSkill;    //공 스킬
    public int bEffectIdx;  //공 효과
    public string bTag; // 공 별명
}
public class BallDataManager : MonoBehaviour
{
    public static BallDataManager instance = null;
    [SerializeField]
    TextAsset myData;
    [SerializeField]
    string myDataFileName;
    public List<BallData> BallDataList;
    void Awake()
    {
        if(instance == null)
            instance = this;
        Init();
    }
    // Use this for initialization
    void Init() {
        if (BallDataList == null)
            BallDataList = new List<BallData>();

        if (myData == null)
            myData = Resources.Load<TextAsset>(myDataFileName);

        string[] lines = myData.text.Split('\n');
        if (lines.Length == 0)
            Debug.Log("text data is nothing!!");
        else
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                string[] txtD = lines[i].Split(',');
                BallData item = setBallDataItem(txtD);
                if (item != null)
                {
                    BallDataList.Add(item);
                }
            }
        }
    }
    BallData setBallDataItem(string[] tData)
    {
        BallData Bdata = new BallData();
        Bdata.bIdx = int.Parse(tData[0]);
        Bdata.bGrade = int.Parse(tData[1]);
        Bdata.bName = tData[2];
        Bdata.bSkill = int.Parse(tData[3]);
        Bdata.bEffectIdx = int.Parse(tData[4]); 
        Bdata.bTag = tData[5];
        return Bdata;
    }
}
