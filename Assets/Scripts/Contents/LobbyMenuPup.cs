public class LobbyMenuPup : PopupBase
{
    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        LobbyManager.instance.curPopup = this;
        LobbyManager.instance.DailyStateCheck();
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        LobbyManager.instance.curPopup = null;
        LobbyManager.instance.DailyStateCheck();
    }
}
