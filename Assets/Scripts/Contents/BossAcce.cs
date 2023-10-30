using UnityEngine;

public class BossAcce : MonoBehaviour
{
    [SerializeField]BossControl bossHit;
    private void Start()
    {
        bossHit = GetComponentInParent<BossControl>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bossHit == null) GetComponentInParent<HitBoss>();
        bossHit.PlayFXeffect(transform.position);
        if (collision.gameObject.CompareTag("Ball")) Destroy(gameObject);
    }
}
