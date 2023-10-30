using UnityEngine;

public class LobbySprite : MonoBehaviour
{
    public UIState state_;
    UISprite sprite_;
    [SerializeField]
    bool isSnap;

    void Start()
    {
        LobbyController.instance.lobbySprite.Add(this);
        sprite_ = GetComponent<UISprite>();
    }

    public void SetSprite(string str , bool isShow)
    {
        sprite_.enabled = isShow;
        if (string.IsNullOrEmpty(str)) str = sprite_.spriteName;
        sprite_.spriteName = str;
        if (isSnap) sprite_.MakePixelPerfect();
    }
}
