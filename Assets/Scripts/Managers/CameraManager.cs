using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : BaseManager {
    
    public CameraManager(GameFacade facade) : base(facade) { }

    private GameObject cameraGo;
    private Animator cameraAnim;
    private FollowTarget followTarget;

    private Vector3 originalPostion;
    private Vector3 originalRotation;

    public override void OnInit()
    {
        cameraGo = Camera.main.gameObject;
        cameraAnim = cameraGo.GetComponent<Animator>();
        followTarget = cameraGo.GetComponent<FollowTarget>();

    }

    //public override void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        //FollowTarget((GameObject.Find("Hunter_BLUE") as GameObject).transform);
    //        FollowTarget(null);
    //    }
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        WalkThroughScene();
    //    }
    //}

    /// <summary>
    /// 从漫游状态切换到跟随状态
    /// </summary>
    /// <param name="target"></param>
    public void FollowRole()
    {
        followTarget.target =facade.GetCurrentRoleGameObject().transform;

        cameraAnim.enabled = false;
        originalPostion = cameraGo.transform.position;
        originalRotation = cameraGo.transform.eulerAngles;

        Quaternion targetQuaternion = Quaternion.LookRotation(followTarget.target.position - cameraGo.transform.position);
        cameraGo.transform.DORotateQuaternion(targetQuaternion, 1.0f).OnComplete(delegate
        {
            followTarget.enabled = true;
        });
        
    }

    /// <summary>
    /// 从跟随状态切换到漫游状态
    /// </summary>
    public void WalkThroughScene()
    {
        followTarget.enabled = false;
        cameraGo.transform.DOMove(originalPostion, 1.0f);
        cameraGo.transform.DORotate(originalRotation, 1.0f).OnComplete(delegate() {
            cameraAnim.enabled = true;
        });
    }
}
