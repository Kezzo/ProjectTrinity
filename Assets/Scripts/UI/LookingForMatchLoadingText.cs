using System.Collections;
using TMPro;
using UnityEngine;

namespace ProjectTrinity.UI
{
    public class LookingForMatchLoadingText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI lookingForMatchText;

        private YieldInstruction wait = new WaitForSeconds(0.5f);

        private void Start()
        {
            StartCoroutine(TextChangeCoroutine());
        }

        private IEnumerator TextChangeCoroutine()
        {
            while (true)
            {
                lookingForMatchText.text = "Looking for Match";

                yield return wait;

                lookingForMatchText.text = "Looking for Match.";

                yield return wait;

                lookingForMatchText.text = "Looking for Match..";

                yield return wait;

                lookingForMatchText.text = "Looking for Match...";

                yield return wait;
            }

        }
    }
}