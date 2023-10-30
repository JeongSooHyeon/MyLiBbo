using UnityEngine;

public class BossControl : MonoBehaviour
{
    [SerializeField] UILabel bossLabel;
    [SerializeField] UISprite edgeSprite;
    [SerializeField] UISprite bossFace;
    [SerializeField] TweenAlpha hitAlpha;
    [SerializeField] TweenPosition b_Pos;
    UISprite bossSprite;

    public int bossHP;
    string bossFaceStr;
    Animator anim;

    [SerializeField] ParticleSystem[] ps_;
    [SerializeField] GameObject boomEffect;
    bool isDead;

    [SerializeField] int mHeight;
    public int posCount = 0;
    public bool mWarning = false;

    private void Start()
    {
        BrickManager.instance.SetBoss();
        anim = GetComponent<Animator>();
        if (bossHP == 0)
        {
            bossHP = DataManager.instance.BossHPCount;
        }
        UIManager.instance.sumBossHp(bossHP);
        bossLabel.text = bossHP.ToString();
        bossFaceStr = bossFace.spriteName;

        posCount += mHeight;

        BrickManager.instance.m_Boss.Add(this);
        b_Pos.to = b_Pos.from = transform.localPosition;
        bossSprite = b_Pos.GetComponent<UISprite>();
    }

    //int idSound = 15;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ball"))
        {
            HitBossCount(1);
        }
    }

    public void PlayFXeffect(Vector3 vec)
    {
        int ranFX = Random.Range(0, 2);
        ps_[ranFX].transform.position = vec;
        ps_[ranFX].Play();
    }

    public void EndGameNow()
    {
        if (!BrickManager.instance.isClear)
        {
            BrickManager.instance.SetBrickGray(1);
        }
    }

    public void HitAcc(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ball"))
        {
            HitBossCount(1);
        }
    }

    public void MoveBoss(int bCount)
    {
        b_Pos.from = b_Pos.to;
        b_Pos.to = new Vector3(b_Pos.to.x, b_Pos.to.y - 74, b_Pos.to.z);

        b_Pos.ResetToBeginning();
        b_Pos.PlayForward();

        ++posCount;

        if (bossHP > 0)
        {
            mWarning = posCount >= bCount;
        }
        else
        {
            mWarning = false;
        }
    }

    public void HitBossCount(int damage)
    {
        bossHP -= damage;
        bossLabel.text = bossHP.ToString();
        if (bossHP % 10 == 0) bossFace.spriteName = bossFaceStr.Replace("_00", "_01");
        if (bossHP % 10 == 2) bossFace.spriteName = bossFaceStr.Replace("_01", "_00");
        if (bossHP % 20 == 0) PlayFXeffect(transform.position);
        hitAlpha.ResetToBeginning();
        hitAlpha.PlayForward();
        SoundManager.instance.ChangeEffects(9);
        if (bossHP <= 0)
        {
            if (isDead) return;
            isDead = true;
            mWarning = false;
            GameObject go = Instantiate(boomEffect);
            go.transform.parent = transform.parent;
            go.transform.localScale = Vector3.one;
            go.transform.position = transform.position;
            go.GetComponent<ParticleSystem>().Play();
            BrickManager.instance.EndGameBoss();
            BrickManager.instance.TurnWarningSpriteBoss();
            gameObject.SetActive(false);
        }
        UIManager.instance.CheckBossStar();
    }
    public void SetGray()
    {
        bossSprite.color = new Color(90 / 255f, 90 / 255f, 90 / 255f);
        edgeSprite.color = new Color(35 / 255f, 35 / 255f, 35 / 255f);
    }
}
