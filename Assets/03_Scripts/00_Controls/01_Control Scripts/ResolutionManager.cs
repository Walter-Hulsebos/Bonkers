using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Bonkers
{
    public class ResolutionManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropDown;
        [SerializeField] private TMP_Dropdown displayDropDown;
        [SerializeField] private TMP_Dropdown qualityDropdown;

        private List<Resolution> fileteredResolutions;
        private Resolution[] resolutions;

        private float currentRefreshRate;
        private int currentResolutionIndex = 0;

        
        [System.Obsolete]
        void Start()
        {
            PopulateDropdown();
            qualityDropdown.onValueChanged.AddListener(OnQualityDropdownValueChanged);

            AddOptions();
            displayDropDown.onValueChanged.AddListener(OnDropdownValueChanged);

            resolutions = Screen.resolutions;
            fileteredResolutions = new List<Resolution>();

            resolutionDropDown.ClearOptions();
            currentRefreshRate = Screen.currentResolution.refreshRate;

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].refreshRate == currentRefreshRate)
                {
                    fileteredResolutions.Add(resolutions[i]);
                }
            }

            List<string> options = new List<string>();
            for (int i = 0; i < fileteredResolutions.Count; i++)
            {
                string resolutionsOption = fileteredResolutions[i].width + "x" + fileteredResolutions[i].height + " " + fileteredResolutions[i].refreshRate + " Hz ";
                options.Add(resolutionsOption);

                if (fileteredResolutions[i].width == Screen.width && fileteredResolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropDown.AddOptions(options);
            resolutionDropDown.value = currentResolutionIndex;
            resolutionDropDown.RefreshShownValue();
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = fileteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, true);
        }

        void AddOptions()
        {
            // Clear existing options
            displayDropDown.ClearOptions();

            // Add new options
            displayDropDown.AddOptions(new List<string>
        {
            "FullScreen",
            "Windowed",
           
        });
        }

        void OnDropdownValueChanged(int index)
        {
            // The function to be executed when the dropdown value changes
            switch (index)
            {
                case 0:
                    Option1Function();
                    break;
                case 1:
                    Option2Function();
                    break;
               
                default:
                    Debug.LogError("Unhandled option index");
                    break;
            }
        }

        void Option1Function()
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }

        void Option2Function()
        {
            Screen.SetResolution(800, 600, false);
        }

        void PopulateDropdown()
        {
            // Clear existing options
            qualityDropdown.ClearOptions();

            List<TMP_Dropdown.OptionData> qualityoptions = new List<TMP_Dropdown.OptionData>();
            foreach (var name in QualitySettings.names)
            {
                qualityoptions.Add(new TMP_Dropdown.OptionData(name));
            }
            qualityDropdown.AddOptions(qualityoptions);
        }

        // Function to handle TMPro dropdown value changes
        void OnQualityDropdownValueChanged(int qualityindex)
        {
            // Set the quality level based on the selected index
            QualitySettings.SetQualityLevel(qualityindex, true);
            Debug.Log("Quality level set to: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        }


    }
}
