using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    private float speed = 3.0f;

    public float forward = 0;

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate() {

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") == false)
            return;
        //如果角色当前处于攻击状态的时候，停止移动
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)
        {
            transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.LookRotation(new Vector3(h, 0, v));

            float res = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));
            forward = res;
            animator.SetFloat("Forward", res);
        }
	}

    
}
