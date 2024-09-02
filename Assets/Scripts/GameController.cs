using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public List<BottleController> bottles = new List<BottleController>();
    private BottleController FirstBottle;
    private BottleController SecondBottle;
    // private Level level;
    private Vector3 startPosition = new Vector3(0,0,0);
    private float spacing = 1f;

    public BottleController bottlePrefab;
    private int numberOfBottles;
    private List<BottleController> gameBottles;


    void Start()
    {
        numberOfBottles = 4;
        InstantiateBottles();
        gameBottles = new List<BottleController>();
    }


    private void InstantiateBottles()
    {
        for (int i = 0; i < numberOfBottles; i++)
        {
            Vector3 position = startPosition + i * new Vector3(spacing,0,0);
            BottleController newBottle = Instantiate(bottlePrefab,position, Quaternion.identity);
            newBottle.transform.SetParent(this.transform);
            bottles.Add(newBottle);
        }
    }
    public void HandleBottleClicked(BottleController clickedBottle)
    {
        if (FirstBottle == null)
        {
            FirstBottle = clickedBottle;
        }
        else if (clickedBottle == FirstBottle)
        {
            // Deselect if clicking the same bottle
            FirstBottle = null;
        }
        else
        {
            SecondBottle = clickedBottle;
            FirstBottle.bottleRef = SecondBottle;

            FirstBottle.UpdateTopColorValues();
            SecondBottle.UpdateTopColorValues();

            if (SecondBottle.FillBottleCheck(FirstBottle.topColor))
            {
                FirstBottle.StartColorTransfer();
            }

            // Reset selections after attempting transfer
            FirstBottle = null;
            SecondBottle = null;
        }
    }




    private bool CheckIfLevelIsCompleted()
    {
        foreach (BottleController bottle in gameBottles)
        {
            if (!bottle.CheckIfBottleIsCompleted())
                return false;
        }
        return true;
    }
}
