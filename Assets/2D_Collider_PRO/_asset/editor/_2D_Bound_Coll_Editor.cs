using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.Reflection;





[CustomEditor(typeof(_2D_Bound_Collider))]
public class _2D_Bound_Coll_Editor : Editor
{

	public SerializedProperty TriggerEnter_2D;
	public SerializedProperty TriggerExit_2D;
	public SerializedProperty TriggerStay_2D;
	public SerializedProperty CollEnter_2D;
	public SerializedProperty CollExit_2D;
	public SerializedProperty CollStay_2D;
	public SerializedProperty SelectedLayer;


	void OnEnable ()
	{
		TriggerEnter_2D = serializedObject.FindProperty ("OnTriggerEnter_2D");
		TriggerExit_2D = serializedObject.FindProperty ("OnTriggerExit_2D");
		TriggerStay_2D = serializedObject.FindProperty ("OnTriggerStay_2D");

		CollEnter_2D = serializedObject.FindProperty ("OnCollisionEnter_2D");
		CollExit_2D = serializedObject.FindProperty ("OnCollisionExit_2D");
		CollStay_2D = serializedObject.FindProperty ("OnCollisionStay_2D");

		SelectedLayer = serializedObject.FindProperty ("selected_layer");

		_2D_Bound_Collider coll_2D = (_2D_Bound_Collider)target;
		MethodInfo mi= coll_2D.GetType().GetMethod("Update_Layers", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		mi.Invoke(coll_2D,null);
	}


	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		_2D_Bound_Collider coll_2D = (_2D_Bound_Collider)target;
		SelectedLayer.intValue = EditorGUILayout.Popup ("Layer" , SelectedLayer.intValue , coll_2D.layers.ToArray());

		if(GUILayout.Button("Update Layers"))
		{
			//coll_2D.Update_Layers();
			MethodInfo mi= coll_2D.GetType().GetMethod("Update_Layers", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			mi.Invoke(coll_2D,null);
		}

		EditorGUILayout.PropertyField (TriggerEnter_2D);
		EditorGUILayout.PropertyField (TriggerExit_2D);
		EditorGUILayout.PropertyField (TriggerStay_2D);
		EditorGUILayout.PropertyField (CollEnter_2D);
		EditorGUILayout.PropertyField (CollExit_2D);
		EditorGUILayout.PropertyField (CollStay_2D);

		serializedObject.ApplyModifiedProperties ();
	}

}
