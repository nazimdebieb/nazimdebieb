using UnityEngine;

namespace BloomGarden.Presentation
{
    /// <summary>Collider marker on the head can of a lane. Only heads carry one, so only heads can be tapped.</summary>
    public sealed class LaneTapTarget : MonoBehaviour
    {
        public int lane;
    }
}
