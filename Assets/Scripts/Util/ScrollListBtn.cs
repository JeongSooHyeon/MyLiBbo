using UnityEngine;

public class ScrollListBtn : MonoBehaviour {
    public Vector3 testPos;
    [SerializeField] private UIScrollView myScrollView;
    [SerializeField] int CurIdx;
    [SerializeField] Transform myTarget;
    [SerializeField] GameObject BackBG;
   
    void OnClick()
    {
        SelectScrollBtn();
    }

    public void SelectScrollBtn()
    {
        SoundManager.instance.TapSound();
        SpringPanel.Begin(myScrollView.gameObject, testPos, 8f);
        BackBG.transform.localPosition = testPos*-1f;
        myScrollView.centerOnChild.CenterOn(myTarget);
        LobbyManager.instance.state = (LobbyState)CurIdx;
        UIStatePosReset();
    }

    public void UIStatePosReset()
    {
        switch (LobbyManager.instance.state)
        {
            case LobbyState.Shop:
                break;
            case LobbyState.Character:
                break;
            case LobbyState.Lobby:
                break;
            case LobbyState.Housing:
                LobbyManager.instance.HousingIdxSet();
                break;
            case LobbyState.Mode:
                break;
        }
    }
}
