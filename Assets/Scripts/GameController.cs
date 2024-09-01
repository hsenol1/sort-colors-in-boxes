using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public BottleController FirstBottle;
    public BottleController SecondBottle;
    public BottleController[] bottles;


    // Start is called before the first frame update
    void Start()
    {
        bottles = new BottleController[2];
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
    //         RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

    //         if (hit.collider != null)
    //         {
    //             if (hit.collider.GetComponent<BottleController>() != null)
    //             {
    //                 if (FirstBottle == null)
    //                 {
    //                     FirstBottle = hit.collider.GetComponent<BottleController>();
    //                 }
    //                 else
    //                 {
    //                     if (FirstBottle == hit.collider.GetComponent<BottleController>())
    //                     {
    //                         FirstBottle = null;
    //                     }
    //                     else
    //                     {
    //                         SecondBottle = hit.collider.GetComponent<BottleController>();
    //                         FirstBottle.bottleRef = SecondBottle;

    //                         FirstBottle.UpdateTopColorValues();
    //                         SecondBottle.UpdateTopColorValues();

    //                         if (SecondBottle.FillBottleCheck(FirstBottle.topColor) == true)
    //                         {
    //                             FirstBottle.StartColorTransfer();
    //                             FirstBottle = null;
    //                             SecondBottle = null;
    //                         }
    //                         else
    //                         {
    //                             FirstBottle = null;
    //                             SecondBottle = null;
    //                         }
    //                     }
    //                 }
        
    //             }
    //         }
    //     }
    // }



    public void OnBottleClicked(BottleController clickedBottle)
    {
        if (FirstBottle == null)
            FirstBottle = clickedBottle;
        else if (clickedBottle == FirstBottle)
            FirstBottle = null;
        else
        {
            SecondBottle = clickedBottle;
            FirstBottle.bottleRef = SecondBottle;
            FirstBottle.UpdateTopColorValues();
            SecondBottle.UpdateTopColorValues();

            if (SecondBottle.FillBottleCheck(FirstBottle.topColor))
                FirstBottle.StartColorTransfer();
            
            FirstBottle = null; SecondBottle = null;
        }
    }
}
