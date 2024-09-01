using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    public Color[] bottleColors = new Color[4]; // Placeholder colors for shader
    public SpriteRenderer maskSR;
    public float timeToRotate = 1.0f;
    public AnimationCurve ScaleAndRotationMultiplierCurve;
    public AnimationCurve FillAmountCurve;
    public AnimationCurve RotationSpeedMultiplier;


    private float[] fillAmounts = new float[5] {-0.5f,-0.29f,-0.08f,0.13f,0.34f};
    private float[] rotationValues = new float[4] {54,71,83,90};

    private int rotationIndex = 0;

    [Range(0, 4)]
    public int numberOfColorsInBottle = 4;

    public Color topColor;
    public int numberOfTopColorLayers = 1;

    public BottleController bottleRef;
    public bool justThisBottle = false;
    private int numberOfColorsToTransfer = 0;

    public Transform leftRotationPoint;
    public Transform rightRotationPoint;
    public Transform chosenRotationPoint;
    // private GameController gameController;
    private float directionMultiplier = 1.0f;

    Vector3 originalPosition;
    Vector3 startPosition;
    Vector3 endPosition;

    public LineRenderer lineRenderer;

    void Start()
    {
        maskSR.material.SetFloat("_FillAmount", fillAmounts[numberOfColorsInBottle]);
        originalPosition = transform.position;
        UpdateColorsOnShader();
        UpdateTopColorValues();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && justThisBottle)
        {
            UpdateTopColorValues();
            if (bottleRef.FillBottleCheck(topColor))
            {
                ChoseRotationPointAndDirection();
                numberOfColorsToTransfer = Mathf.Min(numberOfTopColorLayers, 4 - bottleRef.numberOfColorsInBottle);
                for (int i = 0; i < numberOfColorsToTransfer; i++)
                {
                    bottleRef.bottleColors[bottleRef.numberOfColorsInBottle + i] = topColor;
                }
                bottleRef.UpdateColorsOnShader();
            }
            CalculateRotationIndex(4 - bottleRef.numberOfColorsInBottle);
            StartCoroutine(RotateBottle());
        }
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

        float lastAngleValue = 0;

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(0.0f,directionMultiplier * rotationValues[rotationIndex], lerpValue);

            // transform.eulerAngles = new Vector3(0,0,angleValue);
            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
            maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));

            if (fillAmounts[numberOfColorsInBottle] > FillAmountCurve.Evaluate(angleValue) + 0.005f)
            {
                if (lineRenderer.enabled == false)
                {
                    lineRenderer.startColor = topColor;
                    lineRenderer.endColor = topColor;
                    lineRenderer.SetPosition(0, chosenRotationPoint.position);
                    lineRenderer.SetPosition(1, chosenRotationPoint.position - Vector3.up * 1.45f);
                    lineRenderer.enabled = true;
                }
                maskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angleValue));

                bottleRef.FillUp(FillAmountCurve.Evaluate(lastAngleValue) - FillAmountCurve.Evaluate(angleValue));

            }

            t += Time.deltaTime * RotationSpeedMultiplier.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();
        }

        angleValue = directionMultiplier *  rotationValues[rotationIndex];
        // transform.eulerAngles = new Vector3(0,0,angleValue);
        maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
        maskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angleValue));

        numberOfColorsInBottle -= numberOfColorsToTransfer;
        bottleRef.numberOfColorsInBottle += numberOfColorsToTransfer;
        lineRenderer.enabled = false;
        StartCoroutine(RotateBottleBack());
    }

    public IEnumerator RotateBottleBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = directionMultiplier * rotationValues[rotationIndex];

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(directionMultiplier * rotationValues[rotationIndex], 0.0f, lerpValue);
            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
            maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
            lastAngleValue = angleValue;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UpdateTopColorValues();
        angleValue = 0;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
        StartCoroutine(MoveBottleBack());
    }



    public void UpdateTopColorValues()
    {
        if (numberOfColorsInBottle != 0)
        {
            numberOfTopColorLayers = 1;

            topColor = bottleColors[numberOfColorsInBottle - 1];

            if (numberOfColorsInBottle == 4)
            {
                if (bottleColors[3].Equals(bottleColors[2]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[2].Equals(bottleColors[1]))
                    {
                        numberOfTopColorLayers = 3;
                        if (bottleColors[1].Equals(bottleColors[0]))
                            numberOfTopColorLayers = 4;
                    }
                }
            }

            else if (numberOfColorsInBottle == 3)
            {
                if (bottleColors[2].Equals(bottleColors[1]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[1].Equals(bottleColors[0]))
                        numberOfTopColorLayers = 3;
                }
            }

            else if (numberOfColorsInBottle == 2)
            {
                if (bottleColors[1].Equals(bottleColors[0]))
                {
                    numberOfTopColorLayers = 2;
                }
            }

            rotationIndex = 3 - (numberOfColorsInBottle - numberOfTopColorLayers);
        }
    }


    public bool FillBottleCheck(Color colorToCheck)
    {
        if (numberOfColorsInBottle == 0)
            return true;
        else if (numberOfColorsInBottle == 4)
            return false;
        else if (topColor.Equals(colorToCheck))
            return true;
        else 
            return false;
    }

    private void CalculateRotationIndex(int numberOfEmptySpacesinSecondBottle)
    {
        rotationIndex = 3 - (numberOfColorsInBottle - Mathf.Min(numberOfEmptySpacesinSecondBottle, numberOfTopColorLayers));
    }

    private void FillUp(float fillAmountToAdd)
    {
        maskSR.material.SetFloat("_FillAmount", maskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
    }



    public IEnumerator MoveBottle()
    {
        startPosition = transform.position;
        if (chosenRotationPoint = leftRotationPoint)
        {
            endPosition = bottleRef.rightRotationPoint.position;
        }
        else
        {
            endPosition = bottleRef.leftRotationPoint.position;
        }

        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPosition;
        StartCoroutine(RotateBottle());
    }


    public IEnumerator MoveBottleBack()
    {
        startPosition = transform.position;
        endPosition = originalPosition;

        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPosition;
        transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
        maskSR.sortingOrder -= 2;
    }

    private void ChoseRotationPointAndDirection()
    {
        if (transform.position.x > bottleRef.transform.position.x)
        {
            chosenRotationPoint = leftRotationPoint;
            directionMultiplier = -1.0f;
        }
        else
        {
            chosenRotationPoint = rightRotationPoint;
            directionMultiplier = 1.0f;
        }
            
    }

    public void StartColorTransfer()
    {
        ChoseRotationPointAndDirection();

        numberOfColorsToTransfer = Mathf.Min(numberOfTopColorLayers, 4- bottleRef.numberOfColorsInBottle);

        for (int i = 0; i < numberOfColorsToTransfer; i++)
            bottleRef.bottleColors[bottleRef.numberOfColorsInBottle + i] = topColor;
        bottleRef.UpdateColorsOnShader();
        CalculateRotationIndex(4 - bottleRef.numberOfColorsInBottle);

        transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
        maskSR.sortingOrder += 2;

        StartCoroutine(MoveBottle());
    }

}