using UnityEngine;

public class HitBossAcc : MonoBehaviour
{
    [SerializeField] BossControl bossControl;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bossControl.HitAcc(collision);
        if(bossControl.bossHP % 20 == 0)
        {
            bossControl.PlayFXeffect(transform.position);
        }
    }
}
