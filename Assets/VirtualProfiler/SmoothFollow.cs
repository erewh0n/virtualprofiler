using UnityEngine;

namespace Assets.VirtualProfiler
{
    public class SmoothFollow : MonoBehaviour
    {
        private const float SmoothTime = 0.3f;
        public Transform Target;

        public void LateUpdate()
        {
            transform.position = Target.position - 
                ((Quaternion.Euler(0, 
                                   Mathf.LerpAngle(transform.eulerAngles.y,
                                                   Target.eulerAngles.y,
                                                   2.0f * Time.deltaTime),
                                   0))
                 * Vector3.forward * 500.0f);

            transform.Translate(0, Target.position.y + Global.Instance.DistanceAboveSubject, 0);

            transform.LookAt(Target);
        }

    }
}
