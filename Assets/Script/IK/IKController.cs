using System.Collections.Generic;
using UnityEngine;
using WW2NavalAssembly;

// ReSharper disable once InconsistentNaming
public class IKController : MonoBehaviour {
    [SerializeField] private Transform _targetTransform;
    public ArmJoint[] Joints;
    public float[] Angles;

    private const float SamplingDistance = 0.1f;
    private const float LearningRate = 100f;
    private const float DistanceThreshold = 0.002f;

    public bool IK = false;

    public int maxStep = 200;

    public ArmDTController ArmDT; // for the initial state of arm (reset angle)


    private void Start() {
        float[] angles = new float[Joints.Length];
        
        for (int i = 0; i < Joints.Length; i++) {
            if (Joints[i]._rotationAxis == 'x') {
                angles[i] = Joints[i].transform.localRotation.eulerAngles.x;
            }
            else if (Joints[i]._rotationAxis == 'y') {
                angles[i] = Joints[i].transform.localRotation.eulerAngles.y;
            }
            else if (Joints[i]._rotationAxis == 'z') {
                angles[i] = Joints[i].transform.localRotation.eulerAngles.z;
            }
        }
        Angles = angles;
    }

    private void Update() {
        if (IK)
        {
            IK = false;
            InverseKinematics(GetPositionForJ4(_targetTransform.position));
        }
    }

    public void ResetAngle()
    {
        float[] angles = ArmDT.Rotate;
        for (int i = 0; i < Angles.Length;i++)
        {
            Angles[i] = MathTool.DT_to_IK_angle(angles[i], i);
        }
    }


    public Vector3 GetPositionForJ4(Vector3 pos)
    {
        Vector3 forward = pos - Joints[0].transform.position;
        forward.y = 0;
        float angle = Mathf.Acos(0.1148f / forward.magnitude);
        Vector3 left = Quaternion.AngleAxis(-angle * 180f/Mathf.PI, Vector3.up) * forward;
        left = left.normalized * 0.1148f;
        Vector3 offset = forward - left;
        offset = offset.normalized * 0.1175f;
        return pos + Vector3.up * 0.2446f - offset;
    }

    public bool InverseKinematics(Vector3 pos)
    {
        int step = 0;
        while(step < maxStep && DistanceFromTarget(pos, Angles) >= DistanceThreshold)
        {
            step++;
            InverseKinematicsOneStep(pos, Angles);
        }
        return step < 1000;
    }

    public Vector3 ForwardKinematics(float[] angles) {
        Vector3 prevPoint = Joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < Joints.Length; i++) {
            // Rotates around a new axis
            rotation *= Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].RotationAxis);
            Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

            prevPoint = nextPoint;
        }
        return prevPoint;
    }

    public float DistanceFromTarget(Vector3 target, float[] angles) {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    public float PartialGradient(Vector3 target, float[] angles, int i) {
        // Saves the angle,
        // it will be restored later
        float angle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);

        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);

        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Restores
        angles[i] = angle;

        return gradient;
    }

    public void InverseKinematicsOneStep(Vector3 target, float[] angles) {
        if (DistanceFromTarget(target, angles) < DistanceThreshold)
            return;

        for (int i = Joints.Length - 1; i >= 0; i--) {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            float gradient = PartialGradient(target, angles, i);
            angles[i] -= LearningRate * gradient;

            // Early termination
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
                return;
            
            switch (Joints[i]._rotationAxis) {
                case 'x':
                    Joints[i].transform.localEulerAngles = new Vector3(angles[i], 0, 0);
                    break;
                case 'y':
                    Joints[i].transform.localEulerAngles = new Vector3(0, angles[i], 0);
                    break;
                case 'z':
                    Joints[i].transform.localEulerAngles = new Vector3(0, 0, angles[i]);
                    break;
            }
        }
    }
}