using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class IconFinder : MonoBehaviour
{
    public IconGroupSO groupIconData;

    List<int> indexList;

    // const var
    int GROUP_SIZE;
    int NUMBER_OF_GROUP;
    public const int MAX_RANDOM = 10;
    int ICON_SIZE;

    // -- 
    int targetGroupIndex = 0;
    int targetImageIndex = 0;
    int currentIndex = -1; //index for picking the target image

    public int CurrentIndex => currentIndex;

    List<ImageSO> randomImages = new List<ImageSO>();

    void Start()
    {
        GROUP_SIZE = groupIconData.groups[0].images.Count;
        NUMBER_OF_GROUP = groupIconData.groups.Count;
        ICON_SIZE = GROUP_SIZE * NUMBER_OF_GROUP;
        Debug.Log("Icon Size: " + ICON_SIZE.ToString() + ", Group Size: " + GROUP_SIZE.ToString() + ", Number of Groups: " + NUMBER_OF_GROUP.ToString());

        indexList = Enumerable.Range(0, ICON_SIZE).OrderBy(_ => UnityEngine.Random.value).ToList();
    }

    //------------------Function for picking random icons:--------------------
    //------------------------------------------------------------------------
    // Details: 

    public List<ImageSO> ChooseRandomImageFromSameGroup()
    {
        randomImages.Clear();

        List<int> sameGroupIndexes = Enumerable.Range(targetGroupIndex * GROUP_SIZE, GROUP_SIZE).ToList();
        sameGroupIndexes.Remove(targetImageIndex); // remove the actual target

        // Choose the 9 random images:
        Dictionary<int, int> iconCount = new Dictionary<int, int>();
        List<int> pickedIndexes = new List<int>
        {
            // Add target icon once
            targetImageIndex
        };
        iconCount[targetImageIndex] = 1;

        System.Random rng = new System.Random();
        // Fill remaining images
        while (pickedIndexes.Count < MAX_RANDOM)
        {
            int randomIcon = sameGroupIndexes[rng.Next(sameGroupIndexes.Count)];

            if (!iconCount.ContainsKey(randomIcon))
                iconCount[randomIcon] = 0;

            // Add only if not over 3 times
            if (iconCount[randomIcon] < 3)
            {
                pickedIndexes.Add(randomIcon);
                iconCount[randomIcon]++;
            }

            // Break safety net (in case of infinite loop due to logic error or very small group)
            if (pickedIndexes.Count > 100)
            {
                Debug.LogWarning("Pick limit exceeded, check logic.");
                break;
            }
        }
        // Shuffle so target isn't always first
        pickedIndexes = pickedIndexes.OrderBy(_ => UnityEngine.Random.value).ToList();

        // Get the sprites
        foreach (int idx in pickedIndexes)
        {
            int group = idx / GROUP_SIZE;
            ImageSO s = FindSpriteByGroupAndIndex(group, idx);
            if (s != null)
            {
                randomImages.Add(s);
            }
        }

        return randomImages;
    }

    public ImageSO ChooseTargetImg()
    {
        currentIndex++;

        targetImageIndex = indexList[currentIndex];
        targetGroupIndex = targetImageIndex / GROUP_SIZE;
        //Debug.Log("Target Image Index: " + indexList[currentIndex].ToString());
        //Debug.Log("Group Index: " + targetGroupIndex.ToString());

        ImageSO targetSprite = FindSpriteByGroupAndIndex(targetGroupIndex, targetImageIndex);
        return targetSprite;
    }

    public ImageSO FindSpriteByGroupAndIndex(int groupIndex, int targetIndex)
    {
        // Safety check
        if (groupIndex < 0 || groupIndex >= groupIconData.groups.Count)
            return null;

        var group = groupIconData.groups[groupIndex];

        foreach (var image in group.images)
        {
            if (image.index == targetIndex)
            {
                return image;
            }
        }

        return null; // Not found in the specified group
    }

    public bool IsFinish()
    {
        //return currentIndex >= ICON_SIZE - 1;
        return currentIndex > 0; //for testing only
    }
}
