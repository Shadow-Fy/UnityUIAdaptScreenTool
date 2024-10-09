using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class AdaptUI : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

    private VisualElement labelFromUXML;

    [MenuItem("UI/AdaptUI")]
    public static void ShowExample()
    {
        AdaptUI wnd = GetWindow<AdaptUI>();
        wnd.titleContent = new GUIContent("AdaptUI");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        labelFromUXML = m_VisualTreeAsset.Instantiate();
        labelFromUXML.style.height = 130;
        root.Add(labelFromUXML);

        RegisterInfoFromUXML();
    }

    private void RegisterInfoFromUXML()
    {
        Vector2Field originScreenSize = labelFromUXML.Q<Vector2Field>("OriginScreenSize");
        Vector2Field originUISize = labelFromUXML.Q<Vector2Field>("OriginUISize");
        Vector2Field currentScreenSize = labelFromUXML.Q<Vector2Field>("CurrentScreenSize");
        TextField name = labelFromUXML.Q<TextField>("Name");
        Button createButton = labelFromUXML.Q<Button>("CreateButton");
        ObjectField sprite = labelFromUXML.Q<ObjectField>("Sprite");

        createButton.RegisterCallback<ClickEvent>(evt =>
        {
            CreateNewGameObject(sprite.value as Sprite,
                calCurrentUISize(originScreenSize.value, currentScreenSize.value, originUISize.value), name.value);
        });
    }

    private void CreateNewGameObject(Sprite sprite, Vector2 size, string name)
    {
        Transform parentTransform = Selection.activeTransform;
        Debug.Log(EditorSceneManager.IsPreviewSceneObject(parentTransform));


        GameObject item = new GameObject();
        if (name == "")
        {
            name = "empty";
            Debug.LogWarning("you didn't set GameObject's name");
        }

        item.name = name;
        SpriteRenderer spriteRenderer = item.AddComponent<SpriteRenderer>();
        spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        spriteRenderer.sprite = sprite;
        spriteRenderer.size = size;
        if (parentTransform != null)
        {
            item.transform.SetParent(parentTransform.name != name ? parentTransform : parentTransform.parent);
        }
        else
        {
            Debug.LogWarning("you didn't select a parent GameObject");
        }

        item.transform.localPosition = new Vector3(88,88,88);
    }

    // 计算sprite实际的Size应该为多少
    private Vector2 calCurrentUISize(Vector2 originScreenSize, Vector2 currentScreenSize, Vector2 originUISize)
    {
        Vector2 currentUISize;
        if (originScreenSize.x == 0)
        {
            Debug.LogWarning("originScreenSize.x can not equal zero");
            return new Vector2(0, 0);
        }

        if (originUISize.x == 0)
        {
            Debug.LogWarning("originUISize.x can not equal zero");
            return new Vector2(0, 0);
        }

        currentUISize.x = originUISize.x / originScreenSize.x * currentScreenSize.x;
        currentUISize.y = currentUISize.x / originUISize.x * originUISize.y;
        return currentUISize;
    }
}