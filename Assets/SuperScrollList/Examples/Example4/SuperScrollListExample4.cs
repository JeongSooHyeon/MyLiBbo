using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperScrollListExample4 : MonoBehaviour
{
	public UITable table;
	public GameObject locateButton;

	[Space(10)]
	public SuperScrollListWrapper wrapperA;
	public GameObject elementPrefabA;
	public int dataSizeA = 30;

	[Space(10)]
	public SuperScrollListWrapper wrapperB;
	public GameObject elementPrefabB;
	public int dataSizeB = 30;

	private int locateWrapperIndex = 1; // 0:"A"  1:"B"
	private int locateIndex = 0;

	void Start()
	{
		wrapperA.SetRefreshCallback(OnItemARefresh);
		wrapperA.SetClickCallback(OnItemAClick);
		wrapperA.SpawnNewList(elementPrefabA, dataSizeA, 0);

		wrapperB.SetRefreshCallback(OnItemBRefresh);
		wrapperB.SetClickCallback(OnItemBClick);
		wrapperB.SpawnNewList(elementPrefabB, dataSizeB, 0);

		table.Reposition();

		UIEventListener.Get(locateButton).onClick = OnLocateButtonClick;
		RefreshLocateButton();
		OnLocateButtonClick(null);
	}

	void OnItemARefresh(GameObject go, int index)
	{
		go.transform.GetChild(0).GetComponent<UILabel>().text = "A " + (index).ToString();
	}

	void OnItemAClick(GameObject go, int index)
	{
		Debug.Log("Click item A " + index.ToString());

		locateWrapperIndex = 0;
		locateIndex = index;
		RefreshLocateButton();
	}

	void OnItemBRefresh(GameObject go, int index)
	{
		go.transform.GetChild(0).GetComponent<UILabel>().text = "B " + (index).ToString();
	}

	void OnItemBClick(GameObject go, int index)
	{
		Debug.Log("Click item B " + index.ToString());

		locateWrapperIndex = 1;
		locateIndex = index;
		RefreshLocateButton();
	}

	void OnLocateButtonClick(GameObject go)
	{
		if (locateWrapperIndex == 0)
		{
			wrapperA.LocateSpecifiedItem(locateIndex);
		}
		else if (locateWrapperIndex == 1)
		{
			wrapperB.LocateSpecifiedItem(locateIndex);
		}
	}

	void RefreshLocateButton()
	{
		locateButton.GetComponentInChildren<UILabel>().text = string.Format("Center On  [[55ff55]{0}[-]-[FF55FF]{1}[-]]", locateWrapperIndex == 0 ? "A" : "B", locateIndex);
	}
}
