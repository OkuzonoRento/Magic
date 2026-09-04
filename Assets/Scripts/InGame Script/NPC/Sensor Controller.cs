using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class SensorController : MonoBehaviour
{
    [SerializeField] private EnemyStatusBaseData _baseData;
    private SphereCollider _searchArea = default;
    private float _searchAngle;
    private NPCController _NPC_controller;
    private float _attackCoolTimer = 0f;
    public Animator _animator;
    private IDamageble player;

    private void Start()
    {
        _NPC_controller = transform.parent.GetComponent<NPCController>();
        _searchArea = gameObject.GetComponent<SphereCollider>();
        _searchAngle = _baseData.GetSearchAngle();
        _searchArea.radius = _baseData.GetSearchRadius();
        _animator = transform.parent?.GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player" && _searchArea != null)
        {
            _attackCoolTimer += Time.deltaTime;
            player = col.gameObject.GetComponent<IDamageble>();
            var playerDirection = col.transform.position - transform.position;
            var angle = Vector3.Angle(transform.forward, playerDirection);
            var dis = Vector3.Distance(col.gameObject.transform.position, transform.position);

            if (angle <= _searchAngle)
            {
                if (dis <= _searchArea.radius * 0.15f && dis >= _searchArea.radius * 0.0f)      //距離に応じて行動を設定する処理　　　　
                {
                    if (_attackCoolTimer >= _baseData.GetAttackCoolTime())
                    {
                        _attackCoolTimer = 0f;
                        _animator.SetTrigger("Attack");
                        player.AddDamage(_baseData.GetAttack());
                    }
                    _NPC_controller.SetState(NPCController.NPC_State.Chase, col.gameObject.transform);
                }
                else if (dis <= _searchArea.radius * 0.8f && dis >= _searchArea.radius * 0.15f)
                {
                    _NPC_controller.SetState(NPCController.NPC_State.Chase, col.gameObject.transform);
                }
                else if (dis <= _searchArea.radius * 1.0f && dis >= _searchArea.radius * 0.8f)
                {
                    _NPC_controller.SetState(NPCController.NPC_State.Chase, col.gameObject.transform);
                }
            }
            else if (angle > _searchAngle)
            {
                if (dis <= _searchArea.radius * 0.5f && dis >= _searchArea.radius * 0.0f)
                {
                    _NPC_controller.SetState(NPCController.NPC_State.Chase, col.gameObject.transform);
                }
                else if (dis <= _searchArea.radius * 1.0f && dis >= _searchArea.radius * 0.5f)
                {
                    _NPC_controller.SetState(NPCController.NPC_State.Idle);
                }
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            _NPC_controller.SetState(NPCController.NPC_State.Return);
        }
    }

    public void PlayerAttack()
    {
        player.AddDamage(_baseData.GetAttack());
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (_baseData.GetEditor() && _searchArea != null)
        {
            Handles.color = Color.green;
            Handles.DrawSolidArc(transform.position, Vector3.up,
                Quaternion.Euler(0.0f, -_searchAngle, 0.0f) *
                transform.forward, _searchAngle * 2.0f, _searchArea.radius * 1.0f);
        }
    }

#endif
}
