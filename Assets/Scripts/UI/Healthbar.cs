using ProjectTrinity.Simulation;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectTrinity.UI
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField]
        private Image healthbarImage;

        [SerializeField]
        private Color fullHealthColor;

        [SerializeField]
        private Color damagedHealthColor;

        public void Initialize(MatchSimulationUnit unitState)
        {
            unitState.HealthPercent
                .Select(healthPercent => Mathf.InverseLerp(0, 100, healthPercent))
                .Subscribe(UpdateHealthFill)
                .AddTo(this);
        }

        public void UpdateHealthFill(float fillamount)
        {
            healthbarImage.fillAmount = fillamount;
            healthbarImage.color = fillamount < 1f ? damagedHealthColor : fullHealthColor;
        }
    }
}