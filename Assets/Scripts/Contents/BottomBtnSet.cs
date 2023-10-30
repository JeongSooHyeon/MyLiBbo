using UnityEngine;

public class BottomBtnSet : MonoBehaviour
{
    [SerializeField] BottomItemBtn[] btns;

    public void SetReset()
    {
        for (int i = 0; i < btns.Length; ++i)
        {
            if (btns[i].gameObject.activeSelf) btns[i].SetBottomShow();
        }
    }
}
