using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class Utils
{
    public enum eMonth
    {
        January = 1,
        Febuary,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public static string GetEnumDescription(System.Type type, string name)
    {
        string result = string.Empty;

        MemberInfo[] info = type.GetMember(name);
        if (info != null && info.Length > 0)
        {
            object[] attrs = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs != null && attrs.Length > 0)
            {
                result = ((DescriptionAttribute)attrs[0]).Description;
            }
        }

        return result;
    }

    private const string clone = "(Clone)";
    public static string SubClone(string target)
    {
        int idx = target.IndexOf(clone);
        if (idx < 0)
        {
            return target;
        }

        return target.Substring(0, idx);
    }

    public const int HalfDayHour = 12;
    public const int DyasOfWeek = 7;

    public static System.DateTime UlongToDateTime(ulong ul)
    {
        ulong rest = ul;

        int year = (int)(rest / 100000000);
        rest = rest % 100000000;

        int month = (int)(rest / 1000000); 
        rest = rest % 1000000;

        int day = (int)(rest / 10000); 
        rest = rest % 10000;

        int hour = (int)(rest / 100);
        rest = rest % 100;

        int minute = (int)rest;

        return new System.DateTime(year, month, day, hour, minute, 0);
    }
    
    public static ulong DateTimeToUlong(System.DateTime dt)
    {
        ulong result = (ulong)dt.Year       * 100000000
                        + (ulong)dt.Month   * 1000000
                        + (ulong)dt.Day     * 10000
                        + (ulong)dt.Hour    * 100
                        + (ulong)dt.Minute;
        return result;
    }

    public static int GetSpanDays(System.DateTime dt1, System.DateTime dt2)     // dt1 - dt2;
    {
        System.DateTime changedDt1 = new System.DateTime(dt1.Year, dt1.Month, dt1.Day);
        System.DateTime changedDt2 = new System.DateTime(dt2.Year, dt2.Month, dt2.Day);

        int days = (changedDt1 - changedDt2).Days;

        return days;
    }
}
