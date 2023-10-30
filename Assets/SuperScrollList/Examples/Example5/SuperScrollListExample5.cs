using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperScrollListExample5 : MonoBehaviour
{
	public SuperScrollTableWrapper wrapper;
	public GameObject elementPrefab;
	public int dataSize = 50;

	void Start()
	{
		wrapper.SetRefreshCallback(OnItemRefresh);
		wrapper.SetClickCallback(OnItemClick);
		wrapper.SpawnNewList(elementPrefab, dataSize, 0);
	}
	
	void OnItemRefresh(GameObject go, int index)
	{
		go.GetComponent<UIWidget>().width = 60 + (index % 5) * 20;
		go.transform.GetChild(0).GetComponent<UILabel>().text = index.ToString();
	}

	void OnItemClick(GameObject go, int index)
	{
		Debug.Log("Click item " + index.ToString());
	}

	public void OnClick_GoToLeft()
	{
		wrapper.Refresh(0);
	}

	public void OnClick_GoToRight()
	{
		wrapper.Refresh(1);
	}
}
