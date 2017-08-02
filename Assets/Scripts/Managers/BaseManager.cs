using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager {

    protected GameFacade facade;
    public BaseManager(GameFacade facade)
    {
        this.facade = facade;
    }

    public virtual void OnInit() { }

	public virtual void OnDestroy() { }

    /// <summary>
    /// 用来实现异步的pushPanel
    /// </summary>
    public virtual void Update(){ }
}
