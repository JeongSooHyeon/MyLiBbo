using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class packageItem
{
    public int pID;
    public string pDiscountRate;
    public double pPrice;
    public int pMonsterID;
    public int pCoinCount;
    public int pBombCount;
    public int pAddBallCount;
}

public class PackageManager : MonoBehaviour
{
    [SerializeField] TextAsset myData;
    public List<packageItem> packageList;

    void Awake()
    {
        if (packageList == null)
            packageList = new List<packageItem>();
        else
            packageList.Clear();

        if (myData == null)
            myData = Resources.Load<TextAsset>("PackageData");

        string[] lines = myData.text.Split('\n');

        if (lines.Length == 0)
            Debug.Log("Text data is nothing.");
        else
        {
            for(int i = 0; i < lines.Length; ++i)
            {
                string[] txtD = lines[i].Split(',');
                packageList.Add(GetChangePackageData(txtD));
            }
        }
    }

    packageItem GetChangePackageData(string[] sData)
    {
        packageItem ret = new packageItem();

        int.TryParse(sData[0], out ret.pID);
        ret.pDiscountRate = sData[1];
        double.TryParse(sData[2], out ret.pPrice);

        int.TryParse(sData[3], out ret.pMonsterID);
        int.TryParse(sData[4], out ret.pCoinCount);
        int.TryParse(sData[5], out ret.pBombCount);
        int.TryParse(sData[6], out ret.pAddBallCount);

        return ret;
    }

    public packageItem GetPackageItem(int Idx)
    {
        if (Idx < 0)
            return null;

        return packageList.Find(t => t.pID == Idx);
    }
}
