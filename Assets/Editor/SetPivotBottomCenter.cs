using UnityEngine;
using UnityEditor;

public class SetPivotBottomCenter : MonoBehaviour
{
    [MenuItem("Tools/Set Pivot to Bottom Center")]
    static void SetPivot()
    {
        Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        foreach (Texture2D tex in textures)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            // Nếu sprite mode = Single
            if (importer.spriteImportMode == SpriteImportMode.Single)
            {
                importer.spritePivot = new Vector2(0.5f, 0f); // bottom center
            }
            // Nếu sprite mode = Multiple (sheet nhiều frame)
            else if (importer.spriteImportMode == SpriteImportMode.Multiple)
            {
                var sheet = importer.spritesheet;
                for (int i = 0; i < sheet.Length; i++)
                {
                    sheet[i].alignment = (int)SpriteAlignment.Custom;
                    sheet[i].pivot = new Vector2(0.5f, 0f); // bottom center
                }
                importer.spritesheet = sheet;
            }

            importer.SaveAndReimport();
            Debug.Log($"✅ Set pivot for {tex.name}");
        }
    }
}