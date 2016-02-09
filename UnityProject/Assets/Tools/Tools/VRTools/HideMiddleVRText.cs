using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HideMiddleVRText : MonoBehaviour
{
    GUIText[] middleVRGUITexts;

    public KeyCode toggleKey = KeyCode.P;
    public KeyCode modifierToggleKey = KeyCode.None;

	// Use this for initialization
	void Start () 
    {
	    if (VRTools.IsClient())
        {
            GameObject GuiTextGameObject = GameObject.Find("__");
            if (GuiTextGameObject != null)
                middleVRGUITexts = GuiTextGameObject.GetComponents<GUIText>();
            if (middleVRGUITexts != null)
                foreach (GUIText middleVRGUIText in middleVRGUITexts)
                    middleVRGUIText.enabled = false;
        }
	}

    public void Update()
    {
        if (VRTools.GetKeyDown(toggleKey) && (modifierToggleKey == KeyCode.None || VRTools.GetKeyPressed(modifierToggleKey)))
        {
            ToggleGUITexts();
        }

    }

    void ToggleGUITexts()
    {
        if (middleVRGUITexts != null)
            foreach (GUIText middleVRGUIText in middleVRGUITexts)
                middleVRGUIText.enabled = !middleVRGUIText.enabled;
    }

}
