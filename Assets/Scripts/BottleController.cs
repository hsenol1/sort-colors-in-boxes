using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    // Start is called before the first frame update
    public Stack<Color> bottleColors = new Stack<Color>();
    public SpriteRenderer maskSR;
    public float timeToRotate = 1.0f;
    public AnimationCurve ScaleAndRotationMultiplierCurve;
    public AnimationCurve FillAmountCurve;
    public AnimationCurve RotationSpeedMultiplier;

    public float[] fillAmounts;
    public float[] rotationValues;
    private int rotationIndex = 0;
    [Range(0,4)]
    public int numberOfColorsInBottle = 4;

    public Color topColor;
    public int numberOfTopColorLayers = 1;

    public BottleController bottleRef;
    public bool justThisBottle = false;
    private int numberOfColorsToTransfer = 0;

    public Transform leftRotationPoint;
    public Transform rightRotationPoint;
    public Transform chosenRotationPoint;

    private float directionMultiplier = 1.0f;
    private GameController gameController;
    void Start()
    {
        gameController = GameController.Instance;
        maskSR.material.SetFloat("_FillAmount",fillAmounts[numberOfColorsInBottle]);
        
        UpdateColorsOnShader();
        UpdateTopColorValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && justThisBottle == true)
        {
            UpdateTopColorValues();
            if (bottleRef.FillBottleCheck(topColor))
            
            {
                PickRotationAndDirection();
                numberOfColorsToTransfer = Mathf.Min(numberOfTopColorLayers, 4 - bottleRef.numberOfColorsInBottle);
                for (int i = 0; i < numberOfColorsToTransfer; i++)
                {
                    bottleRef.bottleColors.Push(topColor);

                }
                bottleRef.UpdateColorsOnShader();
            }

            CalculateRotationIndex(4 - bottleRef.numberOfColorsInBottle);
            StartCoroutine(RotateBottle());
        }
    }



    void OnMouseDown()
    {   
        gameController.OnBottleClicked(this);
    }


    public void StartColorTransfer()
    {
        PickRotationAndDirection();
        numberOfColorsToTransfer = Mathf.Min(numberOfTopColorLayers, 4 - bottleRef.numberOfColorsInBottle);
        for (int i = 0; i < numberOfColorsToTransfer; i++)
        {
            bottleRef.bottleColors.Push(topColor);

        }
        bottleRef.UpdateColorsOnShader();
        
        CalculateRotationIndex(4 - bottleRef.numberOfColorsInBottle);
        StartCoroutine(RotateBottle());

    
    }



    void UpdateColorsOnShader()
    {  
        Color[] colorsArray = bottleColors.ToArray();
        maskSR.material.SetColor("_C1", colorsArray.Length > 0 ? colorsArray[0] : Color.clear);
        maskSR.material.SetColor("_C2", colorsArray.Length > 1 ? colorsArray[1] : Color.clear);
        maskSR.material.SetColor("_C3", colorsArray.Length > 2 ? colorsArray[2] : Color.clear);
        maskSR.material.SetColor("_C4", colorsArray.Length > 3 ? colorsArray[3] : Color.clear);
    }


    public IEnumerator RotateBottle()
    {
        float t = 0;
        float lerpValue;
        float angleValue; 


        float lastAngleValue = 0;

        while (t < timeToRotate)
        {
            lerpValue= t / timeToRotate;
            angleValue = Mathf.Lerp(0.0f,directionMultiplier * rotationValues[rotationIndex], lerpValue);
            // transform.eulerAngles = new Vector3(0,0,angleValue);
            
            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);

            maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));

            if (fillAmounts[numberOfColorsInBottle] > FillAmountCurve.Evaluate(angleValue))
            {
                maskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angleValue));
                bottleRef.FillUp(FillAmountCurve.Evaluate(lastAngleValue) - FillAmountCurve.Evaluate(angleValue));
            }
                

            t += Time.deltaTime * RotationSpeedMultiplier.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();
        }

        angleValue  = directionMultiplier * rotationValues[rotationIndex];
        //        transform.eulerAngles = new Vector3(0,0,angleValue);
        maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
        maskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angleValue));


        numberOfColorsInBottle -= numberOfColorsToTransfer;
        bottleRef.numberOfColorsInBottle += numberOfColorsToTransfer;

        StartCoroutine(RotateBottleBack());

    }


    public IEnumerator RotateBottleBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue; 

        float lastAngleValue= directionMultiplier * rotationValues[rotationIndex];
        while (t < timeToRotate)
        {
            lerpValue= t / timeToRotate;
            angleValue = Mathf.Lerp(directionMultiplier * rotationValues[rotationIndex], 0.0f, lerpValue);

            // transform.eulerAngles = new Vector3(0,0,angleValue);
            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
            maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
            lastAngleValue = angleValue;
            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        UpdateTopColorValues();
        angleValue  =  0;
        transform.eulerAngles = new Vector3(0,0,angleValue);
        maskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
        
    }




    public void UpdateTopColorValues()
    {
        if (bottleColors.Count != 0)
        {
            numberOfTopColorLayers = 1;
            topColor = bottleColors.Peek();
            Stack<Color> tempStack = new Stack<Color>(bottleColors);
            tempStack.Pop();
            while (tempStack.Count > 0 && tempStack.Peek().Equals(topColor))
            {
                numberOfTopColorLayers++;
                tempStack.Pop();
            }

            rotationIndex = 3 - (bottleColors.Count - numberOfTopColorLayers);
        }

    }




    public bool FillBottleCheck(Color colorToCheck)
    {
        if (bottleColors.Count == 0)
            return true;

        if (bottleColors.Count == 4)
            return false;

        return topColor.Equals(colorToCheck);
    }
    


    private void CalculateRotationIndex(int numberOfEmptySpacesinSecondBottle)
    {
        rotationIndex = 3- (bottleColors.Count-Mathf.Min(numberOfEmptySpacesinSecondBottle, numberOfTopColorLayers));    
    }


    private void FillUp(float fillAmountToAdd)
    {
        maskSR.material.SetFloat("_FillAmount", maskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
    }


    private void PickRotationAndDirection()
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
}
