using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using System.Threading.Tasks;

namespace Assets.Scripts.TextureProviders
{
    [Serializable]
    public class WebCamTextureProvider : TextureProvider
    {
        [Tooltip("Leave empty for automatic selection.")]
        [SerializeField]
        private string cameraName;
        private WebCamTexture webCamTexture;

        public WebCamTextureProvider(int width, int height, bool holo = false, TextureFormat format = TextureFormat.RGB24, string cameraName = null) : base(width, height, format)
        {
            if (holo)
            {
                webCamTexture = new WebCamTexture(896, 504, 4);
            }
            else
            {
                cameraName = SelectCameraDevice();
                webCamTexture = new WebCamTexture(cameraName);
            }
            InputTexture = webCamTexture;
        }

        public WebCamTextureProvider(WebCamTextureProvider provider,int width, int height, bool holo = false, TextureFormat format = TextureFormat.RGB24) : this(width, height, holo, format, provider?.cameraName)
        {
        }

        public async override void Start()
        {
            webCamTexture.Play();
            await GettingTextureFromCam();
        }

        private async Task GettingTextureFromCam()
        {
            while (webCamTexture)
            {
                webCamTexture.Play();
                await Task.Delay(32);
                if (webCamTexture)
                {
                    webCamTexture.Pause();
                    await Task.Delay(32);
                }

            }

        }

        public override void Stop()
        {
            webCamTexture.Stop();
        }

        public override TextureProviderType.ProviderType TypeEnum()
        {
            return TextureProviderType.ProviderType.WebCam;
        }

        /// <summary>
        /// Return first backfaced camera name if avaible, otherwise first possible
        /// </summary>
        private string SelectCameraDevice()
        {
            if (WebCamTexture.devices.Length == 0)
                throw new Exception("Any camera isn't avaible!");
            Debug.Log("use " + WebCamTexture.devices[1].name);
            return WebCamTexture.devices[1].name;
        }

    }
}