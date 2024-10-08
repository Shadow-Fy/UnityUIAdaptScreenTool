using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AdaptUITest : MonoBehaviour
{
    public Vector2 originScreenSize;
    public Vector2 currentScreenSize;
    public Vector2 originUISize;
    private float realRatio;

    private void OnValidate()
    {
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            this.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.size = calCurrentUISize();
    }

    private Vector2 calCurrentUISize()
    {
        Vector2 currentUISize;
        currentUISize.x = getWidthRatio() * currentScreenSize.x;
        realRatio = currentUISize.x / originUISize.x;
        currentUISize.y = realRatio * originUISize.y;
        return currentUISize;
    }

    private float getWidthRatio()
    {
        return originUISize.x / originScreenSize.x;
    }
}