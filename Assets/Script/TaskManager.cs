using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WW2NavalAssembly;

public class ArmTask
{
    public enum Type
    {
        Finished,
        MoveObject,
        GrabObject
    }
    public Type type = Type.Finished;

    // moveObject
    public Vector3 move_start;
    public Vector3 move_start_right;
    public Vector3 move_end;
    public Vector3 move_end_right;

    // grab
    public Transform grab_hand;

    public void InitAsMoveObject(Vector3 start, Vector3 end, Vector3 start_right, Vector3 end_right)
    {
        type = Type.MoveObject;
        move_start = start;
        move_end = end;
        move_start_right = start_right;
        move_end_right = end_right;
    }
    public void InitAsGrabObject(Vector3 start, Vector3 start_right, Transform hand)
    {
        type = Type.GrabObject;
        move_start = start;
        move_start_right = start_right;
        grab_hand = hand;
    }
}
public class TaskManager : MonoBehaviour // hold the coroutine of tasks
{
    public ArmTask task = new ArmTask();

    public Transform ArmEnd;
    public IKController IKSolver;
    public RouteGenerator routeGenerator;
    public JakaController jaka;

    public ArmDTController ArmDT;
    

    Coroutine taskRoutine;

    GameObject taskSouce;

    public bool jakaRunning = false;

    public bool Cancel = false;


    public void JakaFinished()
    {
        jakaRunning = false;
    }

    public void GetTask(ArmTask t, GameObject sender)
    {
        
        if (task.type == ArmTask.Type.Finished)
        {
            task = t;
            taskSouce = sender;
            Debug.Log("New " + task.type.ToString() + " task assigned.");
            
            if (taskRoutine != null)
            {
            }
            else
            {
                if (t.type == ArmTask.Type.MoveObject)
                {
                    taskRoutine = StartCoroutine(ExecuteMoveObject());
                }
                else
                {
                    taskRoutine = StartCoroutine(ExecuteGetObject());
                }
                
            }
        }
        else
        {
            Debug.Log("Current task not finished.");
        }
    }

    public bool RestRouteValid(int i)
    {
        return routeGenerator.RestRouteValid(i);
    }
    IEnumerator ExecuteMoveObject()
    {
        taskSouce.GetComponent<ObjectFrame>().SwitchTaskMode(true);
        Debug.Log("Start move object coroutine.");
        // generate the action sequence
        List<KeyPoint> turningPoint = new List<KeyPoint>();
        turningPoint.Add(new KeyPoint(ArmEnd.position, task.move_start_right, false));
        turningPoint.Add(new KeyPoint(task.move_start + Vector3.up * 0.07f, task.move_start_right,false));
        turningPoint.Add(new KeyPoint(task.move_start, task.move_start_right, true));
        turningPoint.Add(new KeyPoint(task.move_start + Vector3.up * 0.07f, task.move_end_right, true));
        turningPoint.Add(new KeyPoint(task.move_end + Vector3.up * 0.07f, task.move_end_right, true));
        turningPoint.Add(new KeyPoint(task.move_end, task.move_end_right, false));
        turningPoint.Add(new KeyPoint(task.move_end + Vector3.up * 0.07f, default, false));
        routeGenerator.SetRoute(turningPoint);
        bool solveable = routeGenerator.CalculateSequence();

        if (!solveable)
        {
            Debug.Log("Unable to solve the arm action");
        }
        else
        {
            // generate vis
            routeGenerator.GenerateGhost();

            // execute action
            bool pre_grab = false;
            int index = 0;
            foreach (ArmAction action in routeGenerator.actionSequence)
            {
                if (Cancel)
                {
                    break;
                }
                while (!RestRouteValid(index))
                {
                    yield return new WaitForSeconds(0.2f);
                }
                
                jaka.SetJointRot(action.angles);
                yield return new WaitForSeconds(0.3f);
                if (pre_grab != action.grab)
                {
                    while((action.position - ArmEnd.position).magnitude > 0.02f)
                    {
                        yield return new WaitForSeconds(0.2f);
                    }
                    pre_grab = action.grab;
                    jaka.SetGripper(!action.grab);
                    yield return new WaitForSeconds(0.5f);
                }
                index++;
            }
            // cancel aim box when finish
            try
            {
                taskSouce.GetComponent<ObjectFrame>().SwitchTaskMode(false);
                taskSouce.GetComponent<ObjectFrame>().CancelAim();
            }
            catch { }
        }
        routeGenerator.ResetRoute();
        taskRoutine = null;
        taskSouce = null;
        task = new ArmTask();
        Debug.Log("Task Finished");
        Cancel = false;
        yield break;
    }

    IEnumerator ExecuteGetObject()
    {
        taskSouce.GetComponent<ObjectFrame>().SwitchTaskMode(true);
        Debug.Log("Start move object coroutine.");
        // generate the action sequence
        List<KeyPoint> turningPoint = new List<KeyPoint>();
        turningPoint.Add(new KeyPoint(ArmEnd.position, task.move_start_right, false));
        turningPoint.Add(new KeyPoint(task.move_start + Vector3.up * 0.07f, task.move_start_right, false));
        turningPoint.Add(new KeyPoint(task.move_start, task.move_start_right, true));
        turningPoint.Add(new KeyPoint(task.move_start + Vector3.up * 0.07f, task.move_end_right, true));// when the arm grab the object up

        routeGenerator.SetRoute(turningPoint);
        bool solveable = routeGenerator.CalculateSequence();

        if (!solveable)
        {
            Debug.Log("Unable to solve the arm action");
        }
        else
        {
            // generate vis
            routeGenerator.GenerateGhost();

            // execute action
            bool pre_grab = false;
            int index = 0;
            foreach (ArmAction action in routeGenerator.actionSequence)
            {
                while (!RestRouteValid(index))
                {
                    yield return new WaitForSeconds(0.2f);
                }

                jaka.SetJointRot(action.angles);
                yield return new WaitForSeconds(0.3f);
                if (pre_grab != action.grab)
                {
                    while ((action.position - ArmEnd.position).magnitude > 0.02f)
                    {
                        yield return new WaitForSeconds(0.2f);
                    }
                    pre_grab = action.grab;
                    jaka.SetGripper(!action.grab);
                    yield return new WaitForSeconds(0.5f);
                }
                index++;
            }
            routeGenerator.ResetRoute();
            Debug.Log("Finish Grabbing");
            while (
                Vector3.Angle(task.grab_hand.forward, Vector3.up) > 30f ||
                (task.grab_hand.position + Vector3.up * 0.03f - ArmEnd.position).magnitude > 0.04f
                )
            {
                bool palm_up = Vector3.Angle(task.grab_hand.forward, Vector3.up) < 30f;

                turningPoint = new List<KeyPoint>();
                turningPoint.Add(new KeyPoint(task.grab_hand.position + Vector3.up * (palm_up? 0.03f: 0.12f), task.grab_hand.right, true));
                routeGenerator.SetRoute(turningPoint, true);
                solveable = routeGenerator.CalculateSequence();
                if (!solveable)
                {
                    Debug.Log("Unable to solve the arm action");
                }
                else
                {
                    foreach (ArmAction action in routeGenerator.actionSequence)
                    {
                        jaka.SetJointRot(action.angles);
                    }
                }
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(0.4f);
            turningPoint = new List<KeyPoint>();
            turningPoint.Add(new KeyPoint(task.grab_hand.position + Vector3.up * 0.01f, task.grab_hand.right, true));
            routeGenerator.SetRoute(turningPoint, true);
            solveable = routeGenerator.CalculateSequence();
            if (!solveable)
            {
                jaka.SetGripper(true);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                foreach (ArmAction action in routeGenerator.actionSequence)
                {
                    jaka.SetJointRot(action.angles);
                    yield return new WaitForSeconds(0.3f);
                }
                jaka.SetGripper(true);
                yield return new WaitForSeconds(0.5f);
            }
            
            Debug.Log("Release object on hand");

            turningPoint = new List<KeyPoint>();
            turningPoint.Add(new KeyPoint(task.move_start + Vector3.up * 0.07f, task.move_start_right, false));
            routeGenerator.SetRoute(turningPoint, true);
            solveable = routeGenerator.CalculateSequence();
            if (!solveable)
            {
            }
            else
            {
                foreach (ArmAction action in routeGenerator.actionSequence)
                {
                    jaka.SetJointRot(action.angles);
                }
            }
        }
        routeGenerator.ResetRoute();
        taskRoutine = null;
        taskSouce.GetComponent<ObjectFrame>().SwitchTaskMode(false);
        taskSouce = null;
        task = new ArmTask();
        Debug.Log("Task Finished");
        yield break;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
