using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_Effect_Base : MonoBehaviour
{
    [SerializeField]
    protected float time = 0f;
    [SerializeField]
    protected float delay = 0f;

    [Header("[반복 설정]")]
    [SerializeField]
    protected bool bRepeat = false;
    [SerializeField]
    protected bool bInverse = false;    // repeat 시 inverse 여부
    protected bool isInversing = false; // inverse flag

    [Header("[시작 시 발동]")]
    [SerializeField]
    protected bool isOnPlay = true;

    private Coroutine runningEffect;

    [Header("[종료 시 발동 EVENT]")]
    [SerializeField]
    private UnityEvent exitEvent;

    protected virtual void Init() { }
    protected virtual void Exit() 
    {
        if(exitEvent != null)
        {
            exitEvent.Invoke();
        }
    }
    protected virtual void Awake() { }

    protected virtual void OnEnable()
    {
        if (isOnPlay)
        {
            Play();
        }
    }

    public void Play()
    {
        Init();
        runningEffect = StartCoroutine(PlayEffect());
    }

    public void Stop()
    {
        if(IsPlaying())
        {
            StopCoroutine(runningEffect);
            runningEffect = null;
        }
    }

    public bool IsPlaying()
    {
        return runningEffect != null;
    }

    protected virtual IEnumerator PlayEffect()
    {
        yield return new WaitForSeconds(delay);
    }

    protected virtual bool IsRepeat(ref float elapsedTime)
    {
        if(bRepeat == false)
        {
            return false;
        }

        if(elapsedTime > time)
        {
            // 21. 11. 02 - 시간 보간 정확히 하기 위해 0으로 X
            elapsedTime -= time;
            
            if(bInverse)
            {
                isInversing = !isInversing;
            }
            else
            {
                Init();
            }

            return true;
        }

        return false;
    }
}
