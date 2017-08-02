using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRequest : MonoBehaviour {

    protected RequestCode requestCode=RequestCode.None;
    protected ActionCode actionCode = ActionCode.None;
    protected GameFacade _facade;

    public GameFacade facade
    {
        get
        {
            if (_facade == null)
                _facade = GameFacade.Instance;
            return _facade;
        }

    }

	// Use this for initialization
	public virtual void Awake () {
        GameFacade.Instance.AddRequest(actionCode, this);
        //通过单例模式来添加request
        //facade = GameFacade.Instance;
        ////通过单例模式初始化facade
	}
	
    /// <summary>
    /// 通过GameFacade来发送请求
    /// </summary>
    /// <param name="data"></param>
    protected void SendRequest(string data)
    {
        facade.SendRequest(requestCode, actionCode, data);
    }

    /// <summary>
    /// 发起请求方法
    /// </summary>
    public virtual void SendRequest()
    {

    }

    /// <summary>
    /// 响应请求方法
    /// </summary>
    /// <param name="data">由服务器端发出的响应</param>
    public virtual void OnResponse(string data)
    {
        
    }

    /// <summary>
    /// 销毁方法
    /// </summary>
    public virtual void OnDestroy()
    {
        if (facade != null)
            facade.RemoveRequest(actionCode);
        ////通过单例模式来销毁request
    }

}
