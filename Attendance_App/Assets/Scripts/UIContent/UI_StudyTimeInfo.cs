using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class UI_StudyTimeInfo : MonoBehaviour
{
    private StudyTimeInfo dayInfo;

    [SerializeField]
    private SpriteAtlas atlas;

    [SerializeField]
    private Image iconImg;
    [SerializeField]
    private TMPro.TMP_Text nameText;
    [SerializeField]
    private GameObject suppText;
    [SerializeField]
    private TMPro.TMP_Text apmText;
    [SerializeField]
    private TMPro.TMP_Text hourText;
    [SerializeField]
    private TMPro.TMP_Text minuteText;

    [SerializeField]
    private Image attendImg;
    private Color originColor;
    [SerializeField]
    private Color attendColor;
    [SerializeField]
    private Color absentColor;
    private int attendCheck = 0;

    private void Awake()
    {
        if(attendImg != null)
        {
            originColor = attendImg.color;
        }
    }

    public void Init(StudyTimeInfo sti)
    {
        if(iconImg != null)
        {
            iconImg.sprite = atlas.GetSprite(sti.Student.sprite);
        }

        nameText.text = sti.Name;

        suppText.SetActive(sti.IsSupplement);

        int hour = sti.Hour;
        if(hour > Utils.HalfDayHour)
        {
            apmText.text = "오후";
            hour -= Utils.HalfDayHour;
        }
        else
        {
            apmText.text = "오전";
        }
        hourText.text = hour.ToString("D2");
        minuteText.text = sti.Minute.ToString("D2");

        if (attendImg != null)
        {
            // 선택된 날짜가 오늘 이후에만 출석 체크 가능하도록 설정
            attendImg.gameObject.SetActive(Utils.GetSpanDays(System.DateTime.Now, sti.StudyTime) >= 0);

            switch (sti.IsAttend())
            {
                case StudentInfo.eAttendState.NONE:
                    {
                        attendCheck = 0;
                        attendImg.color = originColor;
                        break;
                    }
                case StudentInfo.eAttendState.ATTEND:
                    {
                        attendCheck = 1;
                        attendImg.color = attendColor;
                        break;
                    }
                case StudentInfo.eAttendState.ABSENT:
                    {
                        attendCheck = 2;
                        attendImg.color = absentColor;
                        break;
                    }
            }
        }

        dayInfo = sti;
    }

    public void Attendance()
    {
        if(attendImg == null)
        {
            return;
        }

        ++attendCheck;

        if(attendCheck % 2 == 1)    // 출석
        {
            dayInfo.Attend(true);
            attendImg.color = attendColor;
        }
        else                        // 불참
        {
            dayInfo.Attend(false);
            attendImg.color = absentColor;
        }
    }
}
