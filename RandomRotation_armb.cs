using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation_armb : MonoBehaviour {

    [Header("Property")]
    private Transform myTransform = null;
    private float rand = 0.1f;

    [Header("Angle Range")]
    public float minAngle = -60f; // minimum angle
    public float MaxAngle = 20f; // Maximum angle

	// Use this for initialization
	void Start () {
        /*
         GameObject seedContainer = GameObject.FindGameObjectWithTag("objRotation");
         RandomRotation seedScript;
         seedScript = seedContainer.GetComponent<RandomRotation>();
         int seed = seedScript.seed;
         Random.seed = seed;
         */
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
        Vector3 setAngle = new Vector3(0f,0f, rand);
        myTransform.localEulerAngles = setAngle;
    }
}
