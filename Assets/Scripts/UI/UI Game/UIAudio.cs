using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIAudio : MonoBehaviour
{
    private Button button;
    private ResourceController resourceController;

    void Awake()
    {
        resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();

        button = GetComponent<Button>();
        button.onClick.AddListener(()=> 
        {
            AudioListener audioListener = Camera.main.GetComponent<AudioListener>();
            Image buttonImage = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
            if (audioListener.enabled)
            {
                buttonImage.sprite = resourceController.mute;
            }
            else
            {
                buttonImage.sprite = resourceController.unmute;
            }
            audioListener.enabled = !audioListener.enabled;
        });
    }
}
