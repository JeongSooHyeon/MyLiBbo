using UnityEngine;
using System.Collections;





[RequireComponent(typeof(Rigidbody2D))]
public class _2D_Hinge_Drag : MonoBehaviour , _2D_IDraggable
{
	[Space(10)]

	[SerializeField()]
	bool no_rotation;
	[SerializeField()]
	bool snap_center;
	[SerializeField()]
	[Range(0,1)]
	float lerp_speed = 1f;
	
	[Header("Optional")]
	[SerializeField()][Tooltip("after drag stop , slow down the rigidbody velocity")]
	float velocity_SlowDown = 1f;
	[SerializeField()][Tooltip("after drag stop , slow down the rigidbody angular velocity")]
	float ang_velocity_SlowDown = 1f;



	HingeJoint2D _hinge_j2d;
	Rigidbody2D _rigid_2d;
	bool old_freeze_rotation;


	#if UNITY_5_5 || UNITY_5_6
	bool last_simulated;
	RigidbodyType2D last_rigidtype;
	#else
	bool old_kinematic;
	#endif






	// Use this for initialization
	void Start () 
	{
		Init ();
	}
	



	void Init()
	{
		_hinge_j2d = GetComponent<HingeJoint2D> ();
		_hinge_j2d.enabled = false;

		_rigid_2d = GetComponent<Rigidbody2D> ();
	}




	public void Start_Drag(Vector2 point , Rigidbody2D rigid = null)
	{
		_hinge_j2d.connectedBody = rigid;
		_hinge_j2d.connectedAnchor = rigid.transform.InverseTransformPoint (point);
		_hinge_j2d.anchor = transform.InverseTransformPoint (point);
		_hinge_j2d.enabled = true;

		old_freeze_rotation = _rigid_2d.freezeRotation;
		_rigid_2d.freezeRotation = no_rotation;

		#if UNITY_5_5 || UNITY_5_6
		last_simulated = _rigid_2d.simulated;
		last_rigidtype = _rigid_2d.bodyType;
		_rigid_2d.simulated = true;
		_rigid_2d.bodyType = RigidbodyType2D.Dynamic;
		#else
		old_kinematic = _rigid_2d.isKinematic;
		_rigid_2d.isKinematic = false;
		#endif
	}

	public void Update_Drag(Vector2 point)
	{
		if (snap_center)
			_hinge_j2d.anchor = Vector2.Lerp(_hinge_j2d.anchor , _rigid_2d.centerOfMass , lerp_speed);
	}

	public void Stop_Drag()
	{
		_hinge_j2d.enabled = false;
		_rigid_2d.velocity /= velocity_SlowDown;
		_rigid_2d.angularVelocity /= ang_velocity_SlowDown;
		_rigid_2d.freezeRotation = old_freeze_rotation;
		#if UNITY_5_5 || UNITY_5_6
		_rigid_2d.simulated = last_simulated;
		_rigid_2d.bodyType = last_rigidtype;
		#else
		_rigid_2d.isKinematic = old_kinematic;
		#endif
	}




	public float Get_Lerp_Speed()
	{
		return lerp_speed;
	}




	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/2DColliderPRO/Add/2D Drag/Hinge Drag")]
	static void Add_2D_Drag_Manager()
	{
		if (UnityEditor.Selection.gameObjects.Length > 0)
		{

			for (int i = 0; i < UnityEditor.Selection.gameObjects.Length; i++)
			{
				if(UnityEditor.Selection.gameObjects[i].GetComponent<Collider2D>() == null)
					UnityEditor.Selection.gameObjects[i].AddComponent<PolygonCollider2D>();

				HingeJoint2D hinge = UnityEditor.Selection.gameObjects[i].AddComponent<HingeJoint2D>();
				hinge.enabled = false;

				UnityEditor.Selection.gameObjects[i].AddComponent<_2D_Hinge_Drag>();
			}
		}
		else
			Debug.Log ("Select GameObject(s)");
	}
	#endif
}
