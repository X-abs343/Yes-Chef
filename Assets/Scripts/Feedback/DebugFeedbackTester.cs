using UnityEngine;
using YesChef.Core;

namespace YesChef.Feedback
{
    public class DebugFeedbackTester : MonoBehaviour
    {
        [SerializeField] private PlayerVisualFeedback visualFeedback;

        private void Start()
        {
            if (visualFeedback == null)
            {
                visualFeedback = GetComponent<PlayerVisualFeedback>();
            }

            if (InputReader.Instance != null)
            {
                InputReader.Instance.OnDebugCelebration += () => visualFeedback?.PlayCelebrationJump();
                InputReader.Instance.OnDebugSadTilt += () => visualFeedback?.PlaySadTilt();
                InputReader.Instance.OnDebugStopSquash += () => visualFeedback?.PlayStopSquashManual();
            }
        }
    }
}