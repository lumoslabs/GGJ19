using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTest : MonoBehaviour
{
    public Camera camera;
    private FMODUnity.StudioEventEmitter emitter;
    private float value = 0;

    void Start()
    {
        emitter = camera.GetComponent<FMODUnity.StudioEventEmitter>();
        Debug.Log("emitter: " + emitter);
    }

    void Update()
    {
        value += 0.001f;
        Debug.Log(value);
        emitter.SetParameter("gameEnd", value);
    }
}
