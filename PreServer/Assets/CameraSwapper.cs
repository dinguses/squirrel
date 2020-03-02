using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;
using UnityEngine.UI;

public class CameraSwapper : MonoBehaviour
{
    public GameObject[] cinemachine;
    public GameObject[] other;
    bool isCinemachineOn = true;
    public TransformVariable mainCameraTV;
    public TransformVariable cameraTV;
    //MCT - Main Camera Transform
    public Transform cinemachineMCT;
    public Transform otherMCT;
    //CT - Camera Transform
    public Transform cinemachineCT;
    public Transform otherCT;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAFrame());
    }

    IEnumerator WaitAFrame()
    {
        yield return null;
        SwapCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCinemachineOn = !isCinemachineOn;
            SwapCamera();
        }
    }

    void SwapCamera()
    {
        if (isCinemachineOn)
        {
            for (int i = 0; i < other.Length; ++i)
            {
                other[i].SetActive(false);
            }
            for (int i = 0; i < cinemachine.Length; ++i)
            {
                cinemachine[i].SetActive(true);
            }
			mainCameraTV.value = cinemachineMCT;
            cameraTV.value = cinemachineCT;
            text.text = "Cinemachine";
        }
        else
        {
            for (int i = 0; i < cinemachine.Length; ++i)
            {
                cinemachine[i].SetActive(false);
            }
            for (int i = 0; i < other.Length; ++i)
            {
                other[i].SetActive(true);
            }
            mainCameraTV.value = otherMCT;
            cameraTV.value = otherCT;
            text.text = "Other";
        }
    }
}
