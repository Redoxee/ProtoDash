namespace Dasher
{
    public class DirtyCanvasRescaler : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField]
        private UnityEngine.UI.CanvasScaler canvasScaler = null;

        [UnityEngine.SerializeField]
        ScalerConfiguration wideConfiguration;

        private void Start()
        {
            if (UnityEngine.Screen.width < UnityEngine.Screen.height)
            {
                return;
            }

            this.ApplyConfiguration(this.wideConfiguration);
        }

        private void ApplyConfiguration(ScalerConfiguration configuration)
        {
            this.canvasScaler.matchWidthOrHeight = configuration.MatchWidthOrHeight;
            this.canvasScaler.referenceResolution = configuration.ReferenceResolution;
        }

        [System.Serializable]
        public struct ScalerConfiguration
        {
            public float MatchWidthOrHeight;

            public UnityEngine.Vector2 ReferenceResolution;
        }
    }
}
