using UnityEngine;

public class ToggleMenuChecker : MonoBehaviour {
    [SerializeField] UIToggle[] myBtns;
    [SerializeField] ScrollListBtn[] myBtnScrolls;
    [SerializeField] UICenterOnChild myGrid;
    public float swipePos;

    public void CheckDrag()
    {
        for (int i = 0; i < myBtns.Length; ++i)
        {
            myBtns[i].value = false;
        }
        string str = myGrid.centeredObject.name.Replace("MainView_", "");
        int cIdx = int.Parse(str);
        SetButton(cIdx);
    }

    public void SetButton(int cIdx)
    {
        myBtns[cIdx].value = true;
    }
}
