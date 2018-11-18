using UnityEngine;

namespace ProjectTrinity.Helper
{
    public static class MathHelper
    {
        public static int Modulo(int value, int modulo)
        {
            return (value % modulo + modulo) % modulo;
        }

        public static long Modulo(long value, long modulo)
        {
            return (value % modulo + modulo) % modulo;
        }


        public static float Modulo(float value, float modulo)
        {
            return (value % modulo + modulo) % modulo;
        }

        public static int Abs(int value)
        {
            return Mathf.Abs(value);
        }

        public static float[] GetCappedTranslations(float xTranslation, float yTranslation)
        {
            float combinedTranslation = Mathf.Abs(xTranslation) + Mathf.Abs(yTranslation);

            if(combinedTranslation <= 1.5f)
            {
                return new float[]
                {
                    xTranslation,
                    yTranslation
                };
            }

            float overhead = Mathf.Abs(1.5f - combinedTranslation) / 2;

            xTranslation = xTranslation > 0 ? xTranslation - overhead : xTranslation + overhead;
            yTranslation = yTranslation > 0 ? yTranslation - overhead : yTranslation + overhead;

            return new float []
            {
                xTranslation,
                yTranslation
            };
        }

        public static int LimitValue(int value, int limit)
        {
            if(Mathf.Abs(value) <= limit)
            {
                return value;
            }

            return value > 0 ? limit : -limit;
        }

        public static int LimitValueDelta(int value, int delta, int limit)
        {
            int appliedDeltaValue = value + delta;

            if (Mathf.Abs(appliedDeltaValue) <= limit)
            {
                return delta;
            }

            int maxDelta = limit - Mathf.Abs(value);

            return delta > 0 ? maxDelta : -maxDelta;
        }

        public static byte GetFrameDiff(byte frame1, byte frame2)
        {
            byte smallerFrame = frame1 > frame2 ? frame2 : frame1;
            byte biggerFrame = frame1 <= frame2 ? frame2 : frame1;
            if (smallerFrame >= 0 && smallerFrame < 30 && (254 - biggerFrame) < 30)
            {
                return (byte) (254 - biggerFrame + smallerFrame);
            }

            return (byte)(frame1 > frame2 ? frame1 - frame2 : frame2 - frame1);
        }

        public static float GetMaxFrameTranslation(Vector3 positionChange, byte frames)
        {
            int xSimulationPositionChangePerFrame = UnitValueConverter.ToSimulationPosition(Mathf.Abs(positionChange.x)) / frames;
            int ySimulationPositionChangePerFrame = UnitValueConverter.ToSimulationPosition(Mathf.Abs(positionChange.z)) / frames;

            float xTranslationPerFrame = xSimulationPositionChangePerFrame / 250;
            float yTranslationPerFrame = ySimulationPositionChangePerFrame / 250;

            return xTranslationPerFrame > yTranslationPerFrame ? xTranslationPerFrame : yTranslationPerFrame;
        }

        public static byte GetRoundedMaxFramesForPositionChange(Vector3 positionChange)
        {
            int xSimulationPositionChange = UnitValueConverter.ToSimulationPosition(Mathf.Abs(positionChange.x));
            int ySimulationPositionChange = UnitValueConverter.ToSimulationPosition(Mathf.Abs(positionChange.z));

            byte roundedFramesNeedToReachX = (byte) Mathf.Round(xSimulationPositionChange / 250);
            byte roundedFramesNeedToReachY = (byte) Mathf.Round(ySimulationPositionChange / 250);

            return roundedFramesNeedToReachX > roundedFramesNeedToReachY ? roundedFramesNeedToReachX : roundedFramesNeedToReachY;
        }
    }
}