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
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Sprite square = GetWhiteSprite();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateStarfield(square);
        CreateGround(square);
        CreateMovingPlatform(square);
        CreatePlayer(square);
        CreateHud();

        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Plataforma Móvel 2D",
            "Cena criada em " + ScenePath + ".\n\n" +
            "A/D ou setas para andar; Espaço, W ou seta para cima para pular.\n" +
            "Toque na plataforma amarela para iniciar os 5 segundos de movimento.",
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
        camera.backgroundColor = new Color(0.045f, 0.065f, 0.105f);
    }

    private static void CreateStarfield(Sprite sprite)
    {
        Vector3[] positions =
        {
            new Vector3(-7.2f, 3.6f), new Vector3(-5.6f, 2.7f), new Vector3(-3.5f, 4.1f),
            new Vector3(-1.7f, 3.3f), new Vector3(0.5f, 4.2f), new Vector3(2.7f, 3.3f),
            new Vector3(4.5f, 4.0f), new Vector3(6.9f, 2.8f), new Vector3(7.6f, 0.8f),
            new Vector3(-7.6f, 0.4f), new Vector3(-6.1f, -1.0f), new Vector3(-4.9f, 1.3f),
            new Vector3(-2.5f, 2.1f), new Vector3(1.2f, 2.6f), new Vector3(3.6f, 1.2f),
            new Vector3(5.8f, -0.6f), new Vector3(6.5f, -2.0f), new Vector3(-7.1f, -2.4f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            float size = (i % 3 == 0) ? 0.09f : 0.055f;
            Color color = (i % 2 == 0)
                ? new Color(0.35f, 0.62f, 0.82f, 0.7f)
                : new Color(0.65f, 0.76f, 0.9f, 0.55f);
            CreateVisual("Estrela " + (i + 1), null, sprite, positions[i],
                new Vector2(size, size), color, -5);
        }
    }

    private static void CreateGround(Sprite sprite)
    {
        GameObject ground = CreatePhysicsObject(
            "Chão", sprite, new Vector2(0f, -3.72f), new Vector2(22f, 0.95f),
            new Color(0.16f, 0.23f, 0.33f), 0);

        BoxCollider2D collider = ground.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(22f, 0.91f);

        CreateVisual("Faixa superior do chão", ground.transform, sprite,
            new Vector2(0f, 0.45f), new Vector2(22f, 0.09f),
            new Color(0.25f, 0.78f, 0.62f), 1);

        // Pequenos detalhes visuais no piso para dar escala e profundidade.
        for (int i = -9; i <= 9; i++)
        {
            CreateVisual("Marca do chão", ground.transform, sprite,
                new Vector2(i * 1.05f, -0.16f), new Vector2(0.025f, 0.38f),
                new Color(0.23f, 0.31f, 0.42f), 2);
        }
    }

    private static GameObject CreatePlayer(Sprite sprite)
    {
        GameObject player = new GameObject("Jogador");
        player.tag = "Player";
        player.transform.position = new Vector2(-4.15f, -2.765f);

        BoxCollider2D box = player.AddComponent<BoxCollider2D>();
        box.size = new Vector2(0.82f, 0.98f);

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        CreateVisual("Corpo", player.transform, sprite, Vector2.zero,
            new Vector2(0.82f, 0.98f), new Color(0.13f, 0.76f, 0.92f), 3);

        // Olhos brancos e pupilas escuras formam um personagem simples e legível.
        CreateVisual("Olho esquerdo", player.transform, sprite,
            new Vector2(-0.19f, 0.16f), new Vector2(0.19f, 0.24f), Color.white, 4);
        CreateVisual("Olho direito", player.transform, sprite,
            new Vector2(0.19f, 0.16f), new Vector2(0.19f, 0.24f), Color.white, 4);
        CreateVisual("Pupila esquerda", player.transform, sprite,
            new Vector2(-0.16f, 0.15f), new Vector2(0.075f, 0.13f),
            new Color(0.04f, 0.12f, 0.2f), 5);
        CreateVisual("Pupila direita", player.transform, sprite,
            new Vector2(0.22f, 0.15f), new Vector2(0.075f, 0.13f),
            new Color(0.04f, 0.12f, 0.2f), 5);
        CreateVisual("Faixa do personagem", player.transform, sprite,
            new Vector2(0f, -0.28f), new Vector2(0.82f, 0.14f),
            new Color(1f, 0.64f, 0.22f), 5);

        player.AddComponent<PlayerController2D>();
        return player;
    }

    private static GameObject CreateMovingPlatform(Sprite sprite)
    {
        GameObject platform = new GameObject("Plataforma Móvel");
        platform.transform.position = new Vector2(-1.15f, -1.55f);

        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(3.25f, 0.38f);

        Rigidbody2D rb = platform.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Corpo principal, borda inferior e faixa de destaque acompanham o Rigidbody.
        CreateVisual("Base escura", platform.transform, sprite,
            new Vector2(0f, -0.13f), new Vector2(3.38f, 0.22f),
            new Color(0.55f, 0.28f, 0.08f), 1);
        CreateVisual("Superfície amarela", platform.transform, sprite,
            Vector2.zero, new Vector2(3.25f, 0.32f),
            new Color(1f, 0.67f, 0.16f), 2);
        CreateVisual("Faixa de brilho", platform.transform, sprite,
            new Vector2(0f, 0.105f), new Vector2(2.95f, 0.045f),
            new Color(1f, 0.88f, 0.47f), 3);

        for (int i = -1; i <= 1; i += 2)
        {
            CreateVisual("Parafuso", platform.transform,
                sprite, new Vector2(i * 1.42f, -0.015f),
                new Vector2(0.09f, 0.09f), new Color(0.45f, 0.25f, 0.08f), 4);
        }

        TimedMovingPlatform behavior = platform.AddComponent<TimedMovingPlatform>();
        SerializedObject serialized = new SerializedObject(behavior);
        serialized.FindProperty("movementDirection").vector2Value = Vector2.right;
        serialized.FindProperty("horizontalSpeed").floatValue = 1.5f;
        serialized.FindProperty("movementDuration").floatValue = 5f;
        serialized.FindProperty("gravityScaleAfterMovement").floatValue = 1f;
        serialized.FindProperty("playerTag").stringValue = "Player";
        serialized.ApplyModifiedPropertiesWithoutUndo();

        return platform;
    }

    private static void CreateHud()
    {
        GameObject hud = new GameObject("HUD - Instruções");
        hud.AddComponent<PlatformGameHUD>();
    }

    private static GameObject CreatePhysicsObject(
        string objectName, Sprite sprite, Vector2 position, Vector2 size,
        Color color, int sortingOrder)
    {
        GameObject root = new GameObject(objectName);
        root.transform.position = position;
        CreateVisual("Visual", root.transform, sprite, Vector2.zero, size, color, sortingOrder);
        return root;
    }

    private static GameObject CreateVisual(
        string objectName, Transform parent, Sprite sprite, Vector2 localPosition,
        Vector2 size, Color color, int sortingOrder)
    {
        GameObject visual = new GameObject(objectName);
        if (parent != null)
            visual.transform.SetParent(parent, false);
        visual.transform.localPosition = localPosition;
        visual.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
        return visual;
    }

    private static Sprite GetWhiteSprite()
    {
        Directory.CreateDirectory("Assets/Art");

        // O sprite antigo tinha apenas 4x4 pixels e, com PPU 100, virava um pontinho.
        // Agora ele é uma unidade de mundo completa (100x100 px com PPU 100).
        bool needsRebuild = true;
        if (File.Exists(SpritePath))
        {
            byte[] currentBytes = File.ReadAllBytes(SpritePath);
            Texture2D probe = new Texture2D(2, 2);
            if (probe.LoadImage(currentBytes))
                needsRebuild = probe.width != 100 || probe.height != 100;
            Object.DestroyImmediate(probe);
        }

        if (needsRebuild)
        {
            Texture2D texture = new Texture2D(100, 100, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[100 * 100];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(SpritePath, ImportAssetOptions.ForceUpdate);
        }

        TextureImporter importer = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
        if (importer != null)
        {
            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }
            if (!Mathf.Approximately(importer.spritePixelsPerUnit, 100f))
            {
                importer.spritePixelsPerUnit = 100f;
                changed = true;
            }
            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }
            if (changed)
                importer.SaveAndReimport();
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        if (sprite == null)
            throw new System.InvalidOperationException("Não foi possível criar o sprite base em " + SpritePath);
        return sprite;
    }
}
