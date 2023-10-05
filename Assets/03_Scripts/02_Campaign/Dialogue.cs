using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bonkers
{
    [System.Serializable]
    public class Dialogue
    {
        public Sprite icon;
        public string name;
        [TextArea(3, 5)]
        public string[] lines;
    }
}
