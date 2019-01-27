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

    public GameObject layout;
    private RectTransform rectTransform;

    private bool playPressed = false;
    private int pageNum = 0;

    public void Start()
    {
        rectTransform = layout.GetComponent<RectTransform>();
    }

    public void Play()
    {
        if (!playPressed)
        {
            start = new Vector3(rectTransform.position.x, rectTransform.position.y, 0);
            endActual += endY;
            end = new Vector3(rectTransform.position.x, endActual, 0);

            startTime = Time.time;
            distance = Vector3.Distance(start, end);
            playPressed = true;
        }
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
