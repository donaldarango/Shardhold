
using System.Collections;
using UnityEngine;
using System;

public class CustomMath : MonoBehaviour
{
    public static Vector2 PolarToCartesian(float r, float theta){
        //TODO check that this is in the correct degree/radian mode
        float x = r*Mathf.Cos(theta);
        float y = r*Mathf.Sin(theta);
        return new Vector2(x, y);
    }

    public static Vector2 CartesianToPolar(float x, float y){
        //TODO check that tangent is in the correct degree/radian mode
        float r = Mathf.Sqrt(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f));
        float theta = Mathf.Atan2(y, x);
        return new Vector2(r, theta);
    }
}