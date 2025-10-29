using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MouseObjectPlacerUI : MonoBehaviour
{
    [Header("Prefabs disponibles para colocar")]
    public GameObject[] prefabs;

    [Header("Iconos para cada prefab (en el mismo orden)")]
    public Sprite[] iconos;

    [Header("Configuración de interfaz")]
    public Transform contenedorBotones; // Panel con HorizontalLayoutGroup o ScrollRect
    public GameObject botonPlantilla;

    [Header("Configuración de colocación")]
    public float altura = 0.02f;
    public Camera camara;
    public GameObject cube;

    private GameObject prefabSeleccionado;
    private GameObject previewActual;
    private bool colocando = false;


    void Start()
    {
        if (camara == null)
            camara = Camera.main;

        GenerarBotones();
    }

    void Update()
    {
        if (colocando)
        {
            ActualizarPreview();
            if (Input.GetMouseButtonDown(0))
                IntentarColocar();
        }
    }

    void GenerarBotones()
{
    // Limpia los botones anteriores
    foreach (Transform hijo in contenedorBotones)
        Destroy(hijo.gameObject);

    for (int i = 0; i < prefabs.Length; i++)
    {
        // Instanciar el botón dentro del contenedor
        GameObject boton = Instantiate(botonPlantilla, contenedorBotones);

        // 🔧 Asegurar posición y escala correctas
        RectTransform rt = boton.GetComponent<RectTransform>();
        rt.anchoredPosition3D = Vector3.zero;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        // Asignar ícono si existe
        Image img = boton.GetComponentInChildren<Image>();
        if (img != null && i < iconos.Length && iconos[i] != null)
            img.sprite = iconos[i];

        // Asignar evento al botón
        int index = i;
        boton.GetComponent<Button>().onClick.AddListener(() => SeleccionarPrefab(index));

        // Asegurar que esté visible
        boton.SetActive(true);
    }
}


    void SeleccionarPrefab(int index)
    {
        if (previewActual) Destroy(previewActual);

        prefabSeleccionado = prefabs[index];
        previewActual = Instantiate(prefabSeleccionado);
        SetPreviewMaterial(previewActual);
        colocando = true;
        Debug.Log("Seleccionado: " + prefabSeleccionado.name);
    }

    void ActualizarPreview()
    {
        Ray ray = camara.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            previewActual.SetActive(true);

            Vector3 posicion = hit.point + hit.normal * altura;
            previewActual.transform.position = posicion;
            previewActual.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            previewActual.SetActive(false);
        }
    }

    void IntentarColocar()
    {
        if (!previewActual.activeSelf) return;

        GameObject objetoNuevo = Instantiate(prefabSeleccionado, previewActual.transform.position, previewActual.transform.rotation);
        objetoNuevo.transform.SetParent(cube.transform);
        Destroy(previewActual);
        previewActual = null;
        colocando = false;
    }

    void SetPreviewMaterial(GameObject obj)
    {
        
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in r.materials)
            {
                Color c = mat.color;
                c.a = 0.5f;
                mat.color = c;
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                
            }
        }
    }
}
