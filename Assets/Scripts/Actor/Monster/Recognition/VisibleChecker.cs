using UnityEngine;

namespace Apis
{
    public class VisibleChecker : MonoBehaviour
    {
        [SerializeField] private IVisible _visible;

        private void OnBecameInvisible()
        {
            _visible.IsInVisible = false;
        }

        private void OnBecameVisible()
        {
            _visible.IsInVisible = true;
        }
    }
}