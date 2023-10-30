public class LoadingPupControl : PopupBase
{
    public float time;
    public override void CallPupTPTS()
    {
        base.CallPupTPTS();
        if (time == 1f)
        {
            Invoke("ClosePupTPTS", time);
        }
    }

    public override void ClosePupTPTS()
    {
        CancelInvoke("ClosePupTPTS");
        base.ClosePupTPTS();
        LobbyManager.instance.isLoad = false;
    }
}
