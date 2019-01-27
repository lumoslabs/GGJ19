using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    public float endY;
    private float endActual = 0;
    private Vector3 start, end;

    public float speed;
    private float startTime;
    private float distance;

    public GameObject titleScreen;
    public GameObject rulesScreen;
    private RectTransform rectTransform;

    private bool playPressed = false;
    private int pageNum = 0;

    public void Start()
    {
        titleScreen.SetActive(true);
        rulesScreen.SetActive(false);
        //rectTransform = layout.GetComponent<RectTransform>();
    }

    public void Play()
    {
        if (pageNum == 0)
        {
            //titleScreen.SetActive(false);
            rulesScreen.SetActive(true);
        }
        if (pageNum == 1)
        {
            titleScreen.SetActive(false);
            rulesScreen.SetActive(false);
        }   
        pageNum++;

    }

    private void Update()
    {
        if (playPressed)
        {

            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / distance;
            rectTransform.position = Vector3.Lerp(start, end, fracJourney);
            if (fracJourney >= 1) {
                playPressed = false;
                pageNum++;
            }
        }
    }

    public bool TitleFinished()
    {
        if (pageNum >=2 )
        {
            return true;
        }
        return false;
    }
}
