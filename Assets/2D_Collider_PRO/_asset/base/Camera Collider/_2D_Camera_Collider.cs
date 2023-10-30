using UnityEngine;
using System.Collections;




/// <summary>
/// Creates 2D EdgeCollider from camera borders.
/// Place on 2D(Othographic) Camera.
/// </summary>
public class _2D_Camera_Collider : MonoBehaviour 
{
	[Space(20)]
	
	[SerializeField()]
	bool set_collider_at_start = true;
	[SerializeField()]
	bool is_dynamic = true;
	[SerializeField()]
	bool is_triggert = true;
	[SerializeField()]
	bool use_effector = false;
	[SerializeField()]
	PhysicsMaterial2D physics_Material_2D;


	// private
	EdgeCollider2D edgeCol2D;
	Camera _camera;



	void Awake ()
	{
		if (GetComponent<Camera> () != null)
			_camera = GetComponent<Camera> ();

		_camera.orthographic = true;

		if(set_collider_at_start)
			SetCollider (is_triggert , use_effector , physics_Material_2D);
	}





	void FixedUpdate()
	{
		if (!is_dynamic)
			return;

		Update_Collider_Size ();
	}





	// +++++++++++++++++++++++++++++++++++++    PUBLIC METHODES    ++++++++++++++++++++++++++++++++++++++++++


	/// <summary>
	/// Sets 2D Edge Collider to Orthographic Camera.
	/// </summary>
	public void SetCollider(bool _is_trigger , bool _use_effector , PhysicsMaterial2D _physics_Material_2D)
	{
		// camera border points
		Vector2 bottomLeft = (Vector2)(_camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane)) - transform.position );
		Vector2 topLeft = (Vector2)(_camera.ScreenToWorldPoint(new Vector3(0, _camera.pixelHeight, _camera.nearClipPlane)) - transform.position );
		Vector2 topRight = (Vector2)(_camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight, _camera.nearClipPlane)) - transform.position );
		Vector2 bottomRight = (Vector2)(_camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, 0, _camera.nearClipPlane)) - transform.position );

		// edge collider creation
		edgeCol2D = gameObject.AddComponent<EdgeCollider2D>();
		Vector2[] points = new Vector2[]{bottomLeft,topLeft,topRight,bottomRight, bottomLeft};
		edgeCol2D.points = points;
		edgeCol2D.isTrigger = _is_trigger;
		edgeCol2D.usedByEffector = _use_effector;
		edgeCol2D.sharedMaterial = _physics_Material_2D;
	}



	/// <summary>
	/// Manual Update_s the size of the camera collider.
	/// </summary>
	public void Update_Collider_Size()
	{
		// camera border points
		Vector2 bottomLeft = (Vector2)_camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
		Vector2 topLeft = (Vector2)_camera.ScreenToWorldPoint(new Vector3(0, _camera.pixelHeight, _camera.nearClipPlane));
		Vector2 topRight = (Vector2)_camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight, _camera.nearClipPlane));
		Vector2 bottomRight = (Vector2)_camera.ScreenToWorldPoint (new Vector3 (_camera.pixelWidth, 0, _camera.nearClipPlane));

		Vector2[] points = new Vector2[]{(Vector2)transform.InverseTransformPoint(bottomLeft),
										(Vector2)transform.InverseTransformPoint(topLeft),
										(Vector2)transform.InverseTransformPoint(topRight),
			                            (Vector2)transform.InverseTransformPoint(bottomRight),
										(Vector2)transform.InverseTransformPoint(bottomLeft)};

		edgeCol2D.points = points;
	}


	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++





#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/2DColliderPRO/Add/Camera Collider")]
	static void Add_Camera_Collider()
	{
		if (UnityEditor.Selection.gameObjects.Length == 1)
		{
			if (UnityEditor.Selection.activeGameObject.GetComponent<Camera>() != null)
				UnityEditor.Selection.activeGameObject.AddComponent<_2D_Camera_Collider> ();
			else
				Debug.Log ("Select only Camera gameobject to add this component");
		}
		else
			Debug.Log ("Select only 1 GameObject");
	}
#endif

}
