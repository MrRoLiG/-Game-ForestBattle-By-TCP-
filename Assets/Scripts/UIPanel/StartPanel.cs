using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartPanel : BasePanel {

    private Button loginButton;
    //private Animator btnAnimator;


    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        
        loginButton = transform.Find("LoginButton").GetComponent<Button>();
        //btnAnimator = loginButton.GetComponent<Animator>();
        loginButton.onClick.AddListener(OnLoginClick);
    }
    
    /// <summary>
    /// 界面暂停
    /// </summary>
    public override void OnPause()
    {
        base.OnPause();

        //btnAnimator.enabled = false;
        //loginButton.transform.DOScale(0, 0.4f).OnComplete(()=>loginButton.gameObject.SetActive(false));
        loginButton.gameObject.SetActive(false);
    }
    /// <summary>
    /// 界面继续
    /// </summary>
    public override void OnResume()
    {
        base.OnExit();
        loginButton.gameObject.SetActive(true);
        //loginButton.transform.DOScale(1, 0.4f).OnComplete(()=>btnAnimator.enabled=true);
    }

    /// <summary>
    /// 给该界面上的登陆按钮添加一个触发事件
    /// </summary>
    private void OnLoginClick()
    {
        PlayClickSound();
        uiManager.PushPanel(UIPanelType.Login);
    }
}
