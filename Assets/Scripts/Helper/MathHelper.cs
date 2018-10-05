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
    }
}