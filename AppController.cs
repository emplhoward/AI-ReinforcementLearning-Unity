using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    float timeScale = 1;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
    }

    public void StartTime()
    {
        Time.timeScale = timeScale;
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void SetTimeX2()
    {
        Time.timeScale = 2;
    }

    public void SetTimeX4()
    {
        Time.timeScale = 4;
    }

    public void SetTimeX8()
    {
        Time.timeScale = 8;
    }

    public void SetTimeX16()
    {
        Time.timeScale = 16;
    }

    public void SetTimeX32()
    {
        Time.timeScale = 32;
    }
}
