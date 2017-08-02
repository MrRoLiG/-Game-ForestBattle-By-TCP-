using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class Arrow : MonoBehaviour {

    public float speed = 5.0f;
    private Rigidbody rigidbodyTemp;

    public RoleType roleType;

    public GameObject ExplosionEffect;

    public bool isLocal = false;
    //定义一个bool表示是否是本地客户端

	// Use this for initialization
	void Start () {
        rigidbodyTemp = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        rigidbodyTemp.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
	}

    /// <summary>
    /// 碰撞检测函数
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_ShootPerson);
            if (isLocal)
            {
                bool PlayerIsLocal = other.GetComponent<PlayerInfo>().isLocal;
                if (isLocal != PlayerIsLocal)
                {
                    GameFacade.Instance.SendAttack(Random.Range(10, 20));
                }
            }
        }
        else
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_Miss);
        }
        GameObject.Instantiate(ExplosionEffect, transform.position, transform.rotation);
        GameObject.Destroy(this.gameObject);
    }


}
