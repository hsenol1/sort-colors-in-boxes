using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    // Start is called before the first frame update
    public Color[] bottleColors;
    public SpriteRenderer maskSR;
    public float timeToRotate = 1.0f;
    public AnimationCurve ScaleAndRotationMultiplierCurve;
    public AnimationCurve FillAmountCurve;
    public AnimationCurve RotationSpeedMultiplier;

    public float[] fillAmounts;


    void Start()
    {
        UpdateColorsOnShader();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            StartCoroutine(RotateBottle());
    }



    void UpdateColorsOnShader()
    {  
        maskSR.material.SetColor("_C1", bottleColors[0]);
        maskSR.material.SetColor("_C2", bottleColors[1]);
        maskSR.material.SetColor("_C3", bottleColors[2]);
        maskSR.material.SetColor("_C4", bottleColors[3]);
    }


    public IEnumerator RotateBottle()
    {
        float t = 0;
        float lerpValue;
        float angleValue; 


        while (t < timeToRotate)
        {
            lerpValue= t / timeToRotate;
            angleValue = Mathf.Lerp(0.0f, 90.0f, lerpValue);
            transform.eulerAngles = new Vector3(0,0,angleValue);

            maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
            maskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angleValue));
            t += Time.deltaTime * RotationSpeedMultiplier.Evaluate(angleValue);

            yield return new WaitForEndOfFrame();
        }

        angleValue  = 90.0f;
        transform.eulerAngles = new Vector3(0,0,angleValue);
        maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
        maskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angleValue));

        StartCoroutine(RotateBottleBack());

    }


    public IEnumerator RotateBottleBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue; 


        while (t < timeToRotate)
        {
            lerpValue= t / timeToRotate;
            angleValue = Mathf.Lerp(90.0f, 0.0f, lerpValue);
            transform.eulerAngles = new Vector3(0,0,angleValue);

            maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        angleValue  = 90.0f;
        transform.eulerAngles = new Vector3(0,0,angleValue);
        maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
        
    }
}
