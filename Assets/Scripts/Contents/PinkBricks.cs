using UnityEngine;

public class PinkBricks : MonoBehaviour
{
    [SerializeField] UILabel HpText;
    [SerializeField] TweenAlpha tweenA;
    public int hp = 100;

    private void Start()
    {
        HpText.text = hp.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            --hp;
            if (hp <= 0)
            {
                gameObject.SetActive(false);
            }
            tweenA.ResetToBeginning(); // 이건 깜박거리는 스프라이트 함수입니다.
            tweenA.PlayForward();
            HpText.text = hp.ToString();
        }
    }
}
