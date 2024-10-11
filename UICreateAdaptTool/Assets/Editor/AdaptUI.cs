using System;
using System.Text.RegularExpressions;
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
        TextField originUICSS = labelFromUXML.Q<TextField>("OriginUICSS");
        Button createButton = labelFromUXML.Q<Button>("CreateButton");
        ObjectField spriteField = labelFromUXML.Q<ObjectField>("Sprite");
        originScreenSize.value = new Vector2(393, 852);
        currentScreenSize.value = new Vector2(720, 1280);
        name.value = "<---empty--->";


        createButton.RegisterCallback<ClickEvent>(evt =>
        {
            Vector2 originUIVector2;
            string css = originUICSS.value;
            if (css != "")
            {
                int width = 1, height = 1;
                Regex regex = new Regex(@"(?<=width:\s)\d+(?=px)");
                Match widthMatch = regex.Match(css);
                if (widthMatch.Success)
                {
                    width = Int32.Parse(widthMatch.Value);
                }

                regex = new Regex(@"(?<=height:\s)\d+(?=px)");
                Match heightMatch = regex.Match(css);
                if (heightMatch.Success)
                {
                    height = Int32.Parse(heightMatch.Value);
                }

                originUIVector2 = new Vector2(width, height);
            }
            else
            {
                originUIVector2 = originUISize.value;
            }

            CreateNewGameObject(spriteField.value as Sprite, 
                calCurrentUISize(originScreenSize.value, currentScreenSize.value, originUIVector2), name.value);
        });
    }

    private void CreateNewGameObject(Sprite sprite, Vector2 size, string name)
    {
        Transform parentTransform = Selection.activeTransform;

        GameObject item = new GameObject();
        if (name == "")
        {
            name = "<---empty--->";
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
        // 在预制体中用代码生成的GameObject不会被unity判断为修改（导致无法保存），所以刻意将位置设置为一个其他数值，需要手动调整位置来使得unity自动保存修改
        item.transform.localPosition = new Vector3(88, 88, 88);
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