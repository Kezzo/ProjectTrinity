using ProjectTrinity.Helper;
using UnityEngine;

namespace ProjectTrinity.Input
{
    public class MatchInputProvider
    {
        public float XTranslation { get; private set; }
        public float YTranslation { get; private set; }
        public float Rotation { get; private set; }
        public bool InputReceived { get; private set; }

        public float AimingRotation { get; private set; }
        public bool AimingInputReceived { get; private set; }
        public bool SpellInputReceived { get; private set; }

        public void AddXTranslation(float xTranslation)
        {
            XTranslation = Mathf.Clamp(xTranslation, -1f, 1f);
            InputReceived = true;
        }

        public void AddYTranslation(float yTranslation)
        {
            YTranslation = Mathf.Clamp(yTranslation, -1f, 1f);
            InputReceived = true;
        }

        public void AddRotation(float rotation)
        {
            Rotation = rotation;
            InputReceived = true;
        }

        public void AddAimingRotation(float rotation)
        {
            AimingRotation = rotation;
            AimingInputReceived = true;
        }

        public void AddSpellInput(float rotation)
        {
            Rotation = rotation;
            AimingRotation = rotation;
            SpellInputReceived = true;
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

        public byte GetSimulationAimingRotation()
        {
            return (byte)Mathf.Lerp(0, 255, Mathf.InverseLerp(0, 360f, MathHelper.Modulo(AimingRotation, 360)));
        }

        // should be called in every simulation frame so inputs in next update can be registered.
        public void Reset()
        {
            XTranslation = 0f;
            YTranslation = 0f;
            InputReceived = false;
            AimingInputReceived = false;
            SpellInputReceived = false;
        }
    }
}