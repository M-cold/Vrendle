using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class videoScrubBar : MonoBehaviour {
    
    public VideoPlayer VP;
    public bool slide;
    public Slider ProgressSlider;
    public GameObject loadingImage;


	// Use this for initialization
	void Start () {
		
	}

	void Update () {
		if(!slide && VP.frameCount > 0)
        {
            ProgressSlider.value = (float)VP.frame / (float)VP.frameCount;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || Input.GetMouseButtonDown(0))
        {
            slide = true;
        }
        else
        {
            slide = false;
        }

        if(ProgressSlider.value != VP.frameCount)
        {
            loadingImage.SetActive(true);
        }
        else
        {
            loadingImage.SetActive(false);
        }
    }

    public void changeValue(Slider slider)
    {
        if (!slide && !VP.isPlaying)
        {
            float frame = (float)slider.value * (float)VP.frameCount;
            VP.frame = (long)frame;
            Debug.Log("set slider");
        }
    }
}
