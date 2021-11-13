using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour {

    [Header("Property")]
    private Transform myTransform = null;
    private float rand = 0.1f;
    //public int seed = 27;

    [Header("Angle Range")]
    public float minAngle = 0f; // minimum angle
    public float MaxAngle = 60f; // Maximum angle

	// Use this for initialization
	void Start () {
        //Random.seed = seed;
        myTransform = GetComponent<Transform>();        
    }
	
    // Get Random Angle
    private float RandomAngle() {
        float angle = Random.Range(minAngle, MaxAngle);
        return angle;
    }

	// Update is called once per frame
	void Update() {
        rand = RandomAngle();
        Vector3 setAngle = new Vector3(0f, 0f, rand);
        myTransform.localEulerAngles = setAngle;
    }
}
