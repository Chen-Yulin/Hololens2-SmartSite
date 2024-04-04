using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HoloCanCapture : MonoBehaviour
{
    Vector2Int requestedCamSize = new Vector2Int(896, 504);

    public RenderTexture targetTexture;

    private WebCamTexture wct;
    async void Start()
    {
        wct = new WebCamTexture(requestedCamSize.x, requestedCamSize.y, 4);
        if (wct)
        {
            wct.Play();
        }
        await GettingTextureFromCam();
    }

    private async Task GettingTextureFromCam()
    {
        await Task.Delay(1000);

        while (wct)
        {
            Graphics.Blit(wct, targetTexture);
            await Task.Delay(32);
        }
        Debug.LogError("wct failed");
    }
}
