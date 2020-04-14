using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public GameObject grid;
    public Button newGrid;
    public Button run;

    private int choice;
    // Start is called before the first frame update
    void Start()
    {
        choice = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (grid.GetComponent<Grid>().mapSet && grid.GetComponent<Grid>().run) { 
            newGrid.GetComponent<Button>().interactable = false;
            run.GetComponent<Button>().interactable = false;
        }
        else { 
            newGrid.GetComponent<Button>().interactable = true;
            run.GetComponent<Button>().interactable = true;
        }
    }

    public void NewGrid()
    {
        if (grid.GetComponent<Grid>().mapSet) {
            Debug.Log("not supposed to be clicked");
            grid.GetComponent<Grid>().mapSet = false;
            grid.GetComponent<Grid>().run = false;
            grid.GetComponent<Grid>().Clear();
        }
        choice++;
        if (choice > 3)
            choice = 1;
        grid.GetComponent<Grid>().CreateGrid(choice.ToString());
    }

    public void StartSimulation()
    {
        if (grid.GetComponent<Grid>().mapSet && grid.GetComponent<Grid>().converged && !grid.GetComponent<Grid>().run)
            grid.GetComponent<Grid>().Rerun();
        else
            grid.GetComponent<Grid>().run = true;

    }
}
