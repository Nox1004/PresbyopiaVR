using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : MonoBehaviour {
    
    private SteamVR_TrackedObject trackedObj = null;
    private Color clr;
    private bool ControllerOn;
    public Image _image;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void ActiveController()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start () {
        clr = new Color(0, 1, 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
        if (!ControllerOn && Title.toChange)
        {
            ActiveController();
            ControllerOn = true;
        }
        else if(ControllerOn)
        {
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                _image.color = clr; 
            }
            else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LivingRoomScene");
            }
        }
	}
}
