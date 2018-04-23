using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    public class LookAt : ActionTask<Transform>
    {

        [RequiredField]
        public BBParameter<GameObject> lookTarget;
        public BBParameter<float> lerpSpeed;
        public bool repeat = false;

        protected override string info
        {
            get { return "LookAt " + lookTarget; }
        }

        protected override void OnExecute() { DoLook(); }
        protected override void OnUpdate() { DoLook(); }

        void DoLook()
        {
            //var lookPos = lookTarget.value.transform.position;
            //lookPos.y = agent.position.y;
            //agent.LookAt(lookPos);

            Vector3 targetPoint = new Vector3(lookTarget.value.transform.position.x, agent.transform.position.y,
lookTarget.value.transform.position.z) - agent.transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);

            agent.rotation = Quaternion.Slerp(agent.transform.GetChild(0).rotation,
                targetRotation, Time.deltaTime * lerpSpeed.value);

            if (!repeat)
                EndAction(true);
        }


    }
}