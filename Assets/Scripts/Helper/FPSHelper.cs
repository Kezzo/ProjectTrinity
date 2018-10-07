using System.Collections;
using TMPro;
using UnityEngine;

namespace ProjectTrinity.Helper
{
    public class FPSHelper : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_fpsCounterText;

        private int m_framesPerSecond;

        void Start()
        {
            StartCoroutine(DisplayFps());
        }

        private IEnumerator DisplayFps()
        {
            while (true)
            {
                m_framesPerSecond = (int)(1.0f / Time.smoothDeltaTime);
                m_fpsCounterText.text = string.Format("FPS: {0}", m_framesPerSecond);

                m_fpsCounterText.color = m_framesPerSecond < 20 ? Color.red : Color.white;

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}