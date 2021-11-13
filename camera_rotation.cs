using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class camera_rotation : MonoBehaviour
{
    private Transform myTransform = null;
    public GameObject Target = null;
    private Transform targetTransform = null;
    private float rmin = 10;
    private float rmax = 30;
    private float max_theta = 360;
    private float min_theta = 0;
    private float max_ele = 25;
    private float min_ele = -10;

    //public enum CameraViewPointState { FIRST, SECOND, THIRD }
    /// public CameraViewPointState CameraState= CameraViewPointState.THIRD;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        if (Target != null)
        {
            targetTransform = Target.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {

        float angle = UnityEngine.Random.Range(min_theta, max_theta);
        float ele = UnityEngine.Random.Range(min_ele, max_ele);
        // determine random position on a ring around excavator
        rmax = UnityEngine.Random.Range(15,20);
        float cX = rmax*Mathf.Sin(angle * 3.14f / 180);
        float cZ = rmax*Mathf.Cos(angle * 3.14f / 180);
        Vector3 ringPos = new Vector3(cX+targetTransform.position.x, ele + targetTransform.position.y, cZ + targetTransform.position.z);
        //ringPos *= rmax;
        //Vector3 sPos = UnityEngine.Random.insideUnitSphere * rmin;
        //ringPos = ringPos + sPos;
        myTransform.position = ringPos;
        myTransform.LookAt(Target.transform.position);

    }
}
