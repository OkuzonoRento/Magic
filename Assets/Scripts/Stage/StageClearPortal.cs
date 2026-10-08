using System.Collections;
using UnityEngine;

namespace MagicRogue
{
    [RequireComponent(typeof(Collider))]
    public class StageClearPortal : MonoBehaviour
    {
        [Header("クリア時設定")]
        [SerializeField] private int stageClearGold = 100; // クリア獲得ゴールド

        private bool isTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (isTriggered || !other.CompareTag("Player")) return;

            isTriggered = true;

            // プレイヤーの操作停止・無敵化
            if (other.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.SetInvincible(true);
                playerController.SetControlActive(false);
            }

            // ★ ResultUI があればクリアリザルト表示、なければ直でショップへ
            if (ResultUI.Instance != null)
            {
                ResultUI.Instance.OnEnterPortal(stageClearGold);
            }
            else if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.OnStageCleared();
            }
        }
    }
}