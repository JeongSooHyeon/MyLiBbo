using System;
using System.Collections.Generic;
using UnityEngine;


public class DottedLineNgui : MonoBehaviour
{
    [Range(0.01f, 100f)]
    public float Delta;
    [SerializeField] GameObject dotRealObj;
    [SerializeField] Transform forParents;

    //Static Property with backing field
    public static DottedLineNgui instance;

    List<Vector2> positions = new List<Vector2>();
    List<GameObject> dots = new List<GameObject>();
    List<GameObject> firstDots = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        if (DataManager.instance.SweetAimPreUse || DataManager.instance.SweetAimUse) Delta = 79;
    }

    private void Start()
    {
        for(int i = 0; i < 70; ++i)
        {
            GameObject g = Instantiate(dotRealObj);
            g.transform.parent = forParents;
            g.transform.localScale = Vector3.one;
            firstDots.Add(g);
        }
        GC.Collect();
    }


    //Utility fields


    // Update is called once per frame
    void LateUpdate()
    {
        if (positions.Count > 0)
        {
            DestroyAllDots();
            positions.Clear();
        }
    }

    private void DestroyAllDots()
    {
        for (int i = 0; i < dots.Count; ++i)
        {
            FalseDot(dots[i]);
        }
        dots.Clear();
    }
    int cnt;
    GameObject GetOneDot()
    {
        GameObject dotObj = firstDots[cnt];
        dotObj.transform.parent = transform;
        dotObj.transform.localScale = Vector3.one;
        dotObj.SetActive(true);
        ++cnt;
        if (cnt > firstDots.Count - 1) cnt = 0;

        return dotObj;
    }

    void FalseDot(GameObject go)
    {
        go.SetActive(false);
        go.transform.parent = forParents;
        go.transform.localPosition = Vector3.zero;
    }

    public void DrawDottedLine(Vector2 start, Vector2 end, bool isLast)
    {
        DestroyAllDots();
        Vector2 point = start;
        Vector2 direction = (end - start).normalized;
        int count = 0;
        if (DataManager.instance.CurGameMode != (int)GameMode.SHOOTING)
        {
            while ((end - start).magnitude > (point - start).magnitude && count < 7)
            {
                if (isLast) ++count;
                positions.Add(point);

                point += (direction * Delta);
            }
        }
        Render();
    }

    private void Render()
    {
        for (int i = 0; i < positions.Count; ++i)
        {
            GameObject g = GetOneDot();
            g.transform.position = positions[i];
            dots.Add(g);
        }
    }
}
