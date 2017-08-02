using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public Transform target;

    private Vector3 offset = new Vector3(0, 112.47f/3, -82.81417f/2);
    private float smoothing = 2.0f;

	
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing*Time.deltaTime);
        transform.LookAt(target);
	}
}
