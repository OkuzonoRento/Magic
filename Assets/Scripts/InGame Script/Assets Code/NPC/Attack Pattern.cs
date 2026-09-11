using System;
using UnityEngine;

// 1つの攻撃パターンのデータを定義する構造体
[Serializable]
public struct AttackPattern
{
    [Tooltip("識別名（例: Normal, Strong, Far）")]
    public string attackName;

    [Tooltip("アニメーションのTrigger名")]
    public string triggerName;

    [Tooltip("攻撃力（直接指定）")]
    public int damage;

    [Tooltip("攻撃ヒット判定の距離")]
    public float attackRadius;

    [Tooltip("攻撃ヒット判定の角度（片側）")]
    public float attackAngle;

    [Tooltip("発動可能な最大距離（この距離以内なら候補に入る）")]
    public float maxDistance;

    [Tooltip("発動可能な最小距離（遠距離攻撃用：近すぎると使わない）")]
    public float minDistance;
}