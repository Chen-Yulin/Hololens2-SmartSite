using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Profiling;

namespace NN
{
    public class YOLOv8 : MonoBehaviour
    {
        protected YOLOv8OutputReader outputReader;

        protected NNHandler nn;

        public bool workerbusy = false;

        public bool jobcomplete = false;

        public bool ResAvailable = false;

        public bool StartOfDetection = false;

        private List<ResultBox> results = new List<ResultBox>();

        public List<ResultBox> Results
        {
            get { return results; }
            set
            {
                if (value != null)
                {
                    results = value;
                }
            }
        }


        [SerializeField]
        public List<string> className  = new List<string>();

        Tensor input;

        Texture2D sourceImg;

        bool on;

        public void SetSource(Texture2D img)
        {
            sourceImg = img;
        }

        public void TurnOffYolo()
        {
            on = false;
            workerbusy = false;
            jobcomplete = false;
            ResAvailable = false;
            StartOfDetection = false;

            StopAllCoroutines();
            try
            {
                input.tensorOnDevice.Dispose();
            }
            catch { }
        }

        public void TurnOnYolo()
        {
            on = true;
            StartOfDetection = true;
        }

        public YOLOv8(NNHandler nn)
        {
            this.nn = nn;
            outputReader = new();
        }
        public virtual void InitYOLOv8(NNHandler nn)
        {
            this.nn = nn;
            outputReader = new();
            className = readClassNames("StreamingAssets/coco.names");
        }

        public void Run()
        {
            Results = Postprocess(ExecuteModel());
        }
        public void Update()
        {
            if (on)
            {
                Run();
            }
        }

        protected Tensor[] ExecuteModel()
        {
            if (!workerbusy) // start of execution
            {
                if (jobcomplete) // complete
                {
                    jobcomplete = false;
                    ResAvailable = true;
                    input.tensorOnDevice.Dispose();
                    return PeekOutputs().ToArray();
                }
                else // ready to start
                {
                    if (sourceImg)// start of job
                    {
                        input = new Tensor(sourceImg);
                        // ExecuteBlocking(input);
                        workerbusy = true;
                        StartCoroutine(ExecutePartialy(input));
                    }
                    return null;
                }
            }
            else //during execution
            {
                return null;
            }
        }

        IEnumerator ExecutePartialy(Tensor preprocessed)
        {
            var it = nn.worker.StartManualSchedule(preprocessed);
            workerbusy = true;
            int cnt = 0;

            while (it.MoveNext())
            {
                ++cnt;
                if (cnt % 5 == 0)
                {
                    nn.worker.FlushSchedule(false);
                    yield return null;
                }
            }

            nn.worker.FlushSchedule(true);
            workerbusy = false;
            jobcomplete = true;
        }

        private IEnumerable<Tensor> PeekOutputs()
        {
            foreach (string outputName in nn.model.outputs)
            {
                Tensor output = nn.worker.PeekOutput(outputName);
                yield return output;
            }
        }

        protected List<ResultBox> Postprocess(Tensor[] outputs)
        {
            if (outputs == null)
            {
                return null;
            }
            Profiler.BeginSample("YOLOv8Postprocessor.Postprocess");
            Tensor boxesOutput = outputs[0];
            List<ResultBox> boxes = outputReader.ReadOutput(boxesOutput).ToList();
            boxes = DuplicatesSupressor.RemoveDuplicats(boxes);
            Profiler.EndSample();
            return boxes;
        }

        protected virtual List<string> readClassNames(string filename)
        {

            // 转换为绝对路径
            string absoluteFilePath = Path.Combine(Application.dataPath, filename);


            List<string> classNames = new List<string>();

            System.IO.StreamReader cReader = null;
            try
            {
                cReader = new System.IO.StreamReader(absoluteFilePath, System.Text.Encoding.Default);

                while (cReader.Peek() >= 0)
                {
                    string name = cReader.ReadLine();
                    classNames.Add(name);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
            finally
            {
                if (cReader != null)
                    cReader.Close();
            }

            return classNames;
        }
    }
}