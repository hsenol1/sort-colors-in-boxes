using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public List<BottleController> bottles = new List<BottleController>();
    private BottleController FirstBottle;
    private BottleController SecondBottle;

    void Start()
    {
        // // Find all BottleController objects in the scene and add them to the list
        // BottleController[] foundBottles = FindObjectsOfType<BottleController>();
        // bottles.AddRange(foundBottles);

        // // Ensure each bottle has a BoxCollider2D
        // foreach (BottleController bottle in bottles)
        // {
        //     if (bottle.GetComponent<BoxCollider2D>() == null)
        //     {
        //         bottle.gameObject.AddComponent<BoxCollider2D>();
        //     }
        // }
    }

    // public void OnBottleClicked(BottleController clickedBottle)
    // {
    //     if (FirstBottle == null)
    //     {
    //         FirstBottle = clickedBottle;
    //     }
    //     else if (clickedBottle == FirstBottle)
    //     {
    //         // Deselect if clicking the same bottle
    //         FirstBottle = null;
    //     }
    //     else
    //     {
    //         SecondBottle = clickedBottle;
    //         FirstBottle.bottleRef = SecondBottle;

    //         FirstBottle.UpdateTopColorValues();
    //         SecondBottle.UpdateTopColorValues();

    //         if (SecondBottle.FillBottleCheck(FirstBottle.topColor))
    //         {
    //             FirstBottle.StartColorTransfer();
    //         }

    //         // Reset selections after attempting transfer
    //         FirstBottle = null;
    //         SecondBottle = null;
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<BottleController>() != null)
                {
                    if (FirstBottle == null)
                    {
                        FirstBottle = hit.collider.GetComponent<BottleController>();
                    }
                    else
                    {
                        if (FirstBottle == hit.collider.GetComponent<BottleController>())
                        {
                            FirstBottle = null;
                        }
                        else
                        {
                            SecondBottle = hit.collider.GetComponent<BottleController>();
                            FirstBottle.bottleRef = SecondBottle;

                            FirstBottle.UpdateTopColorValues();
                            SecondBottle.UpdateTopColorValues();

                            if (SecondBottle.FillBottleCheck(FirstBottle.topColor) == true)
                            {
                                FirstBottle.StartColorTransfer();
                                FirstBottle = null;
                                SecondBottle = null;
                            }
                            else
                            {
                                FirstBottle = null;
                                SecondBottle = null;
                            }
                        }
                    }
        
                }
            }
        }
    }
}
