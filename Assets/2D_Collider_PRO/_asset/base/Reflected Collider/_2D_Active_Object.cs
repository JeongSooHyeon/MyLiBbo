using UnityEngine;
using UnityEngine.Events;
using System.Collections;




/// <summary>
/// Helper component for 2D Reflection. Don't edit
/// </summary>
public class _2D_Active_Object : MonoBehaviour 
{
	[Space(20)]
	
	public UnityEvent On_Raycast;
	public UnityEvent On_Raycast_Stop;
	


	public void _Invoke()
	{
		if (On_Raycast != null)
			On_Raycast.Invoke();
	}


	public void _Invoke_Stop()
	{
		if (On_Raycast_Stop != null)
			On_Raycast_Stop.Invoke();
	}




	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/2DColliderPRO/Add/2D Reflection/Active Object")]
	static void Add_Active_Component()
	{
		if (UnityEditor.Selection.gameObjects.Length == 1)
			UnityEditor.Selection.activeGameObject.AddComponent<_2D_Active_Object> ();
		else
			Debug.Log ("Select only 1 GameObject");
	}
	#endif
}
