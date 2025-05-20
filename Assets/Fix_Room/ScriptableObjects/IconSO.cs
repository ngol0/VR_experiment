using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconData", menuName = "ScriptableObjects/Icon", order = 1)]
public class ImageSO : ScriptableObject
{
    public string iconName;
    public int index;
    public Sprite image;
}
