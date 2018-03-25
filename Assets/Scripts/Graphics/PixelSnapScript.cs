using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSnapScript : MonoBehaviour
{

    const int PPU = 16;

    void LateUpdate()
    {
        float x = (Mathf.Round(transform.parent.position.x * PPU) / PPU) - transform.parent.position.x;
        float y = (Mathf.Round(transform.parent.position.y * PPU) / PPU) - transform.parent.position.y;
        transform.position = new Vector3(x, y, 0);
    }
}
