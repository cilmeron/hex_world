using UnityEngine;
using UnityEngine.AI;

namespace git.Scripts.Components
{
    public class C_Moveable : MonoBehaviour
    {

        private Entity owner;
        [SerializeField] private Vector3 moveToPosition;
        [SerializeField] private Transform moveToTransform;
        [SerializeField] private bool shouldMove = true;
        [SerializeField] private float stoppingDistance = 2f;
        [SerializeField] public bool userTarget = false;
        private NavMeshAgent navMeshAgent;



        void Awake()
        {
            owner = GetComponent<Entity>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Start is called before the first frame update
        void Start()
        {
            EventManager.Instance.deathEvent.AddListener(ResetMoveToTransform);
            navMeshAgent.stoppingDistance = stoppingDistance;
            moveToPosition = transform.position;

            if (!shouldMove)
            {
                navMeshAgent.enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (navMeshAgent != null && navMeshAgent.enabled)
            {
                if (owner.CFormation.IsInFormation())
                {
                    navMeshAgent.stoppingDistance = 0;
                }
                else
                {
                    navMeshAgent.stoppingDistance = stoppingDistance;
                }
                navMeshAgent.SetDestination(GetDestination());
                if (owner != null)
                {
                    if (owner.detector != null)
                        owner.detector.GetRangeProjector().UpdateMaterialProperties();
                }
                if (navMeshAgent.remainingDistance < stoppingDistance) userTarget = false;
                if (owner.CCombat != null)
                {
                    owner.CCombat._attackDistanceDetector.GetRangeProjector().UpdateMaterialProperties();
                }
            }
        }

        private Vector3 GetDestination()
        {
            if (moveToTransform != null)
            {
                return moveToTransform.position;
            }

            return moveToPosition;
        }

        public void SetMoveToTransform(Transform moveToTransform, bool forceMove)
        {
            if (!userTarget) this.moveToTransform = moveToTransform;
        }

        public void SetMoveToPosition(Vector3 moveToPosition, bool forceMove)
        {
            userTarget = true;
            moveToTransform = null;
            C_Formation formation = owner.CFormation;
            if (formation != null)
            {
                if (formation.IsInFormation() && !formation.IsLeader() && !forceMove)
                {
                    return;
                }
            }
            this.moveToPosition = moveToPosition;
            owner.MovePlayer(moveToPosition);
            if (owner.CCombat != null)
            {
                owner.CCombat.userTarget = false;
                owner.CCombat.ResetTarget();
            }
        }

        public void EnableNavMesh(bool active)
        {
            navMeshAgent.enabled = active;
        }

        public NavMeshAgent NavMeshAgent
        {
            get => navMeshAgent;
        }

        private void ResetMoveToTransform(C_Health c)
        {
            if (moveToTransform != null && moveToTransform == c.transform)
            {
                moveToTransform = null;
            }
        }
    }
}