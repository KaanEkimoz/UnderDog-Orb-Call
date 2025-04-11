using com.absence.attributes;
using com.game.enemysystem.ai;
using com.game.enemysystem.statsystemextensions;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace com.game.enemysystem
{
    public class Enemy : MonoBehaviour
    {
        public static void DoFakify(Enemy target)
        {
            target.Fakify();
        }

        public static void DoVirtualize(Enemy target)
        {
            target.IsVirtual = true;
            target.enabled = false;
        }

        [Header("Movement")]
        [SerializeField] protected EnemyAIBuitlInScriptSelector enemyAISelector;
        [SerializeField] public bool hasAttackAnimation;
        [SerializeField] protected bool canMoveDuringAttack;
        [Header("Slow")]
        [SerializeField] public float slowPercentPerOrb = 25f;
        [Header("Stats")]
        [SerializeField] protected EnemyStats enemyStats;

        //AI
        protected IEnemyAI ai;
        protected GameObject target;
        bool aiLocked;

        //Movement
        private float defaultSpeed;

        public bool IsFake { get; set; } = false;
        public bool IsVirtual { get; protected set; } = false;
        public bool IsAttacking { get; protected set; } = false;
        public IEnemyAI AI => ai;
        public virtual bool IsAILocked
        {
            get
            {
                return aiLocked;
            }

            set
            {
                aiLocked = value;
            }
        }

        //Slow
        private float currentSlowAmount = 0;

        //Dummy Mode
        [Header("Dummy Mode")]
        [SerializeField] protected bool isDummyModeActive = false;

        protected virtual void Start()
        {
            if (isDummyModeActive)
                return;

            if (target == null)
                target = GameObject.FindWithTag("Player");

            if (enemyStats == null)
                GetComponent<EnemyStats>();

            ai = enemyAISelector.Current;
            ai.Initialize(target.transform);
            ai.Speed = defaultSpeed + enemyStats.GetStat(EnemyStatType.WalkSpeed);
        }
        protected virtual void Update()
        {
            if (isDummyModeActive || target == null)
                return;

            ai.Speed =
                (ai.DefaultSpeed + enemyStats.GetStat(EnemyStatType.WalkSpeed)) *
                (1 - (currentSlowAmount / 100));

            if (!IsAILocked)
                ai.Refresh(target.transform);

            CustomUpdate();
        }
        protected virtual void CustomUpdate() { }
        protected virtual void Fakify() { }

        public void ApplySlowForSeconds(float slowPercent, float duration)
        {
            StartCoroutine(SlowForSeconds(slowPercent, duration));
        }
        public void ApplyKnockbackForce(Vector3 orbPosition, float knocbackForce)
        {
            Vector3 forceDirection = new Vector3(transform.position.x - orbPosition.x, 0, transform.position.z - orbPosition.z).normalized;
            Vector3 force = forceDirection * knocbackForce;
            transform.DOMove(transform.position + force, 0.4f).SetEase(Ease.OutBack);
        }
        private IEnumerator StopKnockback()
        {
            yield return new WaitForSeconds(0.2f);
            //navMeshAgent.isStopped = false;
        }
        public void ApplySlowForOrbsOnEnemy(int orbCount)
        {
            currentSlowAmount = slowPercentPerOrb * orbCount;

            if (currentSlowAmount > 100)
                currentSlowAmount = 100;
        }
        private IEnumerator SlowForSeconds(float slowPercent, float duration)
        {
            currentSlowAmount = slowPercent;
            yield return new WaitForSeconds(duration);
            currentSlowAmount = 0;
        }
        protected bool GetDistanceToPlayer()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= ai.MovementData.stoppingDistance;
        }
        public void CommitEndOfAttack()
        {
            IsAttacking = false;
        }
    }


}