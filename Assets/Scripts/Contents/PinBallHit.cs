using UnityEngine;

public class PinBallHit : MonoBehaviour
{
    UISprite sprite_;
    TweenPosition tpos;
    int hitCnt;

    private void Start()
    {
        sprite_ = GetComponent<UISprite>();
        tpos = GetComponent<TweenPosition>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Ball"))
        {
            ++hitCnt;
            if (hitCnt > 9) hitCnt = 0;
            sprite_.spriteName = "Ball_" + hitCnt;
        }
    }

    public void SetShwoBall(bool isTrue = false)
    {
        sprite_.enabled = isTrue;
    }
}
