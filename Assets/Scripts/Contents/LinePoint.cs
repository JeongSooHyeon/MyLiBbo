using UnityEngine;

public class LinePoint : MonoBehaviour
{
    int nextWayPoint;
    //lerp for two seconds.
    float timeLerped = 0.0f;
    float speed = 5;
    public Vector2 startPoint;
    public Vector2 nextPoint;
    public new GameObject light;
    DrawLine drawLine;
    // Use this for initialization
    void Start()
    {
        transform.position = DrawLine.waypoints[0];
        nextWayPoint++;
    }

    public void SetDraw(DrawLine draw_)
    {
        drawLine = draw_;
    }
    // Update is called once per frame
    void Update()
    {
        if (!drawLine.draw) return;
        if (startPoint == nextPoint)
            GetComponent<UISprite>().enabled = false;

        timeLerped += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);
        if ((Vector2)transform.position == nextPoint)
        {
            nextWayPoint = 0;
            transform.position = startPoint;

        }
    }
}
