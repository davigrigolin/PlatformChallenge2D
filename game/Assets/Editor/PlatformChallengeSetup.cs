using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlatformChallengeSetup
{
    private const string SpritePath = "Assets/Art/WhiteSquare.png";
    private const string ScenePath = "Assets/Scenes/PlatformChallenge.unity";

    [MenuItem("Tools/Plataforma Móvel 2D/Criar ou Redefinir Cena")]
    public static void CreateOrResetScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWants())
            return;

        Sprite whiteSprite = GetWhiteSprite();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateGround(whiteSprite);
        CreatePlayer(whiteSprite);
        CreateMovingPlatform(whiteSprite);

        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Plataforma Móvel 2D",
            "Cena criada em " + ScenePath + ".\n\n" +
            "Use A/D ou as setas para andar e Espaço, W ou Seta para Cima para pular.\n" +
            "Toque na plataforma amarela para iniciar o movimento de 5 segundos.",
            "OK");
    }

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.055f, 0.075f, 0.11f);
    }

    private static void CreateGround(Sprite sprite)
    {
        GameObject ground = CreateBlock(
            "Chão", sprite, new Vector2(0f, -3.45f),
            new Vector2(18f, 0.5f), new Color(0.25f, 0.32f, 0.40f), 0);

        ground.AddComponent<BoxCollider2D>();
    }

    private static void CreatePlayer(Sprite sprite)
    {
        GameObject player = CreateBlock(
            "Jogador", sprite, new Vector2(-3.6f, -2.75f),
            new Vector2(0.75f, 0.75f), new Color(0.10f, 0.80f, 0.95f), 2);

        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2.3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        player.AddComponent<PlayerController2D>();
    }

    private static void CreateMovingPlatform(Sprite sprite)
    {
        GameObject platform = CreateBlock(
            "Plataforma Móvel", sprite, new Vector2(-0.1f, -1.55f),
            new Vector2(2.4f, 0.35f), new Color(1f, 0.68f, 0.12f), 1);

        platform.AddComponent<BoxCollider2D>();

        Rigidbody2D rb = platform.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        TimedMovingPlatform behavior = platform.AddComponent<TimedMovingPlatform>();
        // Os valores ficam visíveis e editáveis no Inspector.
        SerializedObject serializedBehavior = new SerializedObject(behavior);
        serializedBehavior.FindProperty("movementDirection").vector2Value = Vector2.right;
        serializedBehavior.FindProperty("horizontalSpeed").floatValue = 1.5f;
        serializedBehavior.FindProperty("movementDuration").floatValue = 5f;
        serializedBehavior.FindProperty("gravityScaleAfterMovement").floatValue = 1f;
        serializedBehavior.FindProperty("playerTag").stringValue = "Player";
        serializedBehavior.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject CreateBlock(
        string objectName, Sprite sprite, Vector2 position,
        Vector2 size, Color color, int sortingOrder)
    {
        GameObject block = new GameObject(objectName);
        block.transform.position = position;
        block.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        return block;
    }

    private static Sprite GetWhiteSprite()
    {
        Directory.CreateDirectory("Assets/Art");

        if (!File.Exists(SpritePath))
        {
            Texture2D texture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.white;

            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.Refresh();
        }

        TextureImporter importer = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        if (sprite == null)
            throw new System.InvalidOperationException("Não foi possível criar o sprite base em " + SpritePath);

        return sprite;
    }
}
