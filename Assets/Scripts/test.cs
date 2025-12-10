using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class test : MonoBehaviour
{
    public TextAsset wordList;
    public string temp;
    public string[] list;
    public string currentWord;

    // Start is called before the first frame update
    void Start()
    {
        if (wordList != null)
        {
            temp = wordList.text;
        }

        list = temp.Split(new string[] { "\n" }, StringSplitOptions.None);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsWord()
    {
        //172824 is the length of the list

        int low = 0, high = list.Length - 1;
        while (low <= high)
        {
            int mid = low + (high - low) / 2;

            if(string.Equals(list[mid], currentWord))
                return true;

            if(string.Compare(list[mid], currentWord) < 0)
                low = mid + 1;

            // If x is smaller, ignore right half
            else
                high = mid - 1;
        }

        return false;
    }


    //Deprecated
    /*
    public void ClearWord()
    {
        currentWord = "";
    }

    public void AddLetter(string letter)
    {
        currentWord = string.Concat(currentWord, letter);
    }

    public void RemoveLetter()
    {
        currentWord = currentWord.Remove(currentWord.Length - 1);
    }*/
}
