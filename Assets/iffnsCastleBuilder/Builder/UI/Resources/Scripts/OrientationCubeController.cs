using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationCubeController : MonoBehaviour
{
    [SerializeField] RectTransform viewMarker;

    float cubeWidth = 150;
    float xOffset = 75 - 150;

    public Vector2 input;
    public Vector2 output;

    public void SetViewAngles(float headingAngleDeg, float tiltAngleDeg)
    {
        float x;
        float y = 15;


        x = MathHelper.Remap(iMin: 0, iMax: 360, oMin: 2 * cubeWidth, oMax: -2 * cubeWidth, iValue: headingAngleDeg);

        x += xOffset;

        if(x > 2 * cubeWidth) x -= 4* cubeWidth;
        if(x < -2 * cubeWidth) x += 4* cubeWidth;

        if(tiltAngleDeg > 180)
        {
            y = MathHelper.Remap(iMin: 270, iMax: 360, oMin: -cubeWidth, oMax: 0f, iValue: tiltAngleDeg);
        }
        else
        {
            y = MathHelper.Remap(iMin: 0, iMax: 90, oMin: 0, oMax: cubeWidth, iValue: tiltAngleDeg);
        }

        viewMarker.anchoredPosition = new Vector2(x, y);

        input = new Vector2(headingAngleDeg, tiltAngleDeg);
        output = new Vector2(x, y);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
