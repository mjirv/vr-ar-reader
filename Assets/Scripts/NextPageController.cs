using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PageController;

public class NextPageController : MonoBehaviour
{

    public PageController pageController;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        pageController.NextPage();
    }
}
