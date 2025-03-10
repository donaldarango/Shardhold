
using System.Collections;
using UnityEngine;
using System;
using static CustomDebug;

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

    public static int RandomInt(int min, int max)
    {
        if(min>max && CustomMathDebugging(DebuggingType.Warnings))
        {
            Debug.LogError("WARNING: RandomInt(" + min + ", " + max + ") called with min > max. Expect anomalous output.");
        }
        int result = max + 1;

        int safety = 200;
        while (safety-- > 0 && result == max + 1) //there is a very small chance of getting max+1, so if that happens just try again
        {
            float rand = UnityEngine.Random.Range(min, max + 1);  //plus one is needed to make the max value as likely as the rest
            result = (int)rand;
        }
        if (result > max)    //in the extremely unlikely event that we got max+1 200 times, just return the max
        {
            if (CustomDebug.CustomMathDebugging(DebuggingType.ErrorOnly))
            {
                if (result == max + 1)
                {
                    Debug.Log("Somehow, RandomInt(" + min + ", " + max + ") gave the random value of " + result + ". Values exactly 1 more than the max are possible, but EXTREMELY unlikely. You may want to check for anomalous input. " + max + " will be returned.");
                }
                else
                {
                    Debug.LogError("Somehow, RandomInt(" + min + ", " + max + ") gave the random value of " + result + ". This should not be possible. " + max + " will be returned.");
                }
            }
            result = max;
        }
        if (result < min)
        {
            if (CustomDebug.CustomMathDebugging(DebuggingType.Warnings))
            {
                Debug.LogError("Somehow, RandomInt(" + min + ", " + max + ") gave the random value of " + result + ". This should not be possible. " + min + " will be returned.");
            }
            result = min;
        }
        if (min > max && CustomDebug.CustomMathDebugging(DebuggingType.ErrorOnly))
        {
            Debug.Log("Output from min > max: " + result);
        }
        return result;
    }
}