using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{

    public static bool toChange;

    public Image image;

    public GameObject menu;

    // Use this for initialization
    void Start()
    {
        toChange = false;

        image.gameObject.SetActive(true);

        menu.gameObject.SetActive(false);

        StartCoroutine(ScreenConvert());

    }
    private IEnumerator ScreenConvert()
    {
        bool isFadeIn = false;

        Color clr = new Color(1, 1, 1, 0);

        image.color = clr;

        while (true)
        {
            if (!isFadeIn)
            {
                clr.a += Time.deltaTime / 3;
                image.color = clr;
                if (clr.a >= 1.0f)
                    isFadeIn = !isFadeIn;
            }
            else
            {
                clr.a -= Time.deltaTime / 3;
                image.color = clr;
                if (clr.a <= 0.0f)
                {
                    image.gameObject.SetActive(false);
                    menu.gameObject.SetActive(true);
                    toChange = true;
                    break;
                }
            }
            yield return null;
        }
    }
}
