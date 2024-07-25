using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


namespace WW2NavalAssembly
{
    public static class MathTool
    {
        public static float GetArea(Vector3 v3)
        {
            float[] a = new float[] { v3.x, v3.y, v3.z };
            Array.Sort(a);
            return a[2] * a[1];
        }
        public static Vector2 GetRotatePosition(Vector2 targetPosition, Vector2 centerPosition, float angle)
        {
            float endX = (targetPosition.x - centerPosition.x) * Mathf.Cos(angle * Mathf.Deg2Rad) - (targetPosition.y - centerPosition.y) * Mathf.Sin(angle * Mathf.Deg2Rad) + centerPosition.x;
            float endY = (targetPosition.y - centerPosition.y) * Mathf.Cos(angle * Mathf.Deg2Rad) + (targetPosition.x - targetPosition.x) * Mathf.Sin(angle * Mathf.Deg2Rad) + centerPosition.y;
            return new Vector2(endX, endY);
        }
        public static void AxisLookAt(Transform tr_self, Vector3 lookPos, Vector3 directionAxis, float speed)
        {
            var rotation = tr_self.rotation;
            var targetDir = lookPos - tr_self.position;
            //指定哪根轴朝向目标,自行修改Vector3的方向
            var fromDir = tr_self.rotation * directionAxis;
            //计算垂直于当前方向和目标方向的轴
            var axis = Vector3.Cross(fromDir, targetDir).normalized;
            //计算当前方向和目标方向的夹角
            var angle = Vector3.Angle(fromDir, targetDir);
            //将当前朝向向目标方向旋转一定角度，这个角度值可以做插值
            tr_self.rotation = Quaternion.Lerp(rotation, Quaternion.AngleAxis(angle, axis) * rotation, speed);

        }//from CSDN
        public static Vector2 Get2DCoordinate(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        public static float Get2DDistance(Vector3 v1, Vector3 v2)
        {
            return Vector2.Distance(Get2DCoordinate(v1), Get2DCoordinate(v2));
        }
        public static Vector2 PointRotate(Vector2 center, Vector2 p1, float angle)
        {
            Vector2 tmp = new Vector2();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.x - center.x) * Math.Cos(angleHude) + (p1.y - center.y) * Math.Sin(angleHude) + center.x;
            double y1 = -(p1.x - center.x) * Math.Sin(angleHude) + (p1.y - center.y) * Math.Cos(angleHude) + center.y;
            tmp.x = (float)x1;
            tmp.y = (float)y1;
            return tmp;
        }
        public static float SignedAngle(Vector2 v1, Vector2 v2)
        {
            if (v1.x * v2.y - v1.y * v2.x < 0)
            {
                return -Vector2.Angle(v1, v2);
            }
            else
            {
                return Vector2.Angle(v1, v2);
            }
        }
        public static float GetInitialVel(float caliber, bool AA)
        {
            return (700 + 0.2f * (caliber - 100) + ((20000) / (caliber + 30))) * (AA ? 1.5f : 1f) / 2; // for 1:10
            //return (130 + 0.08f * (caliber + 50) + ((18000) / (caliber + 100))) * (AA ? 2 : 1); // for 1:20
        }
        public static int GetQueueIndex<T>(Queue<T> queue, T target)
        {
            int index = 0;
            foreach (T element in queue)
            {
                if (EqualityComparer<T>.Default.Equals(element, target))
                {
                    return index;
                }
                index++;
            }
            return -1; // Element not found
        }

        public static float DT_to_IK_angle(float angle, int index)
        {
            if (index == 1)
            {
                return -angle;
            }
            else
            {
                return angle;
            }
        }

        public static float[] Real_to_DT_angle(float[] angles)
        {
            float[] res = new float[6];
            res[0] = - angles[0];
            res[1] = angles[1] - 90f;
            res[2] = - (angles[2] + 90f);
            res[3] = angles[3] - 90f;
            res[4] = - (angles[4] - 90f);
            res[5] = angles[5] - 90f;
            return res;
        }
        public static float[] DT_to_Real_angle(float[] angles)
        {
            float[] res = new float[6];
            res[0] = -angles[0];
            res[1] = -angles[1] + 90f;
            res[2] = -angles[2] - 90f;
            res[3] = angles[3] + 90f;
            res[4] = -angles[4] + 90f;
            res[5] = angles[5] + 90f;
            return res;
        }

        public static float NormalizeAndAdjustAngle(float angle)
        {
            // Step 1: Normalize the angle to be within 0 to 360 degrees
            float normalizedAngle = angle % 360;
            if (normalizedAngle < 0)
            {
                normalizedAngle += 360;
            }

            // Step 2: If angle is greater than 180, adjust by subtracting 180
            if (normalizedAngle > 180)
            {
                normalizedAngle -= 180;
            }

            return normalizedAngle;
        }
    }
}
