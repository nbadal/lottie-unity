using UnityEngine;

namespace Lottie.Vector
{
    public class LottieAnimationFromFile : MonoBehaviour
    {
        public TextAsset lottieAsset;
        private void Awake()
        {
            if (lottieAsset == null) return;
            var anim = LottieParser.Parse(lottieAsset.text);
            var controller = gameObject.AddComponent<LottieVectorController>();
            controller.SetAnimation(anim);
        }
    }
}