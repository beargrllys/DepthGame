using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Windows.Kinect;

public class ColorSourceView : MonoBehaviour
{
    public GameObject ColorSourceManager;
    private ColorSourceManager _ColorManager;
    private RawImage rawImage;
    public Texture2D picTexture;

    void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
        rawImage.material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }
    
    void Update()
    {
        if (ColorSourceManager == null)
        {
            return;
        }
        
        _ColorManager = ColorSourceManager.GetComponent<ColorSourceManager>();
        if (_ColorManager == null)
        {
            return;
        }
        picTexture = _ColorManager.GetColorTexture();
        rawImage.texture = picTexture;
    }
}
