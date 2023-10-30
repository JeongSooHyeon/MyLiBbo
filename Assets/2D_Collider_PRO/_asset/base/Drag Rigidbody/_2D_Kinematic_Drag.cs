using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Rigidbody2D))]
public class _2D_Kinematic_Drag : MonoBehaviour , _2D_IDraggable
{

	[Space(10)]

	[SerializeField()]
	bool snap_center;
	[SerializeField()]
	[Range(0,1)]
	float lerp_speed = 1f;
	
	[Space(10)]

	[SerializeField()]
	LayerMask collide_layers;
	[SerializeField()]
	bool trigger_when_drag;
	[SerializeField()]
	Color drag_color;

	//[Space()]

	[Header("Optional")]
	[Tooltip("if this null , The sprite renderer on this gameobject will be picked")]
	[SerializeField()]
	SpriteRenderer sprite_rend;

	



	Rigidbody2D _rigid_2d;
	Collider2D coll;
	SpriteRenderer sp_rend;
	Vector2 offset;
	bool old_freeze_rotation;
	bool old_trigger;
	#if UNITY_5_5 || UNITY_5_6
	bool last_simulated;
	RigidbodyType2D last_rigidtype;
	#else
	bool old_kinematic;
	#endif

	Vector3 old_position;
	Color old_color;
	bool is_collided;





	// Use this for initialization
	void Start () 
	{
		Init ();
	}
	
	
	
	
	void Init()
	{
		_rigid_2d = GetComponent<Rigidbody2D> ();
		coll = GetComponent<Collider2D>();

		if (sprite_rend)
			sp_rend = sprite_rend;
		else
			sp_rend = GetComponent<SpriteRenderer> ();
	}
	
	
	
	
	public void Start_Drag(Vector2 point , Rigidbody2D rigid = null)
	{
		offset = point - (Vector2)transform.position;

		old_freeze_rotation = _rigid_2d.freezeRotation;
		old_position = transform.position;
		old_color = sp_rend.color;
		old_trigger = coll.isTrigger;

		#if UNITY_5_5 || UNITY_5_6
		last_simulated = _rigid_2d.simulated;
		last_rigidtype = _rigid_2d.bodyType;
		_rigid_2d.simulated = true;
		_rigid_2d.bodyType = RigidbodyType2D.Kinematic;
		#else
		old_kinematic = _rigid_2d.isKinematic;
		_rigid_2d.isKinematic = true;
		#endif

		_rigid_2d.freezeRotation = true;
		coll.isTrigger = trigger_when_drag;
	}


	public void Update_Drag(Vector2 point)
	{
		if (snap_center)
			offset = Vector2.Lerp (offset, Vector2.zero, lerp_speed);

		transform.position = point - offset;

		is_collided = false;
		sp_rend.color = old_color;
		if (coll.IsTouchingLayers (collide_layers.value)) 
		{
			is_collided = true;
			sp_rend.color = drag_color;
		}
	}


	public void Stop_Drag()
	{
		coll.isTrigger = old_trigger;
		_rigid_2d.freezeRotation = old_freeze_rotation;

		#if UNITY_5_5 || UNITY_5_6
		_rigid_2d.simulated = last_simulated;
		_rigid_2d.bodyType = last_rigidtype;
		#else
		_rigid_2d.isKinematic = old_kinematic;
		#endif

		if(is_collided)
			transform.position = old_position;

		sp_rend.color = old_color;
		is_collided = false;
	}



	public float Get_Lerp_Speed()
	{
		return lerp_speed;
	}






	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/2DColliderPRO/Add/2D Drag/Kinematic Drag")]
	static void Add_2D_Drag_Manager()
	{
		if (UnityEditor.Selection.gameObjects.Length > 0)
		{
			for (int i = 0; i < UnityEditor.Selection.gameObjects.Length; i++)
			{
				if(UnityEditor.Selection.gameObjects[i].GetComponent<Collider2D>() == null)
					UnityEditor.Selection.gameObjects[i].AddComponent<PolygonCollider2D>();

				UnityEditor.Selection.gameObjects[i].AddComponent<_2D_Kinematic_Drag>();
			}
		}
		else
			Debug.Log ("Select GameObject(s)");
	}
	#endif
}
