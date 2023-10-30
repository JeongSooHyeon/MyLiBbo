using UnityEngine;

public class LobbyLabel : MonoBehaviour
{
    UILabel label_;
    public UIState state_;

    void Start()
    {
        if(label_ == null)
        {
             LobbyController.instance.lobbyLabel.Add(this);
            label_ = GetComponent<UILabel>();
        }
    }
    public void SetLabel(string txt , bool isShow)
    {
        label_.enabled = isShow;
        label_.text = txt;
    }
}
