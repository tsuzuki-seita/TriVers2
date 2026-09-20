using UnityEditor;
using UnityEngine;

public class BossDebugPanel : EditorWindow
{
    private static readonly Color SelectedColor = new Color(1f, 0.82f, 0.3f);

    [SerializeField] private BossDebugPanelIconSet _iconSet;

    private GUIStyle _behaviorButtonStyle;

    [MenuItem("Tools/Boss Debug Panel")]
    private static void Open()
    {
        GetWindow<BossDebugPanel>("Boss Debug Panel");
    }

    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Play Mode中のみ操作できます。", MessageType.Info);
            return;
        }

        BossPresenter presenter = FindFirstObjectByType<BossPresenter>();
        BossAI bossAI = presenter != null ? presenter.BossAI : null;
        if (bossAI == null)
        {
            EditorGUILayout.HelpBox("シーン上にBossPresenterが見つかりません。", MessageType.Warning);
            return;
        }

        BossView bossView = FindFirstObjectByType<BossView>();
        EnsureStyle();

        _iconSet = (BossDebugPanelIconSet)EditorGUILayout.ObjectField(
            "アイコンセット", _iconSet, typeof(BossDebugPanelIconSet), false);

        EditorGUILayout.LabelField("行動を強制実行", EditorStyles.boldLabel);

        var names = bossAI.BehaviorNames;
        GUILayout.BeginHorizontal();
        for (int i = 0; i < names.Count; i++)
        {
            bool isForced = bossAI.ForcedBehaviorIndex == i;
            GUIContent content = new GUIContent(names[i], GetIcon(bossView, names[i], _iconSet));

            Color prevColor = GUI.backgroundColor;
            if (isForced) GUI.backgroundColor = SelectedColor;

            using (new EditorGUI.DisabledScope(isForced))
            {
                if (GUILayout.Button(content, _behaviorButtonStyle, GUILayout.Width(130), GUILayout.Height(140)))
                {
                    bossAI.ForceBehavior(i);
                }
            }

            GUI.backgroundColor = prevColor;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(bossAI.ForcedBehaviorIndex == null))
        {
            if (GUILayout.Button("固定を解除（ランダム選択に戻す）"))
            {
                bossAI.ClearForcedBehavior();
            }
        }
    }

    private void EnsureStyle()
    {
        if (_behaviorButtonStyle != null) return;

        _behaviorButtonStyle = new GUIStyle(GUI.skin.button)
        {
            imagePosition = ImagePosition.ImageAbove,
            wordWrap = true,
            alignment = TextAnchor.LowerCenter,
            fontSize = 11,
        };
    }

    private static Texture GetIcon(BossView bossView, string behaviorName, BossDebugPanelIconSet iconSet)
    {
        if (iconSet != null)
        {
            Texture2D customIcon = iconSet.GetIcon(behaviorName);
            if (customIcon != null) return customIcon;
        }

        if (bossView == null) return null;

        GameObject prefab = behaviorName switch
        {
            "剣攻撃" => bossView.SwordAttackPrefab,
            "魔法" => bossView.MagicAttackPrefab,
            _ => null,
        };
        if (prefab == null) return null;

        Texture preview = AssetPreview.GetAssetPreview(prefab);
        return preview != null ? preview : AssetPreview.GetMiniThumbnail(prefab);
    }
}
