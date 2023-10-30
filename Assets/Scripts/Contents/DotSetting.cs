using UnityEngine;

public class DotSetting : MonoBehaviour
{
    [SerializeField] UISprite sprite_;

    private void Start()
    {
        sprite_.spriteName = DataManager.instance.GetBall(BallManager.instance.ballSpriteCnt);
        gameObject.SetActive(false);
    }
}
