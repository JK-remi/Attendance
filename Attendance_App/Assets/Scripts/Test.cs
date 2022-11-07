using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<System.DateTime, bool> testDic = new Dictionary<System.DateTime, bool>();
        System.DateTime dt1 = new System.DateTime(2022, 3, 1);
        System.DateTime dt2 = new System.DateTime(2000, 1, 3);
        System.DateTime dt3 = System.DateTime.MinValue;
        System.DateTime dt4 = new System.DateTime(2088, 11, 24);
        testDic.Add(dt1, false);
        testDic.Add(dt2, true);
        testDic.Add(dt3, true);
        testDic.Add(dt4, false);

        foreach(System.DateTime key in testDic.Keys.ToList())
        {
            Debug.Log(key);
            if(testDic[key] == false)
            {
                testDic.Remove(key);
                Debug.Log("remove");
            }
        }
    }
}
