// Assets/Editor/GenerateSkyboxCubemap.cs
using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateSkyboxCubemap : MonoBehaviour
{
    // Parámetros ajustables
    const int cubemapSize = 1024; // 512 o 1024
    static Color horizonColor = new Color(0.8f, 0.9f, 1.0f); // cerca del horizonte
    static Color zenithColor  = new Color(0.05f, 0.15f, 0.35f); // arriba
    static Color sunColor     = new Color(1.0f, 0.95f, 0.8f);
    static Vector3 sunDirection = new Vector3(0.3f, 0.7f, 0.5f).normalized;
    static float sunSize = 0.02f; // angular size
    static float sunHaloSize = 0.06f;
    static float starIntensity = 1.0f;
    static int starOctaves = 4;
    static float starScale = 30.0f;

    [MenuItem("Tools/Generar Skybox Procedural")]
    public static void Generate()
    {
        // Crear cubemap
        var cubemap = new Cubemap(cubemapSize, TextureFormat.RGBA32, false);
        cubemap.name = "ProceduralSkyboxCubemap";

        for (int face = 0; face < 6; face++)
        {
            CubemapFace cmFace = (CubemapFace)face;
            Color[] colors = new Color[cubemapSize * cubemapSize];

            for (int y = 0; y < cubemapSize; y++)
            {
                for (int x = 0; x < cubemapSize; x++)
                {
                    // uv en [-1,1]
                    float u = (x + 0.5f) / cubemapSize * 2f - 1f;
                    float v = (y + 0.5f) / cubemapSize * 2f - 1f;

                    Vector3 dir = TexelCoordToDirection(cmFace, u, v);
                    dir.Normalize();

                    // Degradado por componente y (elevación)
                    float t = Mathf.Clamp01(dir.y * 0.5f + 0.5f); // 0 horizon, 1 zenit
                    Color baseColor = Color.Lerp(horizonColor, zenithColor, Mathf.Pow(t, 1.3f));

                    // Sol: brillo según ángulo con sunDirection
                    float cosAngle = Vector3.Dot(dir, sunDirection);
                    float sunDisk = SmoothStep(1f - sunSize, 1f - sunSize * 0.5f, cosAngle);
                    float sunHalo = SmoothStep(1f - sunHaloSize, 1f - sunSize, cosAngle);
                    Color col = baseColor;
                    col += sunColor * Mathf.Clamp01(sunHalo) * 0.8f;
                    col += sunColor * Mathf.Clamp01(sunDisk) * 2.0f; // núcleo más brillante

                    // Estrellas: solo en la parte nocturna (cuando dir.y < threshold)
                    if (dir.y < 0.2f)
                    {
                        float stars = 0f;
                        float frequency = starScale;
                        float amp = 1f;
                        for (int o = 0; o < starOctaves; o++)
                        {
                            float n = Perlin3D(dir * frequency + Vector3.one * 12.34f * (o+1));
                            stars += (n * 0.5f + 0.5f) * amp;
                            frequency *= 2f;
                            amp *= 0.5f;
                        }
                        // Intensificar puntos (hacerlas puntuales)
                        stars = Mathf.Pow(Mathf.Clamp01(stars * 1.5f), 6f) * starIntensity;
                        col += new Color(stars, stars, stars, 0f);
                    }

                    // Tonalidad final y clamp
                    col.r = Mathf.Clamp01(col.r);
                    col.g = Mathf.Clamp01(col.g);
                    col.b = Mathf.Clamp01(col.b);
                    col.a = 1f;

                    colors[y * cubemapSize + x] = col;
                }
            }

            cubemap.SetPixels(colors, cmFace);
        }

        cubemap.Apply();

        // Guardar asset
        string folder = "Assets/GeneratedSkyboxes";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        string path = $"{folder}/{cubemap.name}.cubemap";
        AssetDatabase.CreateAsset(cubemap, path);

        // Crear material skybox
        var mat = new Material(Shader.Find("Skybox/Cubemap"));
        mat.name = "ProceduralSkybox_Mat";
        mat.SetTexture("_Tex", cubemap);

        string matPath = $"{folder}/{mat.name}.mat";
        AssetDatabase.CreateAsset(mat, matPath);

        // Forzar refresh y asignar a escena (RenderSettings)
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Asignar skybox
        RenderSettings.skybox = mat;
        Debug.Log($"Skybox procedural generado y asignado: {matPath}");
    }

    // — Utilidades —

    static float SmoothStep(float edge0, float edge1, float x)
    {
        float t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
        return t * t * (3f - 2f * t);
    }

    // Convierte coordenadas de texel (u,v) del face a un direction vector en espacio local del cubemap
    static Vector3 TexelCoordToDirection(CubemapFace face, float u, float v)
    {
        // u,v en [-1,1]. Según especificación de faces
        switch (face)
        {
            case CubemapFace.PositiveX: return new Vector3(1f, -v, -u);
            case CubemapFace.NegativeX: return new Vector3(-1f, -v, u);
            case CubemapFace.PositiveY: return new Vector3(u, 1f, v);
            case CubemapFace.NegativeY: return new Vector3(u, -1f, -v);
            case CubemapFace.PositiveZ: return new Vector3(u, -v, 1f);
            case CubemapFace.NegativeZ: return new Vector3(-u, -v, -1f);
        }
        return Vector3.forward;
    }

    // Perlin 3D (composición simple usando 2D Perlin)
    static float Perlin3D(Vector3 p)
    {
        float ab = Mathf.PerlinNoise(p.x, p.y);
        float bc = Mathf.PerlinNoise(p.y, p.z);
        float ac = Mathf.PerlinNoise(p.x, p.z);
        float ba = Mathf.PerlinNoise(p.y, p.x);
        float cb = Mathf.PerlinNoise(p.z, p.y);
        float ca = Mathf.PerlinNoise(p.z, p.x);
        return (ab + bc + ac + ba + cb + ca) / 6f;
    }
}
