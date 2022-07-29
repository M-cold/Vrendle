using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thumbnail : MonoBehaviour {

    public string url;
    public RawImage ThumbImage;
    public Texture Thumb;

    //Loads the thumbnail and gives it to the button
    IEnumerator Start()
    {
        WWW www = new WWW("file:///" + url);
        while (!www.isDone)
            yield return null;
        Thumb = www.texture;
        LoadThumbnail();
    }

    public void LoadThumbnail()
    {
        ThumbImage.texture = Thumb;
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }
}
