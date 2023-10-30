using UnityEngine;
using System.Collections;



public interface _2D_IDraggable
{
	void Start_Drag(Vector2 point , Rigidbody2D rigid = null);
	void Update_Drag(Vector2 point);
	void Stop_Drag();
	float Get_Lerp_Speed();
}
