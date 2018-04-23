using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    public class CallGuards : ActionTask<GameObject>
    {
        public BBParameter<Transform> self;
        public BBParameter<GameObject> player;

        Vector3 sawPlayerAtThisPosition;

        protected override string info
        {
            get { return "Guard Called to " + sawPlayerAtThisPosition + " position"; }
        }

        protected override void OnExecute() { GuardsCalled(); }
        protected override void OnUpdate() { GuardsCalled(); }

        void GuardsCalled()
        {
            GameObject[] guards = GameObject.FindGameObjectsWithTag("AI_Guard");
            GameObject nearestGuard = GetClosestGuard(guards);
            sawPlayerAtThisPosition = player.value.transform.position;
            nearestGuard.GetComponent<Blackboard>().SetValue("SawPlayerAtThisPosition", sawPlayerAtThisPosition);
            nearestGuard.GetComponent<Blackboard>().SetValue("CalledToSearchPlayer", true);
            EndAction(true);
        }

        GameObject GetClosestGuard(GameObject[] guards)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = self.value.position;
            foreach (GameObject potentialTarget in guards)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

            return bestTarget;
        }
    }
}