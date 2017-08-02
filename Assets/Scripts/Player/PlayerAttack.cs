using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    private Animator animator;

    private Transform leftHandTrans;

    public GameObject arrowPrefab;

    private Vector3 shotDirect;

    private PlayerManager playerManager;


	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        leftHandTrans = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
	}
	
	// Update is called once per frame
	void Update () {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit);
                if (isCollider)
                {
                    Vector3 targetPoint = hit.point;
                    targetPoint.y = transform.position.y;
                    shotDirect = targetPoint - transform.position;
                    //得到发射的方向

                    transform.rotation = Quaternion.LookRotation(shotDirect);
                    animator.SetTrigger("Attack");
                    Invoke("Shot", 0.5f);
                }
            }
        }
	}

    /// <summary>
    /// 设置PlayerManager
    /// </summary>
    /// <param name="playerManager"></param>
    public void SetPlayerManager(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
    }

    /// <summary>
    /// 发射
    /// </summary>
    /// <param name="direct">发射的方向</param>
    private void Shot()
    {
        playerManager.Shot(arrowPrefab, leftHandTrans.position, Quaternion.LookRotation(shotDirect));
    }
}
