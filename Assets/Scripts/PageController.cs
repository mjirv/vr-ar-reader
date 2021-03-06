﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpubSharp;
// using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEditor;
using System.Net;
using System.IO;

public class PageController : MonoBehaviour
{
    public Camera camera;

    private string bookPath;
    private string bookText;
    private LinkedListNode<string> curText;

    public TextMeshPro pageOne;
    public TextMeshPro pageTwo;

    private int startPos, endPos;

    private LinkedList<string> chapterTexts;
    private EpubBook book;

    // Use this for initialization
    void Start()
    {
        using (var client = new WebClient())
        {
            //TODO: Next step - allow the user to choose what book to read (input url? search? browse?)
            Stream stream = client.OpenRead("http://www.gutenberg.org/ebooks/135.epub.noimages");
            book = EpubReader.Read(stream, false);
        }

        chapterTexts = new LinkedList<string>();

        foreach (EpubTextFile chapter in book.SpecialResources.HtmlInReadingOrder)
        {
            string newText = chapter.TextContent + '\n';
            newText = Regex.Replace(newText, "<br[^>]*>", "\n");
            newText = Regex.Replace(newText, "<p[^>]*>", "\n");
            newText = Regex.Replace(newText, "</p>", "");

            newText = Regex.Replace(newText, "<h5[^>]*>", "<size=+1>");
            newText = Regex.Replace(newText, "</h5>", "</size>");
            newText = Regex.Replace(newText, "<h4[^>]*>", "<size=+2>");
            newText = Regex.Replace(newText, "</h4>", "</size>");
            newText = Regex.Replace(newText, "<h3[^>]*>", "<size=+3>");
            newText = Regex.Replace(newText, "</h3>", "</size>");
            newText = Regex.Replace(newText, "<h2[^>]*>", "<size=+4>");
            newText = Regex.Replace(newText, "</h2>", "</size>");
            newText = Regex.Replace(newText, "<h1[^>]*>", "<size=+5>");
            newText = Regex.Replace(newText, "</h1>", "</size>");
            newText = Regex.Replace(newText, "<div[^>]*>", "\n");
            newText = Regex.Replace(newText, "</div>", "\n");

            // Replace anything not matched above, or italics/bold/underline, with blank
            newText = Regex.Replace(newText, "</?(((p|size|i|br|u|b)[^ r=>][^>]*)|((?!(p|br|size|i|b|u|/))[^>]*))>", "");

            chapterTexts.AddLast(new LinkedListNode<string>(newText));
        }

        curText = chapterTexts.First;

        pageOne.text = curText.Value;
        pageTwo.text = curText.Value;

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    NextPage();
        //}
        //if (Input.GetButtonDown("Fire2"))
        //{
        //    PrevPage();
        //}

    }

    public void NextPage()
    {
        pageOne.pageToDisplay += 2;
        pageTwo.pageToDisplay += 2;

        if (pageTwo.pageToDisplay > pageTwo.textInfo.pageCount)
        {
            // Make blank if pageOne is the last page in the chapter
            pageTwo.text = "";
        }

        if (pageOne.pageToDisplay > pageOne.textInfo.pageCount)
        {
            // Next chapter
            if (curText.Next != null)
            {
                PrepareUpdateText();
                UpdateText(curText.Next);

                pageOne.pageToDisplay = 1;
                pageTwo.pageToDisplay = 2;
            }
        }
    }

    public void PrevPage()
    {
        pageOne.pageToDisplay -= 2;
        pageTwo.pageToDisplay -= 2;

        // Make sure they can't go negative, as this can make it hard to go forward again
        if (pageOne.pageToDisplay < 1)
        {
            pageOne.pageToDisplay = 1;
        }

        if (pageTwo.pageToDisplay < 2)
        {
            pageTwo.pageToDisplay = 2;
        }

        if (pageOne.pageToDisplay < 1)
        {
            // Previous chapter
            if (curText.Previous != null)
            {
                PrepareUpdateText();
                UpdateText(curText.Previous);

                camera.Render();

                pageOne.pageToDisplay = pageOne.textInfo.pageCount - 1;
                pageTwo.pageToDisplay = pageOne.textInfo.pageCount;

            }
        }
    }

    void PrepareUpdateText()
    {
        // TODO: figure out why this doesn't work
        pageOne.text = "Loading...";
        pageTwo.text = "...";

        pageOne.pageToDisplay = 1;
        pageTwo.pageToDisplay = 1;

        camera.Render();
    }

    void UpdateText(LinkedListNode<string> newText)
    {
        curText = newText;
        pageOne.text = curText.Value;
        pageTwo.text = curText.Value;
    }

}