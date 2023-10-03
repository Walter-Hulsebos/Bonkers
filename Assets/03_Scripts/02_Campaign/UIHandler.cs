using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Bonkers
{
    public class UIHandler : CampaignSubManager
    {
        public void UpdateTextUI(TextMeshProUGUI textObject, string message, float fontSize)
        {
            textObject.text = message;
            textObject.fontSize = fontSize;
        }

        public void UpdateSlider(Slider slider, float value)
        {
            slider.value = value;
        }

        public void ToggleTextElement(TextMeshProUGUI text, bool value)
        {
            text.gameObject.SetActive(value);
        }


        public void TogglePanel(GameObject panel, bool value)
        {
            panel.SetActive(value);
        }
    }



}
