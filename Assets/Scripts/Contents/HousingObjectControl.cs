using UnityEngine;

public class HousingObjectControl : MonoBehaviour
{
    [SerializeField] int idx;
    GameObject obj;

    public void CheckObj()
    {
        if (obj == null)
            obj = gameObject;
        obj.SetActive(DataManager.instance.stageChapterStateList[idx] == 2);
    }
}
