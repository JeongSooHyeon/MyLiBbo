using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Camera))]
public class _2D_Drag_Manager : MonoBehaviour 
{
	//[Space(10)]

	Camera _camera;
	RaycastHit2D rhit_2d;
	Vector3 screen_pos;
	Vector3 world_pos;
	Vector3 hit_point;
	float lerp_speed = 1f;
	_2D_IDraggable drag_object;
	Rigidbody2D temp_rigid_2d;



	void Awake ()
	{
		_camera = GetComponent<Camera> ();
		if (!_camera)
			_camera = Camera.main;

		temp_rigid_2d = transform.Find("temp_rigid").GetComponent<Rigidbody2D> ();
	}



	// Update is called once per frame
	void Update () 
	{

		if (Input.GetMouseButtonDown (0)) 
		{
			screen_pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -1 * _camera.transform.position.z);
			world_pos = _camera.ScreenToWorldPoint (screen_pos);

			rhit_2d = Physics2D.Raycast (world_pos, _camera.transform.forward, 100);

			if (rhit_2d.collider) 
			{
				_2D_IDraggable draggable = rhit_2d.collider.GetComponent<_2D_IDraggable>();
				hit_point = rhit_2d.point;

				if(draggable != null)
				{
					drag_object = draggable;
					temp_rigid_2d.mass = 1000;
					temp_rigid_2d.transform.position = world_pos;
					lerp_speed = drag_object.Get_Lerp_Speed();
					drag_object.Start_Drag(world_pos , temp_rigid_2d);
				}
			}
		}



		if(drag_object != null)
		if (Input.GetMouseButton(0)) 
		{
			screen_pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -1 * _camera.transform.position.z);
			world_pos = _camera.ScreenToWorldPoint (screen_pos);

			Vector3 vlerp = Vector3.Lerp(temp_rigid_2d.transform.position , world_pos , lerp_speed);
			temp_rigid_2d.MovePosition(vlerp);
			drag_object.Update_Drag(vlerp);
		}



		if(drag_object != null)
		if (Input.GetMouseButtonUp(0)) 
		{
			temp_rigid_2d.isKinematic = false;
			temp_rigid_2d.mass = 0;
			drag_object.Stop_Drag();
			drag_object = null;
		}

	}






	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/2DColliderPRO/Add/2D Drag/Drag Manager" , false , 1)]
	static void Add_2D_Drag_Manager()
	{
		if (UnityEditor.Selection.gameObjects.Length == 1)
		{
			UnityEditor.Selection.activeGameObject.AddComponent<_2D_Drag_Manager>();

			GameObject go = new GameObject("temp_rigid");
			Rigidbody2D rigidbody = go.AddComponent<Rigidbody2D>();
			rigidbody.isKinematic = true;
			go.transform.SetParent(UnityEditor.Selection.activeGameObject.transform);
		}
		else
			Debug.Log ("Select only 1 GameObject");
	}
	#endif

}
