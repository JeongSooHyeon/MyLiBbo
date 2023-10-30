using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;





[System.Serializable]
public class TriggerEvent : UnityEvent<Collider2D>
{}
[System.Serializable]
public class CollisionEvent : UnityEvent<Collision2D>
{}


/// <summary>
/// Dynamic Bounds Collider for Sprite Renderer
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class _2D_Bound_Collider : MonoBehaviour
{
	[Space(15)]

	[SerializeField()]
	bool _dynamic = true;
	[SerializeField()]
	bool is_trigger;
	[SerializeField()]
	bool used_by_effector;
	[SerializeField]
	PhysicsMaterial2D physical_material_2D;


	[Space(30)]
	// trigger events
	[HideInInspector()]
	public TriggerEvent OnTriggerEnter_2D;
	[HideInInspector()]
	public TriggerEvent OnTriggerExit_2D;
	[HideInInspector()]
	public TriggerEvent OnTriggerStay_2D;


	[Space(30)]
	// collision events
	[HideInInspector()]
	public CollisionEvent OnCollisionEnter_2D;
	[HideInInspector()]
	public CollisionEvent OnCollisionExit_2D;
	[HideInInspector()]
	public CollisionEvent OnCollisionStay_2D;
	



	
	// privates
	Transform _transform;
	BoxCollider2D b_col_2D;
	SpriteRenderer sp_rend;
	Bound_Slave_Collider slave_collider;
	[HideInInspector()] public int selected_layer;
	[HideInInspector()] public List<string>layers = new List<string>();






	// Use this for initialization
	void Start () 
	{	
		_transform = transform;

		sp_rend = GetComponent<SpriteRenderer> ();

		Init_Slave_Collider ();

		Set_Trigger (is_trigger);
		Use_Effector (used_by_effector);
		Set_Physical_Material (physical_material_2D);
	}



	


	//******************************   PUBLIC METHODES   ****************************
	public Collider2D Get_Bound_Collider()
	{
		return slave_collider.Get_Collider ();
	}
	public void Stop_Update()
	{
		_dynamic = false;
	}
	public void Start_Update()
	{
		_dynamic = true;
	}
	public void Set_Trigger(bool b)
	{
		slave_collider.Get_Collider ().isTrigger = b;
	}
	public void Set_Physical_Material(PhysicsMaterial2D phys_mat)
	{
		slave_collider.Get_Collider ().sharedMaterial = phys_mat;
	}
	public void Use_Effector(bool b)
	{
		slave_collider.Get_Collider ().usedByEffector = b;
	}
	//********************************************************************************





	void FixedUpdate ()
	{
		if(_dynamic)
			Update_Bound_Collider ();
	}
	




	void Init_Slave_Collider()
	{
		GameObject go = new GameObject("bounds_collider_2D");
		go.transform.SetParent (transform);
		go.transform.localPosition = Vector3.zero;
		go.gameObject.layer = selected_layer;
		b_col_2D = go.AddComponent<BoxCollider2D> ();

		slave_collider = go.AddComponent<Bound_Slave_Collider>();
		slave_collider.Set_Master_Collider (this);
	}



	void Update_Bound_Collider()
	{
		b_col_2D.transform.eulerAngles = new Vector3 (0,0,0);

		float scale_x = _transform.localScale.x;
		float scale_y = _transform.localScale.y;

		b_col_2D.size = new Vector2 (sp_rend.bounds.size.x / scale_x , sp_rend.bounds.size.y / scale_y);
		b_col_2D.offset = new Vector2 (sp_rend.bounds.center.x - _transform.position.x, sp_rend.bounds.center.y - _transform.position.y);
	}






	void Set_Layers()
	{
		layers = new List<string> ();

		layers.Add ("Default");
		layers.Add ("Transparent");
		layers.Add ("Ignore Raycast");
		layers.Add ("");
		layers.Add ("Water");
		layers.Add ("UI");
		layers.Add ("");
		layers.Add ("");

		for (int i = 8; i < 32; i++)
		{
			string s = LayerMask.LayerToName(i);
			if(s.Length > 0)
				layers.Add(s);
		}
	}



	void Update_Layers()
	{	
		int k = 0;
		for (int i = 0; i < 32; i++)
		if(LayerMask.LayerToName(i).Length > 0)
			k++;

		if(k != layers.Count)
			Set_Layers ();
		else
		{
			for (int i = 0; i < layers.Count; i++)
			if(layers[i] != LayerMask.LayerToName(i))
				Set_Layers();
		}
	}




#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/2DColliderPRO/Add/Bounds Collider")]
	static void Add_Bound_Collider()
	{
		if (UnityEditor.Selection.gameObjects.Length == 1)
			UnityEditor.Selection.activeGameObject.AddComponent<_2D_Bound_Collider> ();
		else
			Debug.Log ("Select an empty GameObject");
	}
#endif

}
