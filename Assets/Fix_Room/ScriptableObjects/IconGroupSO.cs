using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconGroupData", menuName = "ScriptableObjects/IconGroup", order = 1)]
public class IconGroupSO : ScriptableObject
{
    [System.Serializable]
    public class ImageGroup
    {
        public string groupName;
        public List<ImageSO> images;
    }

    public List<ImageGroup> groups = new List<ImageGroup>(3); // 3 groups
}
