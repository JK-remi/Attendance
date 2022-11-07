using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    private static DBManager _instance = null;
    public static DBManager Instance
    {
        get
        {
            _instance = GameObject.FindObjectOfType<DBManager>();
            if (_instance == null)
            {
                GameObject container = new GameObject("DBManager");
                _instance = container.AddComponent<DBManager>();
            }

            return _instance;
        }
    }

    private Dictionary<ulong, StudentInfo> studentDic;
    public ICollection<ulong> IDs
    {
        get { return studentDic.Keys; }
    }

    private StudentInfo modifiedStudent;
    public StudentInfo ModifiedStudent
    {
        get { return modifiedStudent; }
    }

    private void Awake()
    {
        Load();
    }

    public List<StudyTimeInfo> GetStudyList(System.DateTime dt)
    {
        List<StudyTimeInfo> studyList = new List<StudyTimeInfo>();
        foreach(ulong id in IDs)
        {
            StudentInfo info = null;
            studentDic.TryGetValue(id, out info);

            if (info == null)
            {
                continue;
            }

            // 등록된 시간 이전이라면 제외
            if(Utils.GetSpanDays(dt, info.startDT) < 0)
            {
                continue;
            }

            // 기존 study time find & add
            if(info.studyTime != null && info.studyTime.Count > 0)
            {
                for (int i = 0; i < info.studyTime.Count; i++)
                {
                    System.DateTime st = info.studyTime[i];
                    if (dt.DayOfWeek == info.studyTime[i].DayOfWeek)
                    {
                        studyList.Add(new StudyTimeInfo(id,     
                                                        new System.DateTime(dt.Year, dt.Month, dt.Day, st.Hour, st.Minute, 0), 
                                                        false));
                    }
                }
            }

            // 보충 time find & add
            if (info.supplementTime != null && info.supplementTime.Count > 0)
            {
                foreach (System.DateTime key in info.supplementTime.Keys)
                {
                    System.DateTime supt = info.supplementTime[key];
                    if (Utils.GetSpanDays(dt, info.supplementTime[key]) == 0)
                    {
                        studyList.Add(new StudyTimeInfo(id, info.supplementTime[key], true));
                    }
                }
            }
        }

        return studyList.OrderBy(x => x.StudyTime).ToList();
    }

    public ulong MakeID(System.DateTime dt)
    {
        ulong id = (ulong)dt.Year       * 10000000000 
                    + (ulong)dt.Month   * 100000000
                    + (ulong)dt.Day     * 1000000
                    + (ulong)dt.Hour    * 10000
                    + (ulong)dt.Minute  * 100
                    + (ulong)dt.Second;
        return id;
    }

    public void Modify(StudentInfo info)
    {
        modifiedStudent = info;

        if(studentDic.ContainsKey(info.id) == false)
        {
            studentDic.Add(info.id, info);
        }
        else
        {
            studentDic[info.id] = info;
        }

        Save();
    }

    public void Remove(ulong id)
    {
        studentDic.Remove(id);
    }

    private string PathForDocumetsFile(string filename)
    {
        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), filename);
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
    }

    private void Save()
    {
        string jsonData = string.Empty;
        foreach (ulong id in studentDic.Keys)
        {
            studentDic[id].UpdateSupplement();
            StudentInfoForJson json = new StudentInfoForJson(studentDic[id]);
            jsonData += JsonUtility.ToJson(json) + "\n";
        }

        string path = PathForDocumetsFile("Database.json");

        if (File.Exists(path) == false)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(jsonData);
            sw.Close();
            fs.Close();
        }
        else
        {
            File.WriteAllText(path, jsonData);
        }
    }

    private void Load()
    {
        string path = PathForDocumetsFile("Database.json");
        if (File.Exists(path) == false)
        {
            studentDic = new Dictionary<ulong, StudentInfo>();
            return;
        }

        string jsonData = File.ReadAllText(path);
        string[] datas = jsonData.Split('\n');
        studentDic = new Dictionary<ulong, StudentInfo>(datas.Length);
        for (int i = 0; i < datas.Length; i++)
        {
            if(datas[i] == string.Empty)
            {
                continue;
            }

            StudentInfoForJson json = JsonUtility.FromJson<StudentInfoForJson>(datas[i]);
            StudentInfo info = new StudentInfo(json);

            studentDic.Add(info.id, info);
        }
    }

    public StudentInfo Find(ulong id)
    {
        StudentInfo info;
        studentDic.TryGetValue(id, out info);

        return info;
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
