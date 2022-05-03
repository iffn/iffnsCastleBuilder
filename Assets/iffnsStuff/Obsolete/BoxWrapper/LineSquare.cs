using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSquare : MonoBehaviour
{
    LineRenderer line;
    LineRenderer Line
    {
        get
        {
            if (line != null) return line;
            else
            {
                line = transform.GetComponent<LineRenderer>();
                return line;
            }
        }

    }

    public void UpdateDisplay(Vector2 size, float offset, float lineWidth)
    {
        float xPosition = size.x / 2 - lineWidth / 2 + offset;
        float yPosition = size.y / 2 - lineWidth / 2 + offset;

        Line.SetPosition(0, new Vector3(xPosition, yPosition, 0));
        Line.SetPosition(1, new Vector3(-xPosition, yPosition, 0));
        Line.SetPosition(2, new Vector3(-xPosition, -yPosition, 0));
        Line.SetPosition(3, new Vector3(xPosition, -yPosition, 0));

        Line.widthMultiplier = lineWidth;
        /*
        Line.startWidth = lineWidth;
        Line.endWidth = lineWidth;
        */
    }

    public Color LineColor
    {
        set
        {
            Line.startColor = value;
            Line.endColor = value;
        }
    }

}
