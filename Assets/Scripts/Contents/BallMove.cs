using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Vector3 ball_Pos_;
    Vector2 save_vel;
    public Rigidbody2D rd;
    [SerializeField] CircleCollider2D cc2D;
    [SerializeField] UISprite sprite_ = null;
    [SerializeField] TweenPosition t_pos;
    public bool isMove;
    public bool isFirst = false;
    public float roDel = 0;

    void Start()
    {
        BallManager.instance.ballList.Add(this);
        sprite_.spriteName = DataManager.instance.GetBall(BallManager.instance.ballSpriteCnt);
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            roDel += Time.deltaTime;
        }
    }

    public void RotateBall()
    {
        if (roDel >= BallManager.instance.roTime)
        {
            Vector2 vel = rd.velocity;
            float angle = Mathf.Atan2(rd.velocity.y, rd.velocity.x);
            int i = angle >= 0 ? 1 : -1;
            rd.velocity += new Vector2(0, i * 200f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            roDel = 0;
            if (testForce > rd.velocity.magnitude)
            {
                rd.velocity = rd.velocity.normalized * saveForce;
            }
        }
        if (collision.gameObject.CompareTag("RBounce"))
        {

            if (testForce > rd.velocity.magnitude) rd.velocity = rd.velocity.normalized * saveForce;
        }

        if (roDel >= BallManager.instance.roTime)
        {
            float angle = Mathf.Atan2(rd.velocity.y, rd.velocity.x);
            int i = angle >= 0 ? 1 : -1;
            rd.velocity += new Vector2(0, i * 200f);
            roDel = 0;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("RBounce"))
        {
            if (testForce > rd.velocity.magnitude) rd.velocity = rd.velocity.normalized * saveForce;
        }
    }
    public float testForce, saveForce;
    public void SetMove(float forceX, float force)
    {
        cc2D.enabled = true;
        isMove = true;
        sprite_.enabled = true;
        rd.isKinematic = false;
        transform.localEulerAngles = new Vector3(0, 0, forceX);
        rd.velocity = transform.up * force;
        save_vel = rd.velocity;
        testForce = rd.velocity.magnitude;
        saveForce = force;
        ball_Pos_ = transform.position;
    }

    public void FalseSprite()
    {
        if ((GameMode)DataManager.instance.CurGameMode == GameMode.SHOOTING)
            return;

        if (!isFirst) sprite_.enabled = false;
        isFirst = false;
    }

    public void spriteOn()
    {
        sprite_.enabled = true;
    }

    public void StopBall(bool isShow = false)
    {
        isMove = false;
        cc2D.enabled = false;
        rd.velocity = Vector2.zero;
        rd.isKinematic = true;
        t_pos.from.x = transform.localPosition.x;
        if (isShow) t_pos.from.y = transform.localPosition.y;
        else t_pos.from.y = -440;
        t_pos.to.x = BallManager.instance.RetunrnFirstBall();
        t_pos.ResetToBeginning();
        t_pos.PlayForward();
        roDel = 0;
        transform.localEulerAngles = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Brick"))
        {
            if (collision.GetComponent<BrickCollider>() != null)
                collision.GetComponent<BrickCollider>().PassBall(gameObject);
        }
        if (collision.CompareTag("Wall"))
        {
            if (collision.GetComponent<WallControl>().isSideWall) rd.velocity = new Vector2(-rd.velocity.x, rd.velocity.y);
            else rd.velocity = new Vector2(rd.velocity.x, -rd.velocity.y);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            if (testForce > rd.velocity.magnitude) rd.velocity = rd.velocity.normalized * saveForce;
        }
        if (rd.velocity != Vector2.zero) save_vel = rd.velocity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Brick"))
        {
            if (rd.velocity == Vector2.zero)
            {
                rd.velocity = save_vel.normalized * saveForce;
            }
        }
    }
}