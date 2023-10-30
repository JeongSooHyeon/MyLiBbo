using UnityEngine;

public class MoregamePup : PopupBase
{
    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        LobbyManager.instance.curPopup = myType;
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        LobbyManager.instance.curPopup = null;
    }

    public void ClickMoreGame_1()
    {
        SoundManager.instance.TapSound();
        Application.OpenURL(DataManager.TinyGolfURL);
    }
}
