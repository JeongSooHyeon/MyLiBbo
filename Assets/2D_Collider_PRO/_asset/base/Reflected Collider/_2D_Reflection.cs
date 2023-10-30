using UnityEngine;
using System.Collections.Generic;

public class _2D_Reflection : MonoBehaviour
{

    [Space(15)]

    [SerializeField()]
    [Tooltip("Will it run at start , or you want to manually activate it ?")]
    bool activate_at_start = true;

    /*[Space(15)]

    [SerializeField()]
    string sorting_layer_name = "Default";
    [SerializeField()]
    int sorting_order = 0;*/

    [Space(15)]

    [SerializeField()]
    [Tooltip("If null , than get this transform")]
    Transform ray_source;
    [SerializeField()]
    [Range(1, 30)]
    int current_rays_count = 3;
    [SerializeField()]
    LayerMask reflection_layers;
    [SerializeField()]
    LayerMask obstacle_layers;
    [SerializeField]
    LayerMask sweetAim_layers;
    [SerializeField()]
    string Active_Objects_Tag = "Player";
    [SerializeField] string lastRay = "Bottom";

    [Space(15)]



    //LineRenderer line_rend;
    RaycastHit2D[] rhits_2d;
    //RaycastHit2D[] rhits_2d_ra;

    [SerializeField] List<Vector2> points;
    [SerializeField] Vector3[] local_points;
    Vector2 origin;

    int current_step;
    Vector2 current_origin;
    Vector2 current_dir;
    LayerMask combined_mask;
    const int max_rays = 30;

    RaycastHit2D current_obstacle;
    _2D_Active_Object last_active_object;
    _2D_Active_Object current_active_object;

    bool active = false;
    bool lineOn = false;
    bool isSweet;

    [SerializeField] float rad;






    //*******************************   PUBLIC METHODES   *********************************

    /// <summary>
    /// Start Casting the Ray
    /// </summary>
    public void Activate()
    {
        active = true;
    }

    /// <summary>
    /// Stop Casting the Ray
    /// </summary>
    public void Deactivate()
    {
        active = false;
    }

    private void Awake()
    {
        if (DataManager.instance.SweetAimPreUse || DataManager.instance.SweetAimUse) isSweet = true;
    }


    /// <summary>
    /// Set_s the current rays count.
    /// </summary>
    public void Set_Rays_Count(int count)
    {
        count = Mathf.Clamp(count, 1, max_rays);
        current_rays_count = count;
    }


    /// <summary>
    /// Get_s the raycast hits , thats have colliders.
    /// </summary>
    public RaycastHit2D[] Get_All_Active_Hits()
    {
        List<RaycastHit2D> rhits_list = new List<RaycastHit2D>();

        for (int i = 0; i < rhits_2d.Length; i++)
        {
            if (rhits_2d[i].collider != null)
                rhits_list.Add(rhits_2d[i]);
        }



        return rhits_list.ToArray();
    }


    /// <summary>
    /// Get_s the current obstacle hit info.
    /// </summary>
    public RaycastHit2D Get_Current_Obstacle_Hit()
    {
        return current_obstacle;
    }

    //***************************************************************************************

    void Start()
    {
        if (ray_source == null) // If source is null , than get this transform
            ray_source = transform;
        origin = (Vector2)ray_source.position;
        current_origin = origin;
        current_dir = (Vector2)ray_source.right;
        points = new List<Vector2>();
        points.Add(origin);
        rhits_2d = new RaycastHit2D[max_rays];
        //rhits_2d_ra = new RaycastHit2D[max_rays];

        if (isSweet) combined_mask = reflection_layers | obstacle_layers | sweetAim_layers;
        else combined_mask = reflection_layers | obstacle_layers; // combine raycast layers

        // start raycasting at start
        if (activate_at_start)
            Activate();
    }

    void Update()
    {
        if (!active)
            return;

        // first point and direction
        current_origin = (Vector2)ray_source.position;
        current_dir = (Vector2)ray_source.right;

        // reset old points
        points.Clear();
        points.Add(current_origin);

        // temp
        bool obstacle = false;
        bool active_object = false;

        for (int i = 0; i < current_rays_count; i++)
        {
            /*if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING)
                rhits_2d[i] = Physics2D.Raycast(current_origin, current_dir, Mathf.Infinity, combined_mask.value);
            // rhits_2d[i] = Physics2D.CircleCast(current_origin, 0.5f, current_dir, 1, combined_mask.value);

            else */rhits_2d[i] = Physics2D.CircleCast(current_origin, rad, current_dir, Mathf.Infinity, combined_mask.value); //  rhits_2d[i].centroid 52.29519

            if (rhits_2d[i].collider != null)
            {
                //set new origin and direction for raycast
                /*if (DataManager.instance.CurGameMode == (int)GameMode.SHOOTING) 
                    current_origin = Vector2.Lerp(current_origin, rhits_2d[i].centroid, 0.995f);
                else */current_origin = Vector2.Lerp(current_origin, rhits_2d[i].centroid, 0.995f);
                points.Add(current_origin);

                Debug.DrawLine(current_origin, current_dir, i == 0 ? Color.red : Color.blue);
                current_dir = Vector2.Reflect(current_dir, rhits_2d[i].normal);

                if (rhits_2d[i].collider.CompareTag(Active_Objects_Tag))
                {
                    current_active_object = rhits_2d[i].collider.GetComponent<_2D_Active_Object>();
                    active_object = true;

                    if (last_active_object != current_active_object)
                    {
                        current_active_object._Invoke(); // start new actvie object
                        last_active_object = current_active_object;
                    }
                }
                if (rhits_2d[i].collider.CompareTag(lastRay)) break;


                if (Mathf.RoundToInt(Mathf.Log(obstacle_layers.value, 2)) == rhits_2d[i].collider.gameObject.layer)
                {
                    obstacle = true;
                    current_obstacle = rhits_2d[i];
                    break;
                }
            }
        }

        if (!obstacle) // if no any obstacles , clear old info
            current_obstacle = new RaycastHit2D();

        if (!active_object)//if no hit with any active object , stop last active object
            if (last_active_object != null)
            {
                last_active_object._Invoke_Stop();
                last_active_object = null;
            }
    }

    public void CheckLineOn(bool b)
    {
        lineOn = b;
        if (lineOn)
            Update_DotLine();
    }

    void Update_DotLine()
    {
        bool last = false;
        for (int i = 1; i < points.Count; ++i)
        {
            if (!isSweet && i == points.Count - 1) last = true;
            DottedLineNgui.instance.DrawDottedLine(points[i - 1], points[i], last);
        }
    }

    void _Update_LineRend()
    {

        if (points.Count < 2) return;
        local_points = new Vector3[points.Count];
        int lr_p_count = (points.Count - 1) * 2;
        if (points.Count <= 1) return;


        for (int i = 0; i < local_points.Length; i++)
        {

            local_points[i] = transform.InverseTransformPoint(points[i]);

        }

        for (int i = 0; i < local_points.Length; i++)
        {

            if (i == 0)
            {
                local_points[0].z = 30;
                //line_rend.SetPosition(0, local_points[i]);
                //CheckReflectPos = local_points[i];
            }
            else
                if (i == local_points.Length - 1)
            {
                local_points[i].z = 0;
                //line_rend.SetPosition(lr_p_count - 1, local_points[i]);
            }


            if (i < local_points.Length - 2)
            {
                Vector3 v1 = Vector3.Lerp(local_points[i], local_points[i + 1], 0.985f);
                v1.z = 0;
                //line_rend.SetPosition(2 * i + 1, v1);
                Vector3 v2 = Vector3.Lerp(local_points[i + 1], local_points[i + 2], 0.985f);
                if (i == 2) v2.z = 0;
                else v2.z = 30;

                //line_rend.SetPosition(2 * i + 2, v2);
            }


        }

    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/2DColliderPRO/Add/2D Reflection/Reflection", false, 1)]
    static void Add_Reflection_Component()
    {
        if (UnityEditor.Selection.gameObjects.Length == 1)
            UnityEditor.Selection.activeGameObject.AddComponent<_2D_Reflection>();
        else
            Debug.Log("Select only 1 GameObject");
    }
#endif

}
