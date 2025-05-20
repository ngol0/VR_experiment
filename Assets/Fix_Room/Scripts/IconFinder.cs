using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class IconFinder : MonoBehaviour
{
    public IconGroupSO groupIconData;

    List<int> indexList;

    const int GROUP_SIZE = 9;
    const int MAX_RANDOM = 10;
    const int ICON_SIZE = 27;
    int targetGroupIndex = 0;
    int targetImageIndex = 0;
    int currentIndex = -1; //index for picking the target image

    public int TargetIndex => targetImageIndex;
    public int CurrentIndex => currentIndex;

    List<ImageSO> randomImages = new List<ImageSO>();

    [Header("Random Image Configuration")]
    [SerializeField][Range(0, 10)] int numberOfSameGroupImages = 4;

    void Start()
    {
        indexList = Enumerable.Range(0, 27).OrderBy(_ => UnityEngine.Random.value).ToList();
    }

    //------------------Function for picking random icons:--------------------
    //------------------------------------------------------------------------
    // Details: Adjust slider in Unity Editor to set the image from same group
    // if image from same group is 10, no image is taken from other group
    public List<ImageSO> ChooseRandomImg()
    {
        randomImages.Clear();

        // --- Get indexes in the target group
        // Pick 4 random others from the same group
        List<int> otherInGroupIndexes = ChooseRandomIndexFromSameGroup(numberOfSameGroupImages);

        // --- Combine all selected indexes
        List<int> pickIndexes = new List<int>
        {
            targetImageIndex // Add actual target first
        };
        pickIndexes.AddRange(otherInGroupIndexes);

        if (numberOfSameGroupImages < MAX_RANDOM)
        {
            //Debug.Log("Picking from other group");
            // --- Get indexes in other groups
            List<int> otherGroupIndexes = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (i == targetGroupIndex) continue;
                otherGroupIndexes.AddRange(Enumerable.Range(i * GROUP_SIZE, GROUP_SIZE));
            }

            List<int> otherGroupChoices = otherGroupIndexes.OrderBy(_ => UnityEngine.Random.value).Take(5).ToList();
            pickIndexes.AddRange(otherGroupChoices);
        }

        // Shuffle the pickable indexes
        pickIndexes = pickIndexes.OrderBy(_ => UnityEngine.Random.value).ToList();

        // Get the sprites
        foreach (int idx in pickIndexes)
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

    public List<int> ChooseRandomIndexFromSameGroup(int numberOfImages)
    {
        randomImages.Clear();

        List<int> sameGroupIndexes = Enumerable.Range(targetGroupIndex * GROUP_SIZE, GROUP_SIZE).ToList();
        sameGroupIndexes.Remove(targetImageIndex); // remove the actual target
        List<int> otherInGroupIndexes = new List<int>();

        if (numberOfImages < MAX_RANDOM)
        // Pick random others from the same group
        {
            otherInGroupIndexes = sameGroupIndexes.OrderBy(
                _ => UnityEngine.Random.value).Take(numberOfImages - 1).ToList();
        }
        else
        {
            for (int i = 0; i < MAX_RANDOM - 1; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, sameGroupIndexes.Count);
                otherInGroupIndexes.Add(sameGroupIndexes[randomIndex]);
            }
        }

        return otherInGroupIndexes;
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
    }
}
