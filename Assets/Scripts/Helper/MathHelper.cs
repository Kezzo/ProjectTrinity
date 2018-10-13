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
    }
}