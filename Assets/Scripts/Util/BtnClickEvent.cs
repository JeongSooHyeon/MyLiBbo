using System.Collections.Generic;
using UnityEngine;

public class BtnClickEvent : MonoBehaviour {
    public List<EventDelegate> onClick = new List<EventDelegate>();

    public bool isBoxColEnable;

    BoxCollider2D bCD2D;

    private void Start()
    {
        bCD2D = GetComponent<BoxCollider2D>();
    }

    private void OnBecameInvisible()
    {
        bCD2D.enabled = false;
    }

    private void OnBecameVisible()
    {
        bCD2D.enabled = true;
    }

    void OnClick()
    {
        if (isBoxColEnable) bCD2D.enabled = false;
        EventDelegate.Execute(onClick);
    }
}
