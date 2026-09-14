using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Split", menuName = "My Create Asset / MoveType / Split")]
public class SplitMove : MoveType
{
    [SerializeField] private int _splitCount = 6;
    [SerializeField] private GameObject _subBulletPrefab;
    [SerializeField] private int _maxSplitDepth = 2; // •ª—ô‚Å‚«‚éãŒÀ‰ñ”i˜A½”j

    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        if (Magic == null) return;

        if (rb != null)
        {
            rb.linearVelocity = Magic.forward * moveSpeed;
        }

        SplitBullet split = Magic.GetComponent<SplitBullet>();
        if (split == null)
        {
            split = Magic.gameObject.AddComponent<SplitBullet>();
            split.Init(_splitCount, _subBulletPrefab, moveSpeed, _maxSplitDepth, 0, null);
        }
    }
}

public class SplitBullet : MonoBehaviour
{
    private int _count;
    private GameObject _subPrefab;
    private float _speed;
    private int _maxDepth;
    private int _currentDepth;
    private MagicController _controller;
    private bool _hasSplit = false;

    // ’¼‘O‚É“–‚½‚Á‚½“G‚ÌID‚ğ‹L˜^iŠgU’¼Œã‚Ì¬’e‚ª‚»‚Ì“G‚É‘¦ƒqƒbƒg‚·‚é‚Ì‚ğ–h‚®j
    private HashSet<int> _ignoredEnemyIds = new HashSet<int>();

    public void Init(int count, GameObject subPrefab, float speed, int maxDepth, int currentDepth, HashSet<int> parentIgnoredIds)
    {
        _count = count;
        _subPrefab = subPrefab;
        _speed = speed;
        _maxDepth = maxDepth;
        _currentDepth = currentDepth;
        _controller = GetComponent<MagicController>();

        if (parentIgnoredIds != null)
        {
            _ignoredEnemyIds = new HashSet<int>(parentIgnoredIds);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (_hasSplit) return;

        // “G‚É“–‚½‚Á‚½ê‡
        if (col.CompareTag("Enemy"))
        {
            int enemyId = col.gameObject.GetInstanceID();

            // ’¼‘O‚É•ª—ôŒ³‚Ì’e‚ª“–‚½‚Á‚½“G‚Ìê‡‚Í–³‹‚·‚é
            if (_ignoredEnemyIds.Contains(enemyId)) return;

            // ƒ_ƒ[ƒWˆ—
            IDamageble damageObj = col.GetComponent<IDamageble>();
            if (damageObj == null) damageObj = col.GetComponentInParent<IDamageble>();

            if (damageObj != null && _controller != null)
            {
                if (_controller.Spark != null)
                {
                    Instantiate(_controller.Spark, transform.position, Quaternion.identity);
                }
                damageObj.AddDamage(Mathf.Max(1, _controller.TotalAttack));
            }

            // “–‚½‚Á‚½“G‚ÌID‚ğ‹L˜^‚µ‚ÄŠgU
            _ignoredEnemyIds.Add(enemyId);
            SpawnSubBullets();
        }
    }

    private void SpawnSubBullets()
    {
        if (_hasSplit) return;
        _hasSplit = true;

        // •ª—ô‚ÌãŒÀ‰ñ”‚É’B‚µ‚Ä‚¢‚ê‚ÎŠgU‚¹‚¸Á–Å
        if (_currentDepth >= _maxDepth)
        {
            Destroy(gameObject);
            return;
        }

        if (_subPrefab != null && _count > 0)
        {
            float angleStep = 360f / _count;
            for (int i = 0; i < _count; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                Vector3 spawnDirection = rotation * transform.forward;

                GameObject subBullet = Instantiate(_subPrefab, transform.position, Quaternion.LookRotation(spawnDirection));

                // –‚–@ƒXƒe[ƒ^ƒX‚Ìˆø‚«Œp‚¬
                MagicController subController = subBullet.GetComponent<MagicController>();
                if (subController == null) subController = subBullet.AddComponent<MagicController>();
                if (_controller != null) subController._baseData = _controller._baseData;

                // ƒXƒvƒŠƒbƒgƒXƒNƒŠƒvƒg‚Ì•t—^‚Æ˜A½[‚³(+1)‚ÌXV
                SplitBullet subSplit = subBullet.GetComponent<SplitBullet>();
                if (subSplit == null) subSplit = subBullet.AddComponent<SplitBullet>();

                // ƒqƒbƒg‚µ‚½“G‚ÌIDî•ñ‚ğ¬’e‚Ö“n‚·
                subSplit.Init(_count, _subPrefab, _speed, _maxDepth, _currentDepth + 1, _ignoredEnemyIds);

                // ‘¬“xİ’è
                Rigidbody subRb = subBullet.GetComponent<Rigidbody>();
                if (subRb != null)
                {
                    subRb.linearVelocity = spawnDirection * _speed;
                }
            }
        }

        Destroy(gameObject);
    }
}