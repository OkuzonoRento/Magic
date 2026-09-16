using UnityEngine;
using BuffSystem.Core;

namespace BuffSystem.Core
{
    public class InGameBuffManager : MonoBehaviour
    {
        private void Start()
        {
            ApplyGlobalBuffs();
        }

        public void ApplyGlobalBuffs()
        {
            if (global::SceneManager._instance == null) return;

            var globalBuffs = global::SceneManager._instance.GlobalBuffs;

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                BuffHandler playerHandler = player.GetComponent<BuffHandler>();
                if (playerHandler == null) playerHandler = player.AddComponent<BuffHandler>();

                foreach (var buff in globalBuffs)
                {
                    if (buff.TargetType == BuffTargetType.Player || buff.TargetType == BuffTargetType.Both)
                    {
                        playerHandler.AddBuff(buff);
                    }
                }
            }

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                ApplyBuffsToEnemy(enemy);
            }
        }

        public static void ApplyBuffsToEnemy(GameObject enemyObj)
        {
            if (enemyObj == null || global::SceneManager._instance == null) return;

            BuffHandler enemyHandler = enemyObj.GetComponent<BuffHandler>();
            if (enemyHandler == null) enemyHandler = enemyObj.AddComponent<BuffHandler>();

            var globalBuffs = global::SceneManager._instance.GlobalBuffs;
            foreach (var buff in globalBuffs)
            {
                if (buff.TargetType == BuffTargetType.Enemy || buff.TargetType == BuffTargetType.Both)
                {
                    enemyHandler.AddBuff(buff);
                }
            }
        }
    }
}