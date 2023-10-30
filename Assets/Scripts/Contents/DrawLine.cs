using UnityEngine;

public class DrawLine : MonoBehaviour
{
    LineRenderer line;
    public bool draw = false;
    Color col;

    public static Vector2[] waypoints = new Vector2[3];
    float addAngle = 185;
    public GameObject pointer;
    public GameObject[,] pointers = new GameObject[3, 15];
    GameObject[,] pointers2 = new GameObject[3, 25];
    //GameObject[,] pointers3 = new GameObject[3, 15];

    Vector3 lastMousePos;
    private bool startAnim;

    // Use this for initialization
    void Start()
    {
        line = GetComponent<LineRenderer>();
        waypoints[0] = GameObject.Find("ArrowImg").transform.position;
        waypoints[1] = waypoints[0] + Vector2.up * 15;
        for (int i = 0; i < 3; i++)
        {
            GeneratePoints(i);
            GeneratePositionsPoints(waypoints, i);
            HidePoints(i);
        }

    }

    int GetAimPoints()
    {
        return pointers2.GetLength(1);
    }

    //bool pointsHidden;
    void HidePoints(int num = 0)
    {
        for (int i = 0; i < pointers.GetLength(1); i++)
        {
            pointers[num, i].GetComponent<UISprite>().enabled = false;
            pointers[num, i].GetComponent<LinePoint>().light.SetActive(false);
        }

        for (int i = 0; i < pointers2.GetLength(1); i++)
        {
            pointers2[num, i].GetComponent<UISprite>().enabled = false;
            pointers2[num, i].GetComponent<LinePoint>().light.SetActive(false);
        }
        //pointsHidden = true;
    }

    void HideAllPoints()
    {
        for (int i = 0; i < 3; i++)
        {
            HidePoints(i);
        }

    }

    void EnableBoostLight()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < pointers.GetLength(1); j++)
            {
                pointers[i, j].GetComponent<LinePoint>().light.SetActive(true);
            }

            for (int j = 0; j < pointers2.GetLength(1); j++)
            {
                pointers2[i, j].GetComponent<LinePoint>().light.SetActive(true);
            }
        }

    }

    private void GeneratePositionsPoints(Vector2[] waypoints, int num = 0)
    {
        HidePoints(num);

        for (int i = 0; i < pointers.GetLength(1); i++)
        {
            Vector2 AB = waypoints[1] - waypoints[0];
            AB = AB.normalized;
            float step = i / 1.5f;
            Vector2 newPos = waypoints[0] + (step * AB);
            if (step >= (waypoints[1] - waypoints[0]).magnitude)
            {
                newPos = waypoints[1];
            }
            //pointsHidden = false;
            pointers[num, i].GetComponent<UISprite>().enabled = true;
            pointers[num, i].transform.position = newPos;
            pointers[num, i].GetComponent<UISprite>().color = col;
            pointers[num, i].GetComponent<LinePoint>().startPoint = pointers[num, i].transform.position;
            pointers[num, i].GetComponent<LinePoint>().nextPoint = pointers[num, i].transform.position;
            if (i > 0)
                pointers[num, i - 1].GetComponent<LinePoint>().nextPoint = pointers[num, i].transform.position;

        }
        for (int i = 0; i < GetAimPoints(); i++)
        {
            Vector2 AB = waypoints[2] - waypoints[1];
            AB = AB.normalized;
            float step = i / 2f;

            if (step < (waypoints[2] - waypoints[1]).magnitude)
            {
                pointers2[num, i].GetComponent<UISprite>().enabled = true;
                pointers2[num, i].transform.position = waypoints[1] + (step * AB);
                pointers2[num, i].GetComponent<UISprite>().color = col;
                pointers2[num, i].GetComponent<LinePoint>().startPoint = pointers2[num, i].transform.position;
                pointers2[num, i].GetComponent<LinePoint>().nextPoint = pointers2[num, i].transform.position;
                if (i > 0)
                    pointers2[num, i - 1].GetComponent<LinePoint>().nextPoint = pointers2[num, i].transform.position;
            }
        }
    }

    void GeneratePoints(int num = 0)
    {
        for (int i = 0; i < pointers.GetLength(1); i++)
        {
            pointers[num, i] = Instantiate(pointer, transform.position, transform.rotation) as GameObject;
            pointers[num, i].transform.parent = transform;
            pointers[num, i].transform.localScale = Vector3.one;
            pointers[num, i].GetComponent<LinePoint>().light.SetActive(false);
            pointers[num, i].GetComponent<LinePoint>().SetDraw(this);
        }
        for (int i = 0; i < pointers2.GetLength(1); i++)
        {
            pointers2[num, i] = Instantiate(pointer, transform.position, transform.rotation) as GameObject;
            pointers2[num, i].transform.parent = transform;
            pointers2[num, i].transform.localScale = Vector3.one;
            pointers2[num, i].GetComponent<LinePoint>().light.SetActive(false);
            pointers2[num, i].GetComponent<LinePoint>().SetDraw(this);

        }
    }

    bool boostEnabled;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            draw = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            draw = false;
        }

        if (UIManager.instance.isShowLine)
        {
            Draw(waypoints);
        }
        else if (!draw)
        {
            for (int i = 0; i < 3; i++)
            {
                HidePoints(i);
            }
        }

    }

    void Draw(Vector2[] waypoints_, int num = 0)
    {
        Vector3 dir = Vector2.zero;

        dir = UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition) - Vector3.back * 10;
        if (num == 1)
        {
            dir.x += 1.5f;
        }
        if (num == 2)
        {
            dir.x -= 1.5f;
        }

        Ray ray = UICamera.mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!UIManager.instance.isShowLine)
        {
            dir.z = 0;
            if (num == 0)
            {
                if (lastMousePos == dir)
                {
                    startAnim = true;
                }
                else
                    startAnim = false;
                lastMousePos = dir;
            }


            RaycastHit2D[] hit = Physics2D.LinecastAll(waypoints_[0], waypoints_[0] + ((Vector2)dir - waypoints_[0]).normalized * 100, ~(1 << LayerMask.NameToLayer("Mesh")));
            foreach (RaycastHit2D item in hit)
            {
                Vector2 point = item.point;

                addAngle = 180;

                if (waypoints_[1].x < 0)
                    addAngle = 10;
                if (item.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    waypoints_[1] = point;
                    waypoints_[2] = point;
                    float angle = 0;
                    angle = Vector2.Angle(waypoints_[0] - waypoints_[1], (point - Vector2.up * 100) - (Vector2)point);
                    if (waypoints_[1].x > 0)
                        angle = Vector2.Angle(waypoints_[0] - waypoints_[1], (Vector2)point - (point - Vector2.up * 100));
                    waypoints_[2] = Quaternion.AngleAxis(angle + addAngle, Vector3.back) * ((Vector2)point - (point - Vector2.up * 100));
                    RaycastHit2D hit2 = Physics2D.Raycast(waypoints_[1], waypoints_[2], 1000, (1 << LayerMask.NameToLayer("Ball")));
                    if (hit2.collider != null)
                    {
                        waypoints_[2] = hit2.point;
                    }
                    break;

                }
                else if (item.collider.gameObject.layer == LayerMask.NameToLayer("Ball"))
                {
                    if (num == 0)
                    {

                    }
                    waypoints_[1] = point;
                    waypoints_[2] = point;
                    break;

                }
                else
                {
                    waypoints_[1] = waypoints_[0] + ((Vector2)dir - waypoints_[0]).normalized * 10;
                    waypoints_[2] = waypoints_[0] + ((Vector2)dir - waypoints_[0]).normalized * 10;
                }
            }
            if (!startAnim)
                GeneratePositionsPoints(waypoints_, num);
        }
    }

    void GetReversSquare(Vector2 firstPos, Vector2 endPos, int num)
    {
        RaycastHit2D[] hit = Physics2D.LinecastAll(firstPos, endPos, 1 << 10);

    }
}
