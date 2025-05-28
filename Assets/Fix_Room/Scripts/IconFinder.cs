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

        // Get all indexes from the same group except the target
        List<int> sameGroupIndexes = Enumerable.Range(targetGroupIndex * GROUP_SIZE, GROUP_SIZE)
            .Where(i => i != targetImageIndex)
            .ToList();

        // Check if we have enough images to fill the requirements
        int uniqueImagesAvailable = sameGroupIndexes.Count;
        int maxPossibleSelections = uniqueImagesAvailable * 3; // Each can appear up to 3 times

        if (maxPossibleSelections < MAX_RANDOM - 1) // -1 because we always include the target once
        {
            Debug.LogError($"Not enough images to fill {MAX_RANDOM} slots. " +
                          $"Available unique images: {uniqueImagesAvailable}, " +
                          $"Max possible selections: {maxPossibleSelections + 1} (including target)");
            return randomImages;
        }

        // Create a pool where each index can appear up to 3 times
        List<int> selectionPool = new List<int>();
        foreach (int index in sameGroupIndexes)
        {
            for (int i = 0; i < 3; i++)
            {
                selectionPool.Add(index);
            }
        }

        // Shuffle the pool using a single random instance
        System.Random rng = new System.Random();
        selectionPool = selectionPool.OrderBy(_ => rng.Next()).ToList();

        // Take the required number of images (excluding the target which we'll add separately)
        List<int> pickedIndexes = selectionPool.Take(MAX_RANDOM - 1).ToList();

        // Add the target image exactly once
        pickedIndexes.Add(targetImageIndex);

        // Final shuffle to ensure target isn't always in the same position
        pickedIndexes = pickedIndexes.OrderBy(_ => rng.Next()).ToList();

        // Convert indexes to ImageSO objects
        foreach (int idx in pickedIndexes)
        {
            int group = idx / GROUP_SIZE;
            ImageSO sprite = FindSpriteByGroupAndIndex(group, idx);

            if (sprite != null)
            {
                randomImages.Add(sprite);
            }
            else
            {
                Debug.LogWarning($"Could not find sprite for group {group}, index {idx}");
            }
        }

        // Verify we got the expected number of images
        if (randomImages.Count != MAX_RANDOM)
        {
            Debug.LogWarning($"Expected {MAX_RANDOM} images but got {randomImages.Count}");
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
        return currentIndex >= ICON_SIZE - 1;
        //return currentIndex > 0; //for testing only
    }
}
