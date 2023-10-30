using UnityEngine;

public class HitBoss : MonoBehaviour
{
    [SerializeField] UILabel bossLabel;
    [SerializeField] UISprite bossFace;
    [SerializeField] TweenAlpha hitAlpha;
    public int bossHP;
    string bossFaceStr;
    Animator anim;
    //[SerializeField] ParticleSystem[] ps_;
    [SerializeField] GameObject boomEffect;
    bool isDead;

    [SerializeField] int mHeight;
    public int posCount = 0;

    private void Start()
    {
        BrickManager.instance.SetBoss();
        anim = GetComponent<Animator>();
        if(bossHP == 0)
        {
            bossHP = DataManager.instance.BossHPCount;
        }
        UIManager.instance.sumBossHp(bossHP);
        bossLabel.text = bossHP.ToString();
        bossFaceStr = bossFace.spriteName;

        posCount += mHeight;
    }

    int idSound = 15;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ball"))
        {
            --bossHP;
            bossLabel.text = bossHP.ToString();
            if (bossHP % 10 == 0) bossFace.spriteName = bossFaceStr.Replace("_00", "_01");
            if (bossHP % 10 == 2) bossFace.spriteName = bossFaceStr.Replace("_01", "_00");
            hitAlpha.ResetToBeginning();
            hitAlpha.PlayForward();
            ++idSound;
            SoundManager.instance.ChangeEffects(9);
            if (bossHP <= 0)
            {
                if (isDead) return;
                isDead = true;
                GameObject go = Instantiate(boomEffect);
                go.transform.parent = transform.parent;
                go.transform.localScale = Vector3.one;
                go.transform.position = transform.position;
                go.GetComponent<ParticleSystem>().Play();
                BrickManager.instance.EndGameBoss();
                gameObject.SetActive(false);
            }
            UIManager.instance.CheckBossStar();
        }
    }
}
