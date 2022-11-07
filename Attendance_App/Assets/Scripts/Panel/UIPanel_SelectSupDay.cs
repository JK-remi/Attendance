using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_SelectSupDay : UIPanel
{
    private UI_SupplementModify uiParent;

    [SerializeField]
    private Text year;
    [SerializeField]
    private Text month;

    [SerializeField]
    private List<Text> dayList;

    [SerializeField]
    private Color originColor;
    [SerializeField]
    private Color todayColor;
    [SerializeField]
    private Color selectedColor;

    private System.DateTime curDT;
    private System.DateTime selectedDT;

    private void Awake()
    {
        uiType = UIManager.UI_type.UI_SELECT_SUP_DAY;
    }

    protected override void Init()
    {
        base.Init();

        uiParent = null;
        curDT = System.DateTime.MinValue;
        selectedDT = System.DateTime.MinValue;
    }

    public void SetMonth(System.DateTime dt, UI_SupplementModify ui)
    {
        uiParent = ui;
        SetMonth(dt);
    }

    public void SetMonth(System.DateTime dt)
    {
        year.text = dt.Year.ToString();
        month.text = ((Utils.eMonth)dt.Month).ToString();

        curDT = new System.DateTime(dt.Year, dt.Month, 1);
        int lastDay = System.DateTime.DaysInMonth(dt.Year, dt.Month);

        int dayCount = 0;
        for (int i = 0; i < dayList.Count; i++)
        {
            if ((dayCount == 0 && i < (int)curDT.DayOfWeek)
                || dayCount >= lastDay)
            {
                dayList[i].text = string.Empty;
                dayList[i].gameObject.SetActive(false);
                continue;
            }

            System.DateTime settingDT = curDT.AddDays(dayCount++);
            dayList[i].text = settingDT.ToString("dd");
            ChangeColor(dayList[i].gameObject, originColor);

            if (Utils.GetSpanDays(settingDT, selectedDT) == 0)
            {
                ChangeColor(dayList[i].gameObject, selectedColor);
            }
            else if (Utils.GetSpanDays(settingDT, System.DateTime.Now) == 0)
            {
                ChangeColor(dayList[i].gameObject, todayColor);
            }
        }
    }

    private void ChangeColor(GameObject obj, Color color)
    {
        Image img = obj.GetComponentInChildren<Image>();
        if(img == null)
        {
            return;
        }

        img.color = color;
    }

    public void ChangeMonth(int addMonth)
    {
        System.DateTime changeDT = curDT.AddMonths(addMonth);
        SetMonth(changeDT);
    }

    public void OpenDayPreview(Text dayText)
    {
        System.DateTime inputDt = curDT.AddDays(System.Convert.ToInt32(dayText.text) - 1);
        if(Utils.GetSpanDays(inputDt, System.DateTime.Now) < 0)     // 오늘 이전 일 선택 불가
        {
            return;
        }

        if(selectedDT == inputDt)                                   // 이 전에 선택된 시간인 경우 선택 취소
        {
            selectedDT = System.DateTime.MinValue;
            if (Utils.GetSpanDays(inputDt, System.DateTime.Now) == 0)
            {
                ChangeColor(dayText.gameObject, todayColor);
            }
            else
            {
                ChangeColor(dayText.gameObject, originColor);
            }
            return;
        }

        if(isClicked)
        {
            return;
        }
        isClicked = true;

        if (UIManager.Instance.OpenUI(UIManager.UI_type.UI_SUP_DAY_INFO, true))
        {
            ((UIPanel_CheckDayInfo)UIManager.Instance.CurUI).SetDayInfo(inputDt, this);
            isClicked = false;
        }
    }

    public void SetSelectDay(System.DateTime dt)
    {
        selectedDT = dt;
        SetMonth(selectedDT);
    }

    public void Save()
    {
        if (isClicked)
        {
            return;
        }
        isClicked = true;

        // 변경된 보충 시간 저장
        if (UIManager.Instance.CloseUI())
        {
            uiParent.SetSupplementTime(selectedDT);
            isClicked = false;
        }
    }

    public void Quit()
    {
        if (isClicked)
        {
            return;
        }
        isClicked = true;

        if (UIManager.Instance.CloseUI())
        {
            isClicked = false;
        }
    }
}
