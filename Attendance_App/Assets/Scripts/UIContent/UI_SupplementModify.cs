using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_SupplementModify : MonoBehaviour
{
    private UIPanel_ModifySupplement uiParent;

    private System.DateTime supKey;
    public System.DateTime Key
    {
        get { return supKey; }
    }

    private System.DateTime supVal;

    [SerializeField]
    private Text supDayText;
    [SerializeField]
    private Text dayOfWeekText;
    [SerializeField]
    private Dropdown apmDd;
    [SerializeField]
    private Dropdown hourDd;
    [SerializeField]
    private Dropdown minDd;

    private const int minuteTerm = 5;

    private bool isDelete = false;
    public bool IsDelete
    {
        get { return isDelete; }
    }

    public void Init(System.DateTime key, System.DateTime val, UIPanel_ModifySupplement ui)
    {
        uiParent = ui;

        SetDayText(val);

        int hourVal = val.Hour - 1;
        if (val.Hour <= Utils.HalfDayHour)
        {
            if(hourVal < 0)
            {
                hourVal = Utils.HalfDayHour - 1;
            }
            apmDd.value = 0;
        }
        else
        {
            apmDd.value = 1;
            hourVal -= Utils.HalfDayHour;
        }
        hourDd.value = hourVal;

        minDd.value = val.Minute / 5;

        supKey = key;
        supVal = val;
    }

    public void SetSupplementTime(System.DateTime dt)
    {
        supVal = new System.DateTime(dt.Year, dt.Month, dt.Day);

        SetDayText(dt);
    }

    private void SetDayText(System.DateTime dt)
    {
        supDayText.text = dt.ToString("yyyy-MM-dd");

        dayOfWeekText.text = dt.ToString("ddd");
    }

    public System.DateTime SupplementTime()
    {
        if(supVal == System.DateTime.MinValue)
        {
            return supVal;
        }

        System.DateTime studyTime = new System.DateTime(supVal.Year, supVal.Month, supVal.Day);
        int hour = hourDd.value + 1;
        if(hour == Utils.HalfDayHour)
        {
            if(apmDd.value == 0)
            {
                hour = 0;
            }
            else
            {
                hour = Utils.HalfDayHour;
            }
        }
        else
        {
            hour += apmDd.value * Utils.HalfDayHour;
        }
        studyTime = studyTime.AddHours(hour);
        studyTime = studyTime.AddMinutes(minDd.value * minuteTerm);

        return studyTime;
    }

    public void OpenSelectDay()
    {
        if(UIManager.Instance.OpenUI(UIManager.UI_type.UI_SELECT_SUP_DAY, true))
        {
            System.DateTime dt = System.DateTime.Now;
            if(supVal != System.DateTime.MinValue       // 보충 시간 설정되지 않았을 경우
                && Utils.GetSpanDays(supVal, dt) >= 0)  // 보충 시간이 오늘 날짜보다 이전인 경우
            {
                dt = supVal;
            }
            ((UIPanel_SelectSupDay)UIManager.Instance.CurUI).SetMonth(dt, this);
        }
    }

    public void Delete()
    {
        if(isDelete)
        {
            return;
        }

        isDelete = true;
        if (UIManager.Instance.OpenUI(UIManager.UI_type.UI_OK_CANCEL, true))
        {
            // ok cancel ui의 ok button event에 DeleteStudent() 연결
            ((UIPanel_OK_Cancel)UIManager.Instance.CurUI).SetNotice("정말 삭제 하시겠습니까?", new UnityAction(uiParent.DeleteSupplementTime));
        }
    }
}
