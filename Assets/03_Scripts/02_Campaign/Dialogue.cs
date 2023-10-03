using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    [System.Serializable]
    public class Dialogue
    {
        public string name;
        [TextArea(3, 5)]
        public string[] lines;
    }
}
