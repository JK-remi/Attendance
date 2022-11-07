using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Effect_TweenColor : UI_Effect_Base
{
    [Header("[TweenColor Setting]")]
    private MaskableGraphic graphic;
    private Color originColor;
    [SerializeField]
    private Color wantedColor;

    protected override void Awake()
    {
        graphic = this.GetComponent<MaskableGraphic>();
        originColor = graphic.color;
    }

    protected override void Init()
    {
        graphic.color = originColor;
    }

    protected override IEnumerator PlayEffect()
    {
        yield return base.PlayEffect();

        float checkedTime = Time.time;
        float elapsedTime = 0f;

        Color curColor = originColor;
        while (elapsedTime < time || IsRepeat(ref elapsedTime))
        {
            elapsedTime += Time.time - checkedTime;
            checkedTime = Time.time;

            if (isInversing == false)
            {
                curColor.r = Mathf.Lerp(originColor.r, wantedColor.r, elapsedTime / time);
                curColor.g = Mathf.Lerp(originColor.g, wantedColor.g, elapsedTime / time);
                curColor.b = Mathf.Lerp(originColor.b, wantedColor.b, elapsedTime / time);
                curColor.a = Mathf.Lerp(originColor.a, wantedColor.a, elapsedTime / time);
            }
            else
            {
                curColor.r = Mathf.Lerp(wantedColor.r, originColor.r, elapsedTime / time);
                curColor.g = Mathf.Lerp(wantedColor.g, originColor.g, elapsedTime / time);
                curColor.b = Mathf.Lerp(wantedColor.b, originColor.b, elapsedTime / time);
                curColor.a = Mathf.Lerp(wantedColor.a, originColor.a, elapsedTime / time);
            }
            graphic.color = curColor;

            yield return null;
        }

        Exit();

        yield break;
    }
}
