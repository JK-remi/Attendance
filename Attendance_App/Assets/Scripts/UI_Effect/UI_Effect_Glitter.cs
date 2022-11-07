using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Effect_Glitter : UI_Effect_Base
{
    [Header("[Glitter Setting]")]
    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float flickTime;

    private bool originActive;

    protected override void Awake()
    {
        if(target != null)
        {
            originActive = target.activeSelf;
        }
    }

    protected override void Init()
    {
        if(target != null)
        {
            target.SetActive(originActive);
        }
    }

    protected override IEnumerator PlayEffect()
    {
        if(target == null || flickTime <= 0f)
        {
            yield break;
        }

        yield return base.PlayEffect();

        float checkedTime = Time.time;
        float elapsedTime = 0f;

        float reservTime = flickTime;

        while (elapsedTime < time || IsRepeat(ref elapsedTime))
        {
            elapsedTime += Time.time - checkedTime;
            checkedTime = Time.time;

            if(elapsedTime >= reservTime)
            {
                reservTime += flickTime;
                target.SetActive(!target.activeSelf);
            }

            yield return null;
        }

        Exit();

        yield break;
    }
}
