using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxLineWrapper : MonoBehaviour
{
    public LineSquare TopLineSqure;
    public LineSquare BottomLineSqure;
    public LineSquare FrontLineSqure;
    public LineSquare BackLineSqure;
    public LineSquare LeftLineSqure;
    public LineSquare RightLineSqure;

    public Vector3 size = Vector3.one / 3f;
    public Vector3 Size
    {
        set
        {
            size = value;

            UpdateDisplay();
        }
    }

    public float lineWidth = 0.1f;
    public float offset = 0.01f;

    void UpdateDisplay()
    {
        bool showSides = size.y != 0;

        FrontLineSqure.gameObject.SetActive(showSides);
        BackLineSqure.gameObject.SetActive(showSides);
        LeftLineSqure.gameObject.SetActive(showSides);
        RightLineSqure.gameObject.SetActive(showSides);

        TopLineSqure.transform.localPosition = Vector3.up * (size.y / 2 + offset);
        BottomLineSqure.transform.localPosition = Vector3.down * (size.y / 2 + offset);
        FrontLineSqure.transform.localPosition = Vector3.forward * (size.z / 2 + offset);
        BackLineSqure.transform.localPosition = Vector3.back * (size.z / 2 + offset);
        LeftLineSqure.transform.localPosition = Vector3.left * (size.x / 2 + offset);
        RightLineSqure.transform.localPosition = Vector3.right * (size.x / 2 + offset);

        TopLineSqure.UpdateDisplay(size: new Vector2(size.x, size.z), offset: offset, lineWidth: lineWidth);
        BottomLineSqure.UpdateDisplay(size: new Vector2(size.x, size.z), offset: offset, lineWidth: lineWidth);
        FrontLineSqure.UpdateDisplay(size: new Vector2(size.x, size.y), offset: offset, lineWidth: lineWidth);
        BackLineSqure.UpdateDisplay(size: new Vector2(size.x, size.y), offset: offset, lineWidth: lineWidth);
        LeftLineSqure.UpdateDisplay(size: new Vector2(size.z, size.y), offset: offset, lineWidth: lineWidth);
        RightLineSqure.UpdateDisplay(size: new Vector2(size.z, size.y), offset: offset, lineWidth: lineWidth);
    }

    public Color LineColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();

        TopLineSqure.LineColor = LineColor;
        BottomLineSqure.LineColor = LineColor;
        FrontLineSqure.LineColor = LineColor;
        BackLineSqure.LineColor = LineColor;
        LeftLineSqure.LineColor = LineColor;
        RightLineSqure.LineColor = LineColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
