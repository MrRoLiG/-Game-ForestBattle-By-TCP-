using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseManager {

	public AudioManager(GameFacade facade) : base(facade) { }

    private const string Sound_Prefix = "Sounds/";
    //存放音频的路径
    public const string Sound_Alert = "Alert";
    public const string Sound_ArrowShoot = "ArrowShoot";
    public const string Sound_Bg_fast = "Bg(fast)";
    public const string Sound_Bg_moderate = "Bg(moderate)";
    public const string Sound_ButtonClick = "ButtonClick";
    public const string Sound_Miss = "Miss";
    public const string Sound_ShootPerson = "ShootPerson";
    public const string Sound_Timer = "Timer";

    private AudioSource bgAudioSource;
    private AudioSource normalAudioSource;

    public override void OnInit()
    {
        base.OnInit();
        GameObject audioSourceGO = new GameObject("AudioSource(GameObject)");

        bgAudioSource = audioSourceGO.AddComponent<AudioSource>();
        normalAudioSource = audioSourceGO.AddComponent<AudioSource>();

        PlaySound(bgAudioSource, LoadSound(Sound_Bg_moderate), 0.5f,true);
    }

    /// <summary>
    /// 播放背景音乐方法，可以通过外部调用
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayBgSound(string soundName)
    {
        PlaySound(bgAudioSource, LoadSound(soundName), 0.5f, true);
    }

    /// <summary>
    /// 播放游戏其他音乐，可以通过外部调用
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayNormalSound(string soundName)
    {
        PlaySound(normalAudioSource, LoadSound(soundName), 0.5f, false);
    }

    /// <summary>
    /// 通过路径访问来访问相应的音乐
    /// </summary>
    /// <param name="soundsName">音频名</param>
    /// <returns></returns>
    private AudioClip LoadSound(string soundsName)
    {
        return Resources.Load<AudioClip>(Sound_Prefix + soundsName);
    }
    /// <summary>
    /// 播放音频方法
    /// </summary>
    /// <param name="audioSource">音频资源管理器</param>
    /// <param name="audioClip">音频</param>
    /// <param name="volume">音量大小</param>
    /// <param name="loop">是否循环播放</param>
    private void PlaySound(AudioSource audioSource, AudioClip audioClip, float volume, bool loop = false)
    {
        audioSource.volume = volume;
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.Play();
    }
}
