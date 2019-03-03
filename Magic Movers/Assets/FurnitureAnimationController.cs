using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureAnimationController : MonoBehaviour
{

    public Color combinedColor;
    public Color blueColor;
    public Color redColor;
    private Animator am;
    public SpriteRenderer outline;
    public SpriteRenderer blorb;
    public BoxCollider2D trashCollider;

    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
        outline.color = combinedColor;
        blorb.color = combinedColor;
    }

    public void SetColor(int playerID)
    {
        if (playerID == 0)
        {
            outline.color = blueColor;
            blorb.color = blueColor;

        }
        else if (playerID == 1)
        {

            outline.color = redColor;
            blorb.color = redColor;
        }
        else
        {
            outline.color = combinedColor;
            blorb.color = combinedColor;
        }
    }


    public void PlayBreak()
    {
        am.SetTrigger("furn_break");
        GetComponent<PolygonCollider2D>().isTrigger = true;
        GetComponent<PolygonCollider2D>().enabled = false;
        trashCollider.enabled = true;
        trashCollider.tag = "Grass";
        gameObject.tag = "Grass"; 
    }

    public void PlayPlace()
    {
        am.SetTrigger("furn_place");
    }

    public void PlayMove()
    {

    }
}
