using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_StudyTimeModify : MonoBehaviour
{
    private UIPanel_ModifyStudyTime uiParent;

    [SerializeField]
    private Dropdown dayDd;
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

    public void Init(System.DateTime dt, UIPanel_ModifyStudyTime ui)
    {
        uiParent = ui;

        dayDd.value = (int)dt.DayOfWeek;

        int hourVal = dt.Hour - 1;
        if (dt.Hour <= Utils.HalfDayHour)
        {
            if (hourVal < 0)
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

        minDd.value = dt.Minute / 5;
    }

    public System.DateTime StudyTime()
    {

        System.DateTime studyTime = new System.DateTime();
        int days = (int)(dayDd.value - studyTime.DayOfWeek) + 7;    // 0001.01.01 = monday
        studyTime = studyTime.AddDays(days);
        int hour = hourDd.value + 1;
        if (hour == Utils.HalfDayHour)
        {
            if (apmDd.value == 0)
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

    public void Delete()
    {
        isDelete = true;
        if (UIManager.Instance.OpenUI(UIManager.UI_type.UI_OK_CANCEL, true))
        {
            // ok cancel ui의 ok button event에 DeleteStudent() 연결
            ((UIPanel_OK_Cancel)UIManager.Instance.CurUI).SetNotice("정말 삭제 하시겠습니까?", new UnityAction(uiParent.DeleteStudyTime));
        }
    }
}
