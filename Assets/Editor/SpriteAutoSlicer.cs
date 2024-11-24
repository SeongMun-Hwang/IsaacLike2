using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

public class SpriteAutoSlicer : EditorWindow
{
    [MenuItem("Tools/Sprite Auto Slicer")]
    private static void ShowWindow()
    {
        GetWindow<SpriteAutoSlicer>("Sprite Auto Slicer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Auto Slicer", EditorStyles.boldLabel);

        if (GUILayout.Button("Slice Selected Sprites"))
        {
            SliceSelectedSprites();
        }
    }

    private void SliceSelectedSprites()
    {
        // ������ ������Ʈ ��������
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);

            // TextureImporter ��������
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null || importer.textureType != TextureImporterType.Sprite)
            {
                Debug.LogWarning($"'{obj.name}' is not a valid Sprite texture.");
                continue;
            }

            importer.spriteImportMode = SpriteImportMode.Multiple; // ���� ��������Ʈ ��� ����

            // �ڵ� �����̽� ����
            SpriteMetaData[] spriteData = AutomaticSlice(importer);
            if (spriteData != null)
            {
                // Importer�� ��Ÿ������ ����
                ApplySpriteMetaData(importer, spriteData);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                Debug.Log($"'{obj.name}' sliced successfully.");
            }
            else
            {
                Debug.LogWarning($"No valid sprites found in '{obj.name}'.");
            }
        }
    }

    private SpriteMetaData[] AutomaticSlice(TextureImporter importer)
    {
        // Texture ������ ��������
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
        if (texture == null) return null;

        List<SpriteMetaData> spriteMetaDataList = new List<SpriteMetaData>();

        // �ؽ�ó�� ���� ������ �������� Rect ����
        Rect[] rects = InternalSpriteUtility.GenerateAutomaticSpriteRectangles(texture, 0, 0);
        foreach (Rect rect in rects)
        {
            SpriteMetaData metaData = new SpriteMetaData
            {
                rect = rect,
                name = $"{texture.name}_{spriteMetaDataList.Count}",
                pivot = new Vector2(0.5f, 0f) // Bottom �ǹ�
            };
            spriteMetaDataList.Add(metaData);
        }

        return spriteMetaDataList.ToArray();
    }

    private void ApplySpriteMetaData(TextureImporter importer, SpriteMetaData[] spriteData)
    {
        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);

        // �ʿ��� ���� ����
        settings.spriteMode = (int)SpriteImportMode.Multiple;
        settings.spritePixelsPerUnit = 100f; // �ʿ信 ���� ���� ����

        importer.SetTextureSettings(settings);
        importer.spritesheet = spriteData; // spritesheet ��� ��Ÿ������ ����
    }
}
