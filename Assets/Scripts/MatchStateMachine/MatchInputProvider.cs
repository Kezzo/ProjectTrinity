using ProjectTrinity.Helper;
using UnityEngine;

namespace ProjectTrinity.MatchStateMachine
{
    public class MatchInputProvider
    {
        public float XTranslation { get; private set; }
        public float YTranslation { get; private set; }
        public float Rotation { get; private set; }
        public bool InputReceived { get; private set; }

        public void AddXTranslation(float xTranslation)
        {
            this.XTranslation = Mathf.Clamp(xTranslation, -1f, 1f);
            InputReceived = true;
        }

        public void AddYTranslation(float yTranslation)
        {
            this.YTranslation = Mathf.Clamp(yTranslation, -1f, 1f);
            InputReceived = true;
        }

        public void AddRotation(float rotation)
        {
            this.Rotation = rotation;
            InputReceived = true;
        }

        public byte GetSimulationXTranslation()
        {
            return (byte) Mathf.Lerp(0, 255, Mathf.InverseLerp(-1, 1f, XTranslation));
        }

        public byte GetSimulationYTranslation()
        {
            return (byte)Mathf.Lerp(0, 255, Mathf.InverseLerp(-1, 1f, YTranslation));
        }

        public byte GetSimulationRotation()
        {
            return (byte)Mathf.Lerp(0, 255, Mathf.InverseLerp(0, 360f, MathHelper.Modulo(Rotation, 360)));
        }

        // should be called in every simulation frame so inputs in next update can be registered.
        public void Reset()
        {
            XTranslation = 0f;
            YTranslation = 0f;
            Rotation = 0f;
            InputReceived = false;
        }
    }
}