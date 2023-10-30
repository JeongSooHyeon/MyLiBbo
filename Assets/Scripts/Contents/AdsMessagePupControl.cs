public class AdsMessagePupControl : PopupBase
{
    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        LobbyManager.instance.curPopup = this;
    }

    public override void ClosePupTPTS()
    {
        base.ClosePupTPTS();
        LobbyManager.instance.curPopup = null;
    }
}
