using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManager : MonoBehaviour {

    public GameObject controller;
    public GameObject videoController;
    public VideoPlayer VP;
    public GameObject HWPMenu;

    public bool videoUI;
    public bool pauseMenu;
    public GameObject kamer;

    public Button loopButton;


    void Start ()
    {
        videoUI = true;
        pauseMenu = false;
    }
	
    //checkt of je de touchpad indrukt, en zet dan de video op pauze en haalt het pauze menu er voor.
	void Update ()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && !videoUI && VP.isPlaying || (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)) && !videoUI && VP.isPlaying)
        {
            PauseMenuOn();
            VP.Pause();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !videoUI && VP.isPlaying)
        {
            PauseMenuOn();
            VP.Pause();
        }

    }

    //void LateUpdate()
    //{
    //    if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && !VP.isPlaying && !videoUI)
    //    {
    //        VP.Play();
    //        PauseMenuOff();
    //    }
    //}

    public void Loop()
    {
        if (VP.isLooping)
        {
            VP.isLooping = false;
            loopButton.GetComponentInChildren<Text>().text = "Loop off";
            
        }else if (!VP.isLooping)
        {
            VP.isLooping = true;
            loopButton.GetComponentInChildren<Text>().text = "Loop on";
        }
    }
    
    
    //Deze functie start het checken of de video klaar is.
    public void CheckStart()
    {
        InvokeRepeating("checkOver", .5f, .1f);
    }

    //Deze functie stop het checken of de video klaar is.
    public void stopCheck()
    {
        CancelInvoke("checkOver");   
    }

    //Deze functie checkt of de video klaar is, als dat zo is haalt die het menu naar boven.
    private void checkOver()
    {
        if (VP.isPlaying)
        {
            long playerCurrentFrame = VP.GetComponent<VideoPlayer>().frame;
            
            long playerFrameCount = System.Convert.ToInt64(VP.GetComponent<VideoPlayer>().frameCount);
            
            if (playerCurrentFrame < playerFrameCount)
            {

            }
            else
            {
                DonePlaying();
                CancelInvoke("checkOver");
            }
        }
    }

    //Deze functie haalt het menu naar boven als de video klaar is.
    public void DonePlaying()
    {
        PauseMenuOn();
        //HWPMenu.SetActive(true);
        kamer.SetActive(true);
        //controller.GetComponent<LineRenderer>().enabled = true;
    }

    public void VideoUIOn()
    {
        videoUI = true;
    }

    //Als de video op pauze staat zet deze functie de line renderer aan en haalt het pauze menu naar boven.
    public void PauseMenuOn()
    {
        controller.GetComponent<LineRenderer>().enabled = true;
        videoController.SetActive(true);
        pauseMenu = true;
    }

    //Als de video weer op play word gezet zet deze functie de line renderer uit en haalt het pauze menu weg.
    public void PauseMenuOff()
    {
        controller.GetComponent<LineRenderer>().enabled = false;
        videoController.SetActive(false);
        pauseMenu = false;
    }
}