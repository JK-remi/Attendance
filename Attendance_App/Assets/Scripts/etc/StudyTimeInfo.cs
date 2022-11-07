using System.Collections;
using System.Collections.Generic;

public class StudyTimeInfo
{
    private ulong _id;
    private System.DateTime studyTime;
    private bool isSupplement = false;

    public System.DateTime StudyTime
    {
        get { return studyTime; }
    }
    public System.DayOfWeek DayOfWeek
    {
        get { return studyTime.DayOfWeek; }
    }

    public int Hour
    {
        get { return studyTime.Hour; }
    }
    public int Minute
    {
        get { return studyTime.Minute; }
    }

    public StudentInfo Student
    {
        get { return DBManager.Instance.Find(_id); }
    }

    public bool IsSupplement
    {
        get { return isSupplement; }
    }

    public string Name
    {
        get { return Student.name; }
    }

    public void Attend(bool isAttend)
    {
        Student.Attend(StudyTime, isAttend);
    }

    public StudentInfo.eAttendState IsAttend()
    {
        return Student.IsAttend(StudyTime);
    }

    public StudyTimeInfo(ulong id, System.DateTime dt, bool isSup)
    {
        _id = id;
        studyTime = dt;
        isSupplement = isSup;
    }
}
