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
    }
}