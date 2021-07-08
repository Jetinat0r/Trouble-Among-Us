using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBreakersCore : Minigame
{
    public int breakersLeft = 4;
    [SerializeField]
    private DoorBreakersBreaker[] breakers;
    //TODO: HAVE THIS SCRIPT CALL THE DOOR OPEN THING

    private void Start()
    {
        //TODO: HAVE THIS GRAB THE THING RESPONSIBLE FOR ALL DOORS

        int[] selectedBreakers = new int[breakersLeft];
        int numSelected = 0;

        while (numSelected < breakersLeft)
        {
            int random = Random.Range(0, 8);
            bool isFree = true;
            for(int i = 0; i < numSelected; i++)
            {
                if(selectedBreakers[i] == random)
                {
                    isFree = false;
                    break;
                }
            }

            if (isFree)
            {
                selectedBreakers[numSelected] = random;
                numSelected++;
            }
        }

        for (int i = 0; i < selectedBreakers.Length; i++)
        {
            breakers[selectedBreakers[i]].ActivateBreaker();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            CloseMinigame();
        }
    }

    public void ActivateBreaker()
    {
        breakersLeft--;
        if(breakersLeft == 0)
        {
            CompleteMinigame();
        }
    }
}
