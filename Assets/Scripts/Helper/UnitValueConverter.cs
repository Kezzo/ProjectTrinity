namespace ProjectTrinity.Helper
{
    public static class UnitValueConverter
    {
        //TODO: make sense configurable
        private static readonly float positionDivisor = 1000;
        private static readonly int toUnityRotationFactor = 360 / byte.MaxValue;
        private static readonly int toSimulationRotationFactor = byte.MaxValue / 360;

        public static float ToUnityPosition(int simulationPosition)
        {
            return simulationPosition / positionDivisor;
        }

        public static float ToUnityRotation(byte simulationRotation)
        {
            return simulationRotation * toUnityRotationFactor;
        }

        public static int ToSimulationPosition(float unityPosition)
        {
            return (int) (unityPosition * positionDivisor);
        }

        public static byte ToSimulationRotation(float unityRotation)
        {
            return (byte) (MathHelper.Modulo(unityRotation, 360) * toSimulationRotationFactor);
        }
    }
}