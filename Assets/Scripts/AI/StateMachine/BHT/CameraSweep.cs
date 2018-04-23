using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    public class CameraSweep : ActionTask<Transform>
    {

        [RequiredField]
        public BBParameter<GameObject> rotationHead;
        public BBParameter<float> rotation;
        public BBParameter<float> rotationSpeed;
        public bool repeat = false;

        protected override string info
        {
            get { return "Rotating " + rotationHead; }
        }

        protected override void OnExecute() { SweepCamera(); }
        protected override void OnUpdate() { SweepCamera(); }

        void SweepCamera()
        {
            Quaternion currentRotation = rotationHead.value.transform.localRotation;
            Quaternion targetRotation = Quaternion.AngleAxis(rotation.value, Vector3.up);

            if (currentRotation != targetRotation)
            {
                Quaternion newRotation = Quaternion.RotateTowards(
                                 currentRotation,
                                 targetRotation,
                                 rotationSpeed.value * Time.deltaTime);

                rotationHead.value.transform.localRotation = newRotation;

            }
            else
            {
                rotation.value *= -1;
                currentRotation = targetRotation;
                targetRotation = Quaternion.AngleAxis(rotation.value, Vector3.up);
            }

            if (!repeat)
                EndAction(true);
        }
    }
}