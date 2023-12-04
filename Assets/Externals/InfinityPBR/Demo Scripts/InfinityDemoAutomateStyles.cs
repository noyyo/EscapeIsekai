using UnityEngine;

namespace InfinityPBR.Demo
{
    public class InfinityDemoAutomateStyles : MonoBehaviour
    {
        public InfinityDemoCharacter character;

        public void Setup(InfinityDemoCharacter newCharacter) => character = newCharacter;

        public void ToggleAutomated(bool value) => character.automateStyles = value;
    }

}
