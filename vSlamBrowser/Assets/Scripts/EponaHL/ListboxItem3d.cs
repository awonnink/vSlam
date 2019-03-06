using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSlamHL
{
    public class ListboxItem3d : MonoBehaviour
    {

        public Transform previous;
        public Vector3 startPosition;
        public Vector3 startLookAt;
        public VSlamHL.Listbox3d listbox3d;
        float timefactorspeed = 2.0f;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float timeFact = timefactorspeed * Time.deltaTime;
            if (startPosition == Vector3.zero && previous != null)
            {
                if (listbox3d != null)
                {
                    if (Vector3.Distance(transform.position, previous.position) > 0.0001f)
                    {
                        float dis = listbox3d.GetDistance(transform.position);
                        float cdis = Vector3.Distance(transform.position, previous.position);
                        if (cdis > dis)
                        {
                            transform.localPosition = Vector3.Lerp(transform.localPosition, previous.localPosition, timeFact);
                            transform.localRotation = Quaternion.Lerp(transform.localRotation, previous.localRotation, timeFact);
                            VSlamHL.DuoVector3 v = listbox3d.GetClosestDuoVector(transform.position);
                            if (v.Forward != Vector3.zero)
                            {
                                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(v.Forward), timeFact);
                            }
                            transform.localPosition = Vector3.Lerp(transform.localPosition, v.Position, timeFact);
                        }
                    }
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, timeFact);
                if (startLookAt != Vector3.zero)
                {
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(startLookAt), timeFact);
                }
            }
        }
    }
}
