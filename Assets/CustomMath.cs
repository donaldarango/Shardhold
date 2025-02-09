
using System.Collections;
using UnityEngine;
using System;

public class CustomMath : MonoBehaviour
{
    public static Vector2 PolarToCartesian(float r, float theta){
        //TODO check that this is in the correct degree/radian mode
        float x = r*Mathf.cos(theta);
        float y = r*Mathf.sin(theta);
        return new Vector2(x, y);
    }

    public static Vector2 CartesianToPolar(float x, float y){
        //TODO check that tangent is in the correct degree/radian mode
        float r = Mathf.sqr(Mathf.pow(x, 2f) + Mathf.pow(y, 2f));
        float theta = Mathf.inversetan(y/x);
        //TODO there may be issues if x = 0
    }
}