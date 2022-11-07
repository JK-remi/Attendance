using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Effect_TweenPos : UI_Effect_Base
{
    private RectTransform targetTrans;
    private Vector3 originPos;
    [Header("[TweenPos Setting]")]
    [SerializeField]
    private Vector3 wantedPos;

    protected override void Awake()
    {
        targetTrans = this.GetComponent<RectTransform>();
        originPos = targetTrans.anchoredPosition3D;
    }

    protected override void Init()
    {
        targetTrans.anchoredPosition3D = originPos;
    }

    protected override IEnumerator PlayEffect()
    {
        yield return base.PlayEffect();

        float checkedTime = Time.time;
        float elapsedTime = 0f;

        Vector3 moveDir = wantedPos - originPos;
        Vector3 curMove;
        float moveDist = moveDir.magnitude;

        while(elapsedTime < time || IsRepeat(ref elapsedTime))
        {
            elapsedTime += Time.time - checkedTime;
            checkedTime = Time.time;

            if (isInversing == false)
            {
                curMove = moveDir.normalized * Mathf.Lerp(0f, moveDist, elapsedTime / time);
            }
            else
            {
                curMove = moveDir.normalized * Mathf.Lerp(moveDist, 0f, elapsedTime / time);
            }
            targetTrans.anchoredPosition3D = originPos + curMove;

            yield return null;
        }

        Exit();

        yield break;
    }
}
