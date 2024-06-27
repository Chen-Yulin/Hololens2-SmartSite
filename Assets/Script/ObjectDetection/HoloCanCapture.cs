using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HoloCanCapture : MonoBehaviour
{
    Vector2Int requestedCamSize = new Vector2Int(896, 504);

    public RenderTexture targetTexture;
    public bool on_holo = true;
    public WebCamTexture wct;
    async void Start()
    {
        if (on_holo)
        {
            wct = new WebCamTexture(requestedCamSize.x, requestedCamSize.y, 4);
        }
        else
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            foreach (var item in devices)
            {
                Debug.Log(item.name);
            }
            wct = new WebCamTexture(devices[1].name);
        }
        
        if (wct)
        {
            wct.Play();
            await GettingTextureFromCam();
        }
        else
        {
            Debug.LogError("wct failed");
        }
    }

    private async Task GettingTextureFromCam()
    {
        while (wct)
        {
            if (wct && !on_holo)
            {
                wct.Play();
            }
            Graphics.Blit(wct, targetTexture);
            await Task.Delay(32);
            if (wct && !on_holo)
            {
                wct.Pause();
                await Task.Delay(32);
            }
            
        }
        
    }
}
