using UnityEngine;
using UnityEngine.AI;

public class WOMAN : MonoBehaviour
{
    private Animator animator;
    Transform target;
    NavMeshAgent agent;
    public float LookRadius;
    public int count = 0;
    private bool flag = false;
    private bool flag_1 = false;
    private Vector3 previoius_position;
    private Vector3 current_position;
    public float time_reload = 0;
    [SerializeField] float fixedYPosition = 4f; // Желаемая фиксированная позиция по Y

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = PlayerController1.instance.controller.transform;
        animator = GetComponent<Animator>();

        // Фиксируем начальную позицию по Y
        transform.position = new Vector3(transform.position.x, fixedYPosition, transform.position.z);
        previoius_position = transform.position;
    }

    void Update()
    {
        current_position = transform.position;
        count++;
        // Создаем новую позицию цели с фиксированным Y
        Vector3 targetFlatPosition = new Vector3(target.position.x, fixedYPosition, target.position.z);

        float distance = Vector3.Distance(targetFlatPosition, transform.position);
        if (distance <= LookRadius)
        {
            // Назначаем точку назначения с корректным Y
            agent.SetDestination(targetFlatPosition);
            animator.SetBool("angry", false);
            animator.SetBool("idle", false);
            animator.SetBool("run", true);
            animator.SetBool("angry_idle", false);
            flag = true;
            if (distance <= agent.stoppingDistance)
            {
                time_reload += Time.deltaTime;
                LookTarget();
                if (time_reload >= 5f || time_reload >= 0f && time_reload <= 1f)
                {
                    PlayerController1.instance.health -= 10;
                    time_reload = 1.1f;
                    animator.SetBool("hit", true);
                    Debug.Log("Урон нанесен!");
                }
                else if (time_reload >= 1f && time_reload <= 2f)
                {
                    animator.SetBool("hit", false);
                }

            }
            else
            {
                time_reload = 0f;
                animator.SetBool("hit", false);
            }
        }
        else if(LookRadius >= 10 && LookRadius <= 15 && !flag_1)
        {
            animator.SetBool("angry", true);
            animator.SetBool("idle", false);
            animator.SetBool("run", false);
            animator.SetBool("angry_idle", false);
            flag_1 = true;
        }
        else
        {
            animator.SetBool("angry", false);
            animator.SetBool("idle", false);
            animator.SetBool("run", false);
            animator.SetBool("angry_idle", true);
            flag = false;
        }
        if (count > 900 && !flag)
        {
            animator.SetBool("angry", false);
            animator.SetBool("idle", true);
            animator.SetBool("run", false);
            animator.SetBool("angry_idle", false);
            count = 0;
            flag_1 = false;
        }
        
        transform.position = new Vector3(transform.position.x, fixedYPosition, transform.position.z);
        //if (HasPositionChanged())
        //{
        //    animator.SetBool("angry_idle", true);
        //}
        previoius_position = current_position;
    }

    void LookTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Игнорируем вертикальную составляющую для поворота
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, LookRadius);
    }

    //bool HasPositionChanged()
    //{
    //    return Vector3.Distance(current_position, previoius_position) < 0.01f;
    //}
}
