using UnityEngine;

public class MapSelectController : MonoBehaviour
{
    /// <summary>
    /// UIボタンのOnClickイベントから呼び出す処理
    /// </summary>
    public void OnSelectMap(MapData mapData)
    {
        if (mapData == null)
        {
            Debug.LogError("MapDataが割り当てられていません！");
            return;
        }

        if (SceneManager._instance == null)
        {
            Debug.LogError("SceneManagerが見つかりません！");
            return;
        }

        // SceneManagerに選択したマップを保存
        SceneManager._instance.SelectMapData = mapData;

        Debug.Log($"マップ選択: {mapData.name}");

        // 次のシーン（InGame）へ遷移
        SceneManager._instance.ChangeScene();
    }
}