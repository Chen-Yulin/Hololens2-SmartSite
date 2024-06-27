using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectSpaceManager : MonoBehaviour
{
    int id = 0;
    public GameObject framePrefab;

    
    public Dictionary<string, ObjectFrame> objects = new Dictionary<string, ObjectFrame>();

    public class FrameInfo
    {
        public int id;
        public string cat;
        public Vector3 pos;
        public Vector3 rot;
        public Vector3 size;
        public FrameInfo(int id, string cat, Vector3 pos, Vector3 rot, Vector3 size)
        {
            this.id = id;
            this.cat = cat;
            this.pos = pos;
            this.rot = rot;
            this.size = size;
        }
    }

    public Queue<FrameInfo> frameQueue = new Queue<FrameInfo>();

    public void CreateObject(int id, string cat, Vector3 pos, Vector3 rot, Vector3 size)
    {
        GameObject Frame = Instantiate(framePrefab);
        Frame.transform.parent = transform;
        Frame.SetActive(true);
        Frame.name = cat + id.ToString();
        objects.Add(Frame.name, Frame.GetComponent<ObjectFrame>());
        ObjectFrame frame = objects[Frame.name];
        frame.UpdateFrame(cat, pos, rot, size);
    }

    public void EnqueueUpdateObject(string cat, Vector3 pos, Vector3 rot, Vector3 size)
    {
        frameQueue.Enqueue(new FrameInfo(0, cat, pos, rot, size));
    }

    public void UpdateObject(string cat, Vector3 pos, Vector3 rot, Vector3 size)
    {
        bool pre_found = false;
        foreach (var item in objects)
        {
            ObjectFrame frame = item.Value;

            if (Vector3.Distance(frame.Frame.transform.position, pos) < 0.05f && frame.type == ObjectFrame.Type.Detect)
            {
                pre_found = true;
                item.Value.UpdateFrame(cat, pos, rot, size);
                break;
            }

        }
        if (!pre_found)
        {
            id++;
            CreateObject(id, cat, pos, rot, size);
            
        }
    }

    public void AsyUpdateObject(string cat, float[] transform)
    {
        Vector3 pos = new Vector3(transform[0], transform[1], transform[2]);
        Vector3 rot = new Vector3(transform[3], transform[4], transform[5]);
        Vector3 size = new Vector3(transform[6], transform[7], transform[8]);
        EnqueueUpdateObject(cat, pos, rot, size);
    }


    void Start()
    {
        framePrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        while (frameQueue.Count > 0)
        {
            FrameInfo info = frameQueue.Dequeue();
            UpdateObject(info.cat, info.pos, info.rot, info.size);
        }
    }
}
