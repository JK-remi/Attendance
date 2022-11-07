using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StudentInfo
{
    public enum eAttendState
    {
        ABSENT = -1,
        NONE = 0,
        ATTEND = 1
    }

    public ulong id;

    public string sprite;

    public string name;
    public string nickName;

    public string phoneNumber;
    public string address;

    public System.DateTime startDT;
    public List<System.DateTime> studyTime;

    public Dictionary<System.DateTime, System.DateTime> supplementTime;

    public Dictionary<System.DateTime, bool> attendLog;

    public static StudentInfo Copy(StudentInfo info)
    {
        StudentInfo target = new StudentInfo();

        target.id = info.id;

        target.sprite = info.sprite;

        target.name = info.name;
        target.nickName = info.nickName;

        target.phoneNumber = info.phoneNumber;
        target.address = info.address;

        target.startDT = info.startDT;

        target.studyTime = info.studyTime;
        target.supplementTime = info.supplementTime;
        target.attendLog = info.attendLog;

        return target;
    }

    public void Attend(System.DateTime dt, bool isAttend)
    {
        if (attendLog == null)
        {
            attendLog = new Dictionary<System.DateTime, bool>();
        }

        if (attendLog.ContainsKey(dt) == false)
        {
            attendLog.Add(dt, isAttend);
        }
        else
        {
            attendLog[dt] = isAttend;
        }

        ////////////////////////////////////////////////////////////////////////////
        if(supplementTime == null)
        {
            supplementTime = new Dictionary<System.DateTime, System.DateTime>();
        }

        System.DateTime supKey = System.DateTime.MinValue;
        if(isAttend)
        {
            if (supplementTime.ContainsKey(dt))
            {
                supplementTime.Remove(dt);
            }
        }
        else
        {
            if(supplementTime.ContainsKey(dt) == false)
            {
                supplementTime.Add(dt, System.DateTime.MinValue);
            }
        }

        DBManager.Instance.Modify(this);
    }

    public void UpdateSupplement()
    {
        if(supplementTime == null || supplementTime.Count == 0)
        {
            return;
        }

        System.DateTime now = System.DateTime.Now;
        List<System.DateTime> keyList = supplementTime.Keys.ToList();
        foreach(System.DateTime dt in keyList)
        {
            System.DateTime target = supplementTime[dt];
            if (attendLog.ContainsKey(target)               // 보충 시간에 대한 출석 여부 기록한 경우
                && Utils.GetSpanDays(now, target) > 0)      // 시간이 하루 이상 경과한 경우
            {
                supplementTime.Remove(dt);
            }
        }
    }

    public eAttendState IsAttend(System.DateTime dt)
    {
        if(attendLog == null || attendLog.Count == 0)
        {
            return eAttendState.NONE;
        }

        if(attendLog.ContainsKey(dt) == false)
        {

            return eAttendState.NONE;
        }

        if(attendLog[dt])
        {
            return eAttendState.ATTEND;
        }
        else 
        {
            return eAttendState.ABSENT;
        }
    }

    public string GetStudyDayInfo()
    {
        if (studyTime == null || studyTime.Count == 0)
        {
            return string.Empty;
        }

        string str = "";
        for (int i = 0; i < studyTime.Count; i++)
        {
            str += studyTime[i].ToString("[ddd]요일    tt hh:mm\n");
        }

        return str;
    }

    public string GetSupDayInfo()
    {
        if(supplementTime == null || supplementTime.Count == 0)
        {
            return string.Empty;
        }

        string str = "";
        foreach(System.DateTime dt in supplementTime.Keys)
        {
            if(supplementTime[dt] == System.DateTime.MinValue)
            {
                continue;
            }

            str += supplementTime[dt].ToString("yyyy-MM-dd(ddd) tt hh:mm\n");
        }

        if (str != string.Empty)
        {
            str = "\n[보충]\n" + str;
        }

        return str;
    }

    public System.DateTime ClosestStudyTime(System.DateTime dt)
    {
        if(studyTime == null || studyTime.Count == 0)
        {
            return dt.AddYears(100);
        }

        int idx = 0, days = 0;
        int minDays = int.MaxValue;
        for(int i=0; i< studyTime.Count; i++)
        {
            days = studyTime[i].DayOfWeek - dt.DayOfWeek;
            if(days < 0)
            {
                days += Utils.DyasOfWeek;
            }

            if(minDays > days)
            {
                idx = i;
                minDays = days;
            }
        }

        System.DateTime result = new System.DateTime(dt.Year, dt.Month, dt.Day, studyTime[idx].Hour, studyTime[idx].Minute, studyTime[idx].Minute);
        return result.AddDays(minDays);
    }

    public int GetSupplementCount()
    {
        if(supplementTime == null || supplementTime.Count == 0)
        {
            return 0;
        }


        return supplementTime.Count;
    }

    public StudentInfo() { }

    public StudentInfo(StudentInfoForJson json)
    {
        if (json == null)
        {
            return;
        }

        id = json.id;

        sprite = json.sprite;

        name = json.name;
        nickName = json.nickName;

        phoneNumber = json.phoneNumber;
        address = json.address;

        startDT = Utils.UlongToDateTime(json.startDT);

        int i;
        if (json.studyTime != null && json.studyTime.Length > 0)
        {
            studyTime = new List<System.DateTime>(json.studyTime.Length);
            for (i = 0; i < json.studyTime.Length; i++)
            {
                studyTime.Add(Utils.UlongToDateTime(json.studyTime[i]));
            }
        }

        if (json.supplementLength > 0)
        {
            supplementTime = new Dictionary<System.DateTime, System.DateTime>(json.supplementLength);
            for (i = 0; i < json.supplementLength; i++)
            {
                supplementTime.Add(Utils.UlongToDateTime(json.supplementKey[i]), Utils.UlongToDateTime(json.supplementVal[i]));
            }
        }

        if (json.attendLogLength > 0)
        {
            attendLog = new Dictionary<System.DateTime, bool>(json.attendLogLength);
            for (i = 0; i < json.attendLogLength; i++)
            {
                attendLog.Add(Utils.UlongToDateTime(json.attendLogKey[i]), json.attendLogVal[i]);
            }
        }
    }
}

public class StudentInfoForJson
{
    public ulong id;

    public string sprite;

    public string name;
    public string nickName;

    public string phoneNumber;
    public string address;

    public ulong startDT;

    public ulong[] studyTime;

    public int supplementLength = 0;
    public ulong[] supplementKey;
    public ulong[] supplementVal;

    public int attendLogLength = 0;
    public ulong[] attendLogKey;
    public bool[] attendLogVal;

    public StudentInfoForJson(StudentInfo info)
    {
        if(info == null)
        {
            return;
        }

        id = info.id;

        sprite = info.sprite;

        name = info.name;
        nickName = info.nickName;

        phoneNumber = info.phoneNumber;
        address = info.address;

        startDT = Utils.DateTimeToUlong(info.startDT);

        int i;
        if(info.studyTime != null && info.studyTime.Count > 0)
        {
            studyTime = new ulong[info.studyTime.Count];
            for (i=0; i<info.studyTime.Count; i++)
            {
                studyTime[i] = Utils.DateTimeToUlong(info.studyTime[i]);
            }
        }

        if (info.supplementTime != null && info.supplementTime.Count > 0)
        {
            supplementLength = info.supplementTime.Count;
            supplementKey = new ulong[supplementLength];
            supplementVal = new ulong[supplementLength];
            i = 0;
            foreach (System.DateTime dt in info.supplementTime.Keys)
            {
                supplementKey[i] = Utils.DateTimeToUlong(dt);
                supplementVal[i] = Utils.DateTimeToUlong(info.supplementTime[dt]);
                i++;
            }
        }

        if (info.attendLog != null && info.attendLog.Count > 0)
        {
            attendLogLength = info.attendLog.Count;
            attendLogKey = new ulong[attendLogLength];
            attendLogVal = new bool[attendLogLength];
            i = 0;
            foreach (System.DateTime dt in info.attendLog.Keys)
            {
                attendLogKey[i] = Utils.DateTimeToUlong(dt);
                attendLogVal[i] = info.attendLog[dt];
                i++;
            }
        }
    }
}
