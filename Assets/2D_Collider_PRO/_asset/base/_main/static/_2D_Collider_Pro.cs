using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public enum _2D_Collider_Type
{
	Box,
	Circle,
	Edge,
	Polygon
}





public static class _2D_Collider_Pro
{


	static int max_sorting_layer_value;
	static int max_s_order;
	static int[] sorting_ID_array;
	static int[] sorting_Value_array;
	static int[] sorting_Order_array;

	static List<RaycastHit2D> max_layer_sorted_raycasts;





	/// <summary>
	/// 2D Sorting based Raycast. Returns first high-sorted sprite's collider hit info (RaycastHit2D).
	/// ***- SpriteRenderer and Collider2D will be on a same gameobject.
	/// </summary>
	/// <returns>The raycast.</returns>
	/// <param name="origin">Origin.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="distance">Distance.</param>
	/// <param name="layerMask">Layer mask.</param>
	/// <param name="minDepth">Minimum depth.</param>
	/// <param name="maxDepth">Max depth.</param>
	public static RaycastHit2D Sorting_Raycast(Vector2 origin , Vector2 direction , float distance = Mathf.Infinity , int layerMask = 1<<0 , float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity) 
	{

		RaycastHit2D[] rhit = Physics2D.RaycastAll (origin , direction , distance , layerMask , minDepth , maxDepth);
		RaycastHit2D rhit_2D = new RaycastHit2D();

		Reset();

		sorting_ID_array = new int[rhit.Length];
		sorting_Value_array = new int[rhit.Length];
		for (int i = 0; i < rhit.Length; i++)
		{
			SpriteRenderer sp_rend = rhit[i].collider.GetComponent<SpriteRenderer>();

			if(sp_rend != null)
			{
				sorting_ID_array[i] = rhit[i].collider.GetComponent<SpriteRenderer>().sortingLayerID;
				sorting_Value_array[i] = SortingLayer.GetLayerValueFromID(sorting_ID_array[i]);
			}
			else
			{
				sorting_ID_array[i] = -1;
				sorting_Value_array[i] = -100000;
			}

			if(sorting_Value_array[i] > max_sorting_layer_value)
				max_sorting_layer_value = sorting_Value_array[i];
		}


		max_layer_sorted_raycasts = new List<RaycastHit2D>();
		for (int i = 0; i < rhit.Length; i++)
		{
			if(sorting_Value_array[i] == max_sorting_layer_value)
				max_layer_sorted_raycasts.Add(rhit[i]);
		}


		sorting_Order_array = new int[max_layer_sorted_raycasts.Count];
		for (int i = 0; i < max_layer_sorted_raycasts.Count; i++)
		{
			sorting_Order_array[i] = max_layer_sorted_raycasts[i].collider.GetComponent<SpriteRenderer>().sortingOrder;
			if(sorting_Order_array[i] > max_s_order)
			{
				max_s_order = sorting_Order_array[i];
				rhit_2D = max_layer_sorted_raycasts[i];
			}
		}


		return rhit_2D;

	}





	static void Reset()
	{
		max_sorting_layer_value = SortingLayer.layers[0].value;
		max_s_order = -1000000;
	}






}
