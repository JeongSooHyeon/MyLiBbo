using UnityEngine;


public class SetCharacterCostume : MonoBehaviour
{
    CostumeData costumeData;
    [SerializeField] GameObject[] accObj;
    [SerializeField] GameObject[] bodyObj;
    [SerializeField] GameObject[] headObj;
    [SerializeField] GameObject[] earRObj;
    [SerializeField] GameObject[] earLObj;
    [SerializeField] GameObject[] armRObj;
    [SerializeField] GameObject[] armLObj;

    public void SetCostume(int num)
    {
        costumeData = CharBallDataManager.instance.costumeDataList[num];
        if (costumeData.accNum != -1)
        {
            accObj[costumeData.accNum].SetActive(true);
        }

        if (costumeData.bodyNum != -1)
        {
            bodyObj[costumeData.bodyNum].SetActive(true);
        }

        if (costumeData.headNum != -1)
        {
            headObj[costumeData.headNum].SetActive(true);
        }

        if (costumeData.earNum != -1)
        {
            earRObj[costumeData.earNum].SetActive(true);
            earLObj[costumeData.earNum].SetActive(true);
        }

        if (costumeData.armNum != -1)
        {
            armRObj[costumeData.armNum].SetActive(true);
            armLObj[costumeData.armNum].SetActive(true);
        }
    }
}
