using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MagicRogue
{
    public enum MapType
    {
        Normal, // 通常戦闘
        Event,  // イベント（敵大量発生など）
        Boss    // ボス
    }

    public enum ClearConditionType
    {
        KillCount,     // 規定数の敵撃破
        SurviveTime,   // 一定時間生存（敵撃破で時間短縮）
        BossDefeat     // ボス撃破
    }

    [Serializable]
    public struct EnemySpawnConfig
    {
        [Tooltip("スポーンさせる敵のプレハブ")]
        public GameObject enemyPrefab;

        [Tooltip("出現重み（数字が大きいほど出現しやすい）")]
        [Range(1, 100)]
        public int spawnWeight;
    }

    [CreateAssetMenu(fileName = "NewMapData", menuName = "MagicRogue/Map Data")]
    public class MapData : ScriptableObject
    {
        [Header("マップ種別・選択画面用情報")]
        [Tooltip("マップの種別（Normal / Event / Boss）")]
        public MapType mapType = MapType.Normal;

        [Tooltip("クリア条件のタイプ")]
        public ClearConditionType clearConditionType = ClearConditionType.SurviveTime;

        [Tooltip("遷移先のシーン名・アセット名（例: Plain）")]
        public string mapName;

        [Tooltip("UI表示名（例: 深淵の森）")]
        public string displayName;

        [Tooltip("選択画面で回転表示する3Dモデルのプレハブ")]
        public GameObject model3DPrefab;

        [TextArea]
        [Tooltip("選択画面で表示するマップの説明文")]
        public string description;

        [Header("インゲーム生成設定")]
        [Tooltip("インゲーム中に生成するステージモデル全体のプレハブ")]
        public GameObject mapStagePrefab;

        [Tooltip("プレイヤーの初期スポーン位置")]
        public Vector3 playerSpawnPosition = Vector3.zero;

        [Tooltip("プレイヤーの初期スポーン回転")]
        public Vector3 playerSpawnRotation = Vector3.zero;

        [Header("スポーン設定")]
        [Tooltip("このマップに出現する敵とその確率リスト")]
        public List<EnemySpawnConfig> spawnableEnemies = new List<EnemySpawnConfig>();

        [Tooltip("同時に存在できる最大敵数")]
        public int maxEnemyCount = 20;

        [Tooltip("スポーン間隔（秒）")]
        public float spawnInterval = 3f;

        [Header("クリア・ポータル設定")]
        [Tooltip("クリアに必要な目標生存時間（秒）")]
        public float targetSurviveTime = 60f;

        [Tooltip("敵を1体撃破した際に短縮される制限時間（秒）")]
        public float timeReducePerKill = 3f;

        [Tooltip("ステージクリアに必要な敵撃破数（KillCount条件用）")]
        public int targetKillCount = 15;

        [Tooltip("クリア時に出現するポータルのプレハブ")]
        public GameObject portalPrefab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(mapName)) return;

            string assetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(assetPath)) return;

            string currentAssetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            if (currentAssetName != mapName)
            {
                EditorApplication.delayCall += () =>
                {
                    if (this == null) return;

                    string path = AssetDatabase.GetAssetPath(this);
                    if (!string.IsNullOrEmpty(path))
                    {
                        string result = AssetDatabase.RenameAsset(path, mapName);
                        if (string.IsNullOrEmpty(result))
                        {
                            AssetDatabase.SaveAssets();
                        }
                    }
                };
            }
        }
#endif
    }
}