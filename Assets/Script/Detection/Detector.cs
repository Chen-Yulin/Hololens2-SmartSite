using Assets.Scripts;
using Assets.Scripts.TextureProviders;
using NN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Unity.Barracuda;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class Detector : MonoBehaviour
{
    [Tooltip("File of YOLO model.")]
    [SerializeField]
    protected NNModel ModelFile;

    [Tooltip("RawImage component which will be used to draw resuls.")]
    [SerializeField]
    protected Material material_t;

    [Range(0.0f, 1f)]
    [Tooltip("The minimum value of box confidence below which boxes won't be drawn.")]
    [SerializeField]
    protected float MinBoxConfidence = 0.3f;

    [SerializeField]
    protected TextureProviderType.ProviderType textureProviderType;

    [SerializeReference]
    protected TextureProvider textureProvider = null;

    [Tooltip("deploy on hololens")]
    [SerializeField]
    protected bool holo = false;

    protected NNHandler nn;
    protected Color[] colorArray = new Color[] { Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow };

    YOLOv8 yolo;


    private bool detectionOn = false;

    Texture2D CapturedTexture;
    Texture2D DisplayTexture;
    List<ResultBox> boxes = new List<ResultBox>();

    public void ToggleDetection()
    {
        detectionOn = !detectionOn;
        if (detectionOn)
        {
            yolo.TurnOnYolo();
        }
        else
        {
            yolo.TurnOffYolo();
        }
    }

    private void OnEnable()
    {
        nn = new NNHandler(ModelFile);
        //yolo = new YOLOv8Segmentation(nn);
        yolo = gameObject.AddComponent<YOLOv8>();
        yolo.InitYOLOv8(nn);

        textureProvider = GetTextureProvider(nn.model);
        textureProvider.Start();
    }

    private void Update()
    {
        if (detectionOn)
        {
            if (yolo.ResAvailable) // all data has been updated
            {
                Debug.Log("update result "+yolo.Results.Count);
                yolo.ResAvailable = false;
                boxes = yolo.Results;

                DisplayTexture = new Texture2D(CapturedTexture.width, CapturedTexture.height, TextureFormat.RGB24, false);
                Graphics.CopyTexture(CapturedTexture, DisplayTexture);
                DrawResults(boxes, DisplayTexture);
                material_t.mainTexture = DisplayTexture;

                YOLOv8OutputReader.DiscardThreshold = MinBoxConfidence;
                CapturedTexture = GetNextTexture();
                yolo.SetSource(CapturedTexture);
            }
            else if (yolo.StartOfDetection)
            {
                yolo.StartOfDetection = false;
                YOLOv8OutputReader.DiscardThreshold = MinBoxConfidence;
                CapturedTexture = GetNextTexture();
                yolo.SetSource(CapturedTexture);
            }
        }
    }

    protected TextureProvider GetTextureProvider(Model model)
    {
        var firstInput = model.inputs[0];
        int height = firstInput.shape[5];
        int width = firstInput.shape[6];

        TextureProvider provider;
        switch (textureProviderType)
        {
            case TextureProviderType.ProviderType.WebCam:
                if (holo)
                {
                    provider = new WebCamTextureProvider(textureProvider as WebCamTextureProvider, width, height, true);
                }
                else
                {
                    provider = new WebCamTextureProvider(textureProvider as WebCamTextureProvider, width, height, false);
                }
                break;

            case TextureProviderType.ProviderType.Video:
                provider = new VideoTextureProvider(textureProvider as VideoTextureProvider, width, height);
                break;
            default:
                throw new InvalidEnumArgumentException();
        }
        return provider;
    }

    protected Texture2D GetNextTexture()
    {
        return textureProvider.GetTexture();
    }

    void OnDisable()
    {
        nn.Dispose();
        textureProvider.Stop();
    }

    protected void DrawResults(IEnumerable<ResultBox> results, Texture2D img)
    {
        results.ForEach(box => DrawBox(box, img));
    }

    protected virtual void DrawBox(ResultBox box, Texture2D img)
    {
        Color boxColor = colorArray[box.bestClassIndex % colorArray.Length];
        int boxWidth = (int)(box.score / MinBoxConfidence);
        TextureTools.DrawRectOutline(img, box.rect, boxColor, boxWidth, rectIsNormalized: false, revertY: true);
    }

    private void OnValidate()
    {
        Type t = TextureProviderType.GetProviderType(textureProviderType);
        if (textureProvider == null || t != textureProvider.GetType())
        {
            if (nn == null)
                textureProvider = RuntimeHelpers.GetUninitializedObject(t) as TextureProvider;
            else
            {
                textureProvider = GetTextureProvider(nn.model);
                textureProvider.Start();
            }

        }
    }
}
