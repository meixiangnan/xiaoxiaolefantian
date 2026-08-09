using UnityEngine;

namespace Watermelon
{

    [CreateAssetMenu(fileName = "PU AddTime Settings", menuName = "Content/Power Ups/PU AddTime Settings")]
    public class PUAddTimeSettings : PUCustomSettings
    {
        [LineSpacer("Settings")]
        [SerializeField] int revertElementsCount = 1;
        public int RevertElementsCount => revertElementsCount;

        public override void Initialise()
        {

        }
    }
}
