using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscountRate : MonoBehaviour
{
    public Grid grid;
    public Button up;
    public Button down;
    float discount;

    // Start is called before the first frame update
    void Start()
    {
        discount = .9f;
        Debug.Log(discount);
        gameObject.GetComponent<Text>().text = discount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (grid.run || !grid.mapSet)
        {
            up.GetComponent<Button>().interactable = false;
            down.GetComponent<Button>().interactable = false;
        }
        else 
        {
            up.GetComponent<Button>().interactable = true;
            down.GetComponent<Button>().interactable = true;
        }
    }

    public void upClicked()
    {
        float click = float.Parse(gameObject.GetComponent<Text>().text);
        if (click < 0.9f) { 
            gameObject.GetComponent<Text>().text = (click + .1f).ToString();
            grid.discountRate += .1f;
        }
    }

    public void downClicked()
    {
        float click = float.Parse(gameObject.GetComponent<Text>().text);
        if (click > 0.1f)
        {
            gameObject.GetComponent<Text>().text = (click - .1f).ToString();
            grid.discountRate -= .1f;
        }
    }
}

