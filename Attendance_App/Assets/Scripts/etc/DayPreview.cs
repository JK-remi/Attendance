using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayPreview : MonoBehaviour
{
    private System.DateTime _dt;
    public System.DateTime DT
    {
        get
        {
            return _dt;
        }
    }

    private Image btnImg;
    private Color originColor;

    [SerializeField]
    private Text day;

    [SerializeField]
    private Text preview;

    private void Awake()
    {
        btnImg = this.GetComponentInChildren<Image>();
        if(btnImg != null)
        {
            originColor = btnImg.color;
        }
    }

    public void Init(System.DateTime dt, string info, bool bActive)
    {
        ChangeColor(originColor);

        this.gameObject.SetActive(bActive);
        day.text = dt.Day.ToString();
        preview.text = info;

        _dt = dt;
    }

    public void ChangeColor(Color color)
    {
        if(btnImg == null)
        {
            return;
        }

        btnImg.color = color;
    }
}
