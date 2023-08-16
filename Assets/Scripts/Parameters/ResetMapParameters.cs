using UnityEngine;

namespace Parameters
{
    public class ResetMapParameters : MonoBehaviour
    {
        public bool IsReset;
        public int IndexPlayer;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

