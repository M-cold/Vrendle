using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class JsonReader : MonoBehaviour
{

    private string jsonString;

    public VideoPlayer vidPlayer;

    public GameObject buttonPrefab;
    public GameObject buttonContainer;
    public GameObject videoButtonPrefab;
    public GameObject videoButtonContainer;
    public GameObject moduleButtonContainer;
    public GameObject videoScrollBar;
    public GameObject onderwerpScrollBar;
    public GameObject moduleScrollBar;
    public GameObject demoButtonPrefab;

    public GameObject videoMenu;
    public GameObject videoController;
    public GameObject controller;

    public GameObject UIManager;

    public GameObject kamer;
    public GameObject pauseButtonContainer;
    public Slider Scrubslider;

    public Image demoThumb;
    public VideoClip DemoVid;

    //In start load de json file, en start het ophalen van alle videos.
    public void Start()
    {
        jsonString = System.IO.File.ReadAllText(Application.persistentDataPath + "/meta.json");
        JSONObject j = new JSONObject(jsonString);
        LoadAll(j);
        LoadVideoFilter(j);
    }

    //Dit zorgt er voor dat de scrollbar op de juiste positie begind.
    public IEnumerator ScrollFix()
    {
        yield return new WaitForSeconds(0.1f);
        videoScrollBar.GetComponent<Scrollbar>().value = 1.0f;
        onderwerpScrollBar.GetComponent<Scrollbar>().value = 1.0f;
        moduleScrollBar.GetComponent<Scrollbar>().value = 1.0f;
    }

    //Deze functie haalt in stappen alle videos die er in het json bestand staan op en maakt de videoButtons aan en geeft ze de juiste info.
    public void LoadAll(JSONObject json)
    {
        if (json == null)
        {
            GameObject go = Instantiate(demoButtonPrefab);
            go.transform.SetParent(videoButtonContainer.GetComponentInChildren<GridLayoutGroup>().transform, false);
            //go.GetComponentInChildren<Text>().text = "Demo";
            go.GetComponent<Button>().onClick.AddListener(() => OnDemoVideoButtonClick());
            //go.GetComponent<Thumbnail>().ThumbImage = demoThumb;
        }
        JSONObject categories = json.GetField("categories");
        for (int i = 0; i < categories.list.Count; i++)
        {
            JSONObject currentCat = categories[i];

            for (int j = 0; j < currentCat.GetField("groups").list.Count; j++)
            {
                JSONObject subCat = currentCat.GetField("groups")[j];

                for (int k = 0; k < subCat.GetField("children").list.Count; k++)
                {
                    JSONObject module = subCat.GetField("children")[k];
                    for (int m = 0; m < module.GetField("videos").list.Count; m++)
                    {
                        //Hier maak je de video buttons aan en geeft ze een listener zodat er ook wat gebeurd als je er op klikt.
                        JSONObject videos = module.GetField("videos")[m];
                        GameObject go = Instantiate(videoButtonPrefab);
                        go.transform.SetParent(videoButtonContainer.GetComponentInChildren<GridLayoutGroup>().transform, false);
                        go.GetComponentInChildren<Text>().text = videos.GetField("title").ToString().Trim('"');
                        go.GetComponent<Button>().onClick.AddListener(() => OnAllVideoButtonClick(videos));
                        go.GetComponent<Thumbnail>().url = Application.persistentDataPath + videos.GetField("thumb").ToString().Trim('"');
                    }
                }
            }
        }
        StartCoroutine(ScrollFix());
    }

    void OnDemoVideoButtonClick()
    {
        vidPlayer.clip = DemoVid;
        videoMenu.SetActive(false);
        controller.GetComponentInChildren<LineRenderer>().enabled = false;
        UIManager.GetComponentInChildren<UIManager>().videoUI = false;
        kamer.SetActive(false);
        vidPlayer.Play();
        StartCoroutine("CheckStart");
    }

    //Dit is de videoButton info en wat er gebeurd als je er op klikt: de video url ophalen en aan de videoplayer geven en dan starten en menu weg halen.
    public void OnAllVideoButtonClick(JSONObject Jobj)
    {
        if (pauseButtonContainer.GetComponentInChildren<Thumbnail>() != null)
        {
            foreach (RectTransform child in pauseButtonContainer.GetComponent<RectTransform>())
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        //Geeft de url van de video lokatie aan de player, zet het menu en de line renderer uit en start de video.
        vidPlayer.url = Application.persistentDataPath + Jobj.GetField("file").ToString().Trim('"');
        videoMenu.SetActive(false);
        controller.GetComponentInChildren<LineRenderer>().enabled = false;
        UIManager.GetComponentInChildren<UIManager>().videoUI = false;
        kamer.SetActive(false);
        vidPlayer.Play();
        StartCoroutine("CheckStart");
        for (int v = 0; v < Jobj.GetField("related").list.Count; v++)
        {
            JSONObject relatedVideo = Jobj.GetField("related")[v];
            GameObject go = Instantiate(videoButtonPrefab);
            go.transform.SetParent(pauseButtonContainer.GetComponentInChildren<GridLayoutGroup>().transform, false);
            go.GetComponentInChildren<Text>().text = relatedVideo.GetField("title").ToString().Trim('"');
            go.GetComponent<Button>().onClick.AddListener(() => OnRelatedVideoButtonClick(relatedVideo));
            go.GetComponent<Thumbnail>().url = Application.persistentDataPath + relatedVideo.GetField("thumb").ToString().Trim('"');
        }
    }

    public void OnRelatedVideoButtonClick(JSONObject relatedJson)
    {
        vidPlayer.url = Application.persistentDataPath + relatedJson.GetField("file").ToString().Trim('"');
        videoMenu.SetActive(false);
        controller.GetComponentInChildren<LineRenderer>().enabled = false;
        UIManager.GetComponentInChildren<UIManager>().videoUI = false;
        kamer.SetActive(false);
        vidPlayer.Play();
        Scrubslider.value = 0.00001f;
        StartCoroutine("CheckStart");
        UIManager.GetComponent<UIManager>().PauseMenuOff();

        if (pauseButtonContainer.GetComponentInChildren<Thumbnail>() != null)
        {
            foreach (RectTransform child in pauseButtonContainer.GetComponent<RectTransform>())
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    //Deze functie start het checken of de video al klaar is.
    public IEnumerator CheckStart()
    {
        yield return new WaitForSeconds(5f);
        UIManager.GetComponentInChildren<UIManager>().CheckStart();
    }

    //Deze functie haalt de info voor de onderwerpen op, en maakt daar buttons voor aan met de juiste info.
    public void LoadVideoFilter(JSONObject obj)
    {
        //Dit laad alle onderwerp filter knoppen.
        GameObject all = Instantiate(buttonPrefab);
        all.transform.SetParent(buttonContainer.GetComponentInChildren<GridLayoutGroup>().transform, false);
        all.GetComponentInChildren<Text>().text = "Alle video's";
        all.GetComponent<Button>().onClick.AddListener(() => OnAllButtonClick(obj));

        JSONObject root = obj.GetField("categories");
        for (int i = 0; i < root.list.Count; i++)
        {
            JSONObject currentNode = root[i];
            JSONObject groups = currentNode.GetField("groups");
            for (int j = 0; j < groups.list.Count; j++)
            {
                JSONObject theGroup = groups[j];
                JSONObject groupTitles = theGroup.GetField("title");

                GameObject go = Instantiate(buttonPrefab);
                go.transform.SetParent(buttonContainer.GetComponentInChildren<GridLayoutGroup>().transform, false);
                go.GetComponentInChildren<Text>().text = groupTitles.ToString().Trim('"');
                go.GetComponent<Button>().onClick.AddListener(() => OnFilterButtonClick(theGroup));
            }
        }
        StartCoroutine(ScrollFix());
    }

    //Deze functie checkt eerst of er nog videoButons zijn, zo ja haalt ze weg en laad alle videos en maakt er nieuwe buttons voor aan.
    public void OnAllButtonClick(JSONObject allJson)
    {
        //dit is de listener van de "alle video's" knop, haalt oude videoButtons weg en zet nieuwe van alle videos er in en geeft ze de listener.
        if (videoButtonContainer.GetComponentInChildren<Thumbnail>() != null)
        {
            foreach (RectTransform child in videoButtonContainer.GetComponent<RectTransform>())
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        if (moduleButtonContainer.GetComponentInChildren<Button>() != null)
        {
            foreach (RectTransform child in moduleButtonContainer.GetComponent<RectTransform>())
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        LoadAll(allJson);
    }

    //Dit is de FilterButton info en wat er gebeurd als je er op klikt: kijkt in de json file welke filter je aan hebt geklikt en laad de videos die daar onder vallen, 
    //En haalt alle andere videoButtons weg. Ook maakt deze functie de 2e filter buttons aan, de module buttons en geeft deze de juiste info.
    public void OnFilterButtonClick(JSONObject jobj)
    {
        //Checkt eerst of er nog filter knoppen staan en haalt ze weg.
        if (videoButtonContainer.GetComponentInChildren<Thumbnail>() != null)
        {
            foreach (RectTransform child in videoButtonContainer.GetComponent<RectTransform>())
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        if (moduleButtonContainer.GetComponentInChildren<Button>() != null)
        {
            foreach (RectTransform child in moduleButtonContainer.GetComponent<RectTransform>())
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        //Maakt hier de nieuwe video knoppen en module knoppen aan met de juiste filter en geeft ze de juiste listener.
        for (int i = 0; i < jobj.GetField("children").list.Count; i++)
        {
            JSONObject modules = jobj.GetField("children")[i];

            GameObject go2 = Instantiate(buttonPrefab);
            go2.transform.SetParent(moduleButtonContainer.GetComponentInChildren<GridLayoutGroup>().transform, false);
            go2.GetComponentInChildren<Text>().text = modules.GetField("title").ToString().Trim('"');
            go2.GetComponent<Button>().onClick.AddListener(() => OnModuleButtonClick(modules));

            for( int j = 0; j < modules.GetField("videos").list.Count; j++)
            {
                JSONObject vids = modules.GetField("videos")[j];

                GameObject go = Instantiate(videoButtonPrefab);
                go.transform.SetParent(videoButtonContainer.GetComponentInChildren<GridLayoutGroup>().transform, false);
                go.GetComponentInChildren<Text>().text = vids.GetField("title").ToString().Trim('"');
                go.GetComponent<Button>().onClick.AddListener(() => OnAllVideoButtonClick(vids));
                go.GetComponent<Thumbnail>().url = Application.persistentDataPath + vids.GetField("thumb").ToString().Trim('"');
            }
        }
        StartCoroutine(ScrollFix());
    }

    //Deze functie filtert de videoButtons door de module te checken die je aan klikt en de juiste videos te zoekken. En haalt de andere videoButtons weg.
    public void OnModuleButtonClick(JSONObject jsonobj)
    {
        //Haalt eerst de ouder video knoppen weg
        if (videoButtonContainer.GetComponentInChildren<Thumbnail>() != null)
        {
            foreach (RectTransform child in videoButtonContainer.GetComponent<RectTransform>())
            {
                GameObject.Destroy(child.gameObject);
            }
        }


        //zet hier de nieuwe gefilterde video knoppen er in.
        for ( int i = 0; i < jsonobj.GetField("videos").list.Count; i++)
        {
            JSONObject vids = jsonobj.GetField("videos")[i];

            GameObject go = Instantiate(videoButtonPrefab);
            go.transform.SetParent(videoButtonContainer.GetComponentInChildren<GridLayoutGroup>().transform, false);
            go.GetComponentInChildren<Text>().text = vids.GetField("title").ToString().Trim('"');
            go.GetComponent<Button>().onClick.AddListener(() => OnAllVideoButtonClick(vids));
            go.GetComponent<Thumbnail>().url = Application.persistentDataPath + vids.GetField("thumb").ToString().Trim('"');
        }
        StartCoroutine(ScrollFix());
    }

    //Sluit de app
    public void ExitVrendle()
    {
        Application.Quit();
    }

}