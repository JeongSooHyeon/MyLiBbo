using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Manage elements and place them one by one, works like UITable.
/// </summary>
public class SuperScrollTableWrapper : MonoBehaviour
{
	public int maxItemCount = 10;           // how many elements in scene, you shuold give a suitable value
	public float padding = 0;               // distance (pixels) between adjacent elements
	public OnItemRefresh onItemRefresh;     // callback to refresh an element
	public OnItemRefresh onItemClick;     // callback to refresh an element

	public UIScrollView ScrollView { get { if (_scrollView == null) { _scrollView = NGUITools.FindInParents<UIScrollView>(gameObject); } return _scrollView; } }
	public UIPanel Panel { get { if (_panel == null) { _panel = ScrollView.GetComponent<UIPanel>(); } return _panel; } }

	public bool IsHorScroll { get { return (_panel != null) ? (ScrollView.movement == UIScrollView.Movement.Horizontal) : true; } }
	public int DataSize { get; private set; }

	/// <summary>
	/// Build a new list
	/// </summary>
	/// <param name="prefab">prefab of element</param>
	/// <param name="dataSize">your data list size</param>
	/// <param name="defaultProgress"> [0 or 1] scroll position for scroll view</param>
	public void SpawnNewList(GameObject prefab, int dataSize, int defaultProgress)
	{
		Clear();
		InitItems(prefab);

		Resize(dataSize, defaultProgress);
	}

	/// <summary>
	/// Resize the list, and refresh all
	/// </summary>
	/// <param name="progress">the scroll position, 0 is left/top, 1 is right/bottom</param>
	public void Resize(int size, int progress)
	{
		DataSize = Mathf.Max(size, 0);
		_firstItem = null;
		_lastItem = null;
		if (DataSize < _itemList.Count)
		{
			for (int i = _itemList.Count - 1; i >= DataSize; i--)
			{
				Item item = _itemList[i];
				item.Disable();
				_itemPool.Add(item);
				_itemList.RemoveAt(i);
			}
		}
		else
		{
			for (int i = _itemList.Count; i < DataSize; i++)
			{
				if (_itemPool.Count == 0)
				{
					break;
				}
				Item item = _itemPool[_itemPool.Count - 1];
				item.go.SetActive(true);
				_itemPool.RemoveAt(_itemPool.Count - 1);
				_itemList.Add(item);
			}
		}
		Refresh(progress);
	}

	/// <summary>
	/// refresh all without resize
	/// </summary>
	public void Refresh(int progress)
	{
		if (DataSize <= 0)
		{
			return;
		}
		progress = progress > 0 ? 1 : 0;
		
		if (progress == 0)
		{
			_firstItem = _itemList[0];
			_firstItem.dataIndex = 0;
		}
		else
		{
			_firstItem = _itemList[(DataSize - _itemList.Count + 1) % _itemList.Count];
			_firstItem.dataIndex = DataSize - _itemList.Count;
		}
		
		for (int i = 0; i < _itemList.Count; i++)
		{
			Item item;
			if (i == 0)
			{
				item = _firstItem;
				item.tf.localPosition = Vector3.zero;
			}
			else
			{
				int itemIndex = (_firstItem.index + i < _itemList.Count) ? (_firstItem.index + i) : (_firstItem.index + i - _itemList.Count);
				item = _itemList[itemIndex];
				item.dataIndex = _firstItem.dataIndex + i;
			}

			if (onItemRefresh != null)
			{
				onItemRefresh(item.go, item.dataIndex);
			}
			UpdateWidget(item.go);
			item.bounds = NGUIMath.CalculateRelativeWidgetBounds(item.tf);

			if (_lastItem != null)
			{
				if (IsHorScroll)
				{
					item.tf.localPosition = _lastItem.tf.localPosition + new Vector3(_lastItem.bounds.max.x + padding - item.bounds.min.x, 0, 0);
				}
				else
				{
					item.tf.localPosition = _lastItem.tf.localPosition + new Vector3(0, _lastItem.bounds.min.y - padding - item.bounds.max.y, 0);
				}
			}
			
			_lastItem = item;
		}

		ScrollView.currentMomentum = Vector3.zero;
		ScrollView.InvalidateBounds();
		ScrollView.SetDragAmount(progress, progress, false);
	}

	/// <summary>
	/// set the callback for element refresh, you'd better set it before SpawnNewList()
	/// </summary>
	public void SetRefreshCallback(OnItemRefresh refreshCallback)
	{
		onItemRefresh = refreshCallback;
	}

	/// <summary>
	/// set the callback for element click (if element is button)
	/// </summary>
	public void SetClickCallback(OnItemRefresh clickCallback)
	{
		onItemClick = clickCallback;
	}


	private void Clear()
	{
		for (int i = 0; i < _itemPool.Count; i++)
		{
			Item item = _itemPool[i];
			if (item.go != null)
			{
				item.go.SetActive(false);
				Destroy(item.go);
			}
		}
		_itemPool.Clear();
		for (int i = 0; i < _itemList.Count; i++)
		{
			Item item = _itemList[i];
			if (item.go != null)
			{
				item.go.SetActive(false);
				Destroy(item.go);
			}
		}
		_itemList.Clear();
		DataSize = 0;
	}

	private void InitItems(GameObject prefab)
	{
		for (int i = 0; i < maxItemCount; i++)
		{
			Item item = new Item();
			item.index = i;
			item.go = NGUITools.AddChild(gameObject, prefab);
			item.go.name = prefab.name + i.ToString();
			item.tf = item.go.transform;
			item.Disable();

			if (item.go.GetComponent<Collider>() != null)
			{
				UIEventListener.Get(item.go).onClick = OnClick_Item;
			}

			_itemPool.Add(item);
		}
		_itemPool.Reverse();
	}

	private void Awake()
	{
		Panel.onClipMove += OnMove;
	}

	private void OnMove(UIPanel panel)
	{
		if (ignoreOnMove)
		{
			return;
		}
		if (DataSize == _itemList.Count)
		{
			return;
		}
		Vector3[] corners = Panel.worldCorners; // lb, lt, rt, rb

		if (IsHorScroll) // is horizontal scroll view
		{
			bool needCheck2 = true;
			while (_firstItem.dataIndex > 0) //can scroll left/up
			{
				Vector3 firstEdge = _firstItem.tf.TransformPoint(_firstItem.bounds.max);
				if (firstEdge.x > corners[0].x)
				{
					needCheck2 = false;
					//move the last item to be the new first item while the first item in sight
					Item freeItem = _lastItem;
					_lastItem = GetNearbyItem(_lastItem, -1);
					Item pFirstItem = _firstItem;
					_firstItem = freeItem;

					freeItem.dataIndex = pFirstItem.dataIndex - 1;
					if (onItemRefresh != null)
					{
						onItemRefresh(freeItem.go, freeItem.dataIndex);
					}
					UpdateWidget(freeItem.go);
					freeItem.bounds = NGUIMath.CalculateRelativeWidgetBounds(freeItem.tf);
					freeItem.tf.localPosition = pFirstItem.tf.localPosition + new Vector3(pFirstItem.bounds.min.x - padding - freeItem.bounds.max.x, 0, 0);
				}
				else
				{
					break;
				}
			}
			while (needCheck2 && _lastItem.dataIndex < DataSize - 1) //can scroll right/down
			{
				Item secondItem = GetNearbyItem(_firstItem, 1);
				Vector3 secondEdge = secondItem.tf.TransformPoint(_firstItem.bounds.max);
				if (secondEdge.x < corners[0].x)
				{
					//move the first item to be the new last item while the second item out sight
					Item freeItem = _firstItem;
					_firstItem = GetNearbyItem(_firstItem, 1);
					Item pLastItem = _lastItem;
					_lastItem = freeItem;

					freeItem.dataIndex = pLastItem.dataIndex + 1;
					if (onItemRefresh != null)
					{
						onItemRefresh(freeItem.go, freeItem.dataIndex);
					}
					UpdateWidget(freeItem.go);
					freeItem.bounds = NGUIMath.CalculateRelativeWidgetBounds(freeItem.tf);
					freeItem.tf.localPosition = pLastItem.tf.localPosition + new Vector3(pLastItem.bounds.max.x + padding - freeItem.bounds.min.x, 0, 0);
				}
				else
				{
					break;
				}
			}
		}
		else // is vertical scroll view
		{
			bool needCheck2 = true;
			while (_firstItem.dataIndex > 0) //can scroll left/up
			{
				Vector3 firstEdge = _firstItem.tf.TransformPoint(_firstItem.bounds.min);
				if (firstEdge.y < corners[1].y)
				{
					needCheck2 = false;
					//move the last item to be the new first item while the first item in sight
					Item freeItem = _lastItem;
					_lastItem = GetNearbyItem(_lastItem, -1);
					Item pFirstItem = _firstItem;
					_firstItem = freeItem;

					freeItem.dataIndex = pFirstItem.dataIndex - 1;
					if (onItemRefresh != null)
					{
						onItemRefresh(freeItem.go, freeItem.dataIndex);
					}
					UpdateWidget(freeItem.go);
					freeItem.bounds = NGUIMath.CalculateRelativeWidgetBounds(freeItem.tf);
					freeItem.tf.localPosition = pFirstItem.tf.localPosition + new Vector3(0, pFirstItem.bounds.max.y + padding - freeItem.bounds.min.y, 0);
				}
				else
				{
					break;
				}
				break;
			}
			while (needCheck2 && _lastItem.dataIndex < DataSize - 1) //can scroll right/down
			{
				Item secondItem = GetNearbyItem(_firstItem, 1);
				Vector3 secondEdge = secondItem.tf.TransformPoint(_firstItem.bounds.min);
				if (secondEdge.y > corners[1].y)
				{
					//move the first item to be the new last item while the second item out sight
					Item freeItem = _firstItem;
					_firstItem = GetNearbyItem(_firstItem, 1);
					Item pLastItem = _lastItem;
					_lastItem = freeItem;

					freeItem.dataIndex = pLastItem.dataIndex + 1;
					if (onItemRefresh != null)
					{
						onItemRefresh(freeItem.go, freeItem.dataIndex);
					}
					UpdateWidget(freeItem.go);
					freeItem.bounds = NGUIMath.CalculateRelativeWidgetBounds(freeItem.tf);
					freeItem.tf.localPosition = pLastItem.tf.localPosition + new Vector3(0, pLastItem.bounds.min.y - padding - freeItem.bounds.max.y, 0);
				}
				else
				{
					break;
				}
				break;
			}
		}

		ScrollView.restrictWithinPanel = _firstItem.dataIndex == 0 || _lastItem.dataIndex == DataSize - 1;
	}

	private Item GetNearbyItem(Item src, int offset)
	{
		int index = src.index + offset;
		if (index < 0)
		{
			index += _itemList.Count;
		}
		else if (index >= _itemList.Count)
		{
			index -= _itemList.Count;
		}
		return _itemList[index];
	}

	/// <summary>
	/// refresh all widgets in a gameObject
	/// </summary>
	private void UpdateWidget(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		_widgetList.Clear();
		go.GetComponentsInChildren(true, _widgetList);
		for (int i = 0; i < _widgetList.Count; i++)
		{
			_widgetList[i].ResetAndUpdateAnchors();
		}
	}

	private void OnClick_Item(GameObject go)
	{
		if (onItemClick == null)
		{
			return;
		}
		for (int i = 0; i < _itemList.Count; i++)
		{
			Item item = _itemList[i];
			if (item.go == go)
			{
				onItemClick(go, item.dataIndex);
				return;
			}
		}
	}
	[System.Serializable]
	private class Item
	{
		public int index;
		public int dataIndex;
		public bool isShow;
		public Bounds bounds;
		public GameObject go;
		public Transform tf;

		public void Disable()
		{
			isShow = false;
			if (go != null)
			{
				go.SetActive(false);
			}
		}
	}

	public delegate void OnItemRefresh(GameObject go, int dataIndex);

	private UIScrollView _scrollView;
	private UIPanel _panel;

	private List<GameObject> _goList = new List<GameObject>();
	[SerializeField]private List<Item> _itemList = new List<Item>();
	[SerializeField] private List<Item> _itemPool = new List<Item>();
	private Item _firstItem;
	private Item _lastItem;

	private List<UIWidget> _widgetList = new List<UIWidget>();
	private bool ignoreOnMove = false;
}
