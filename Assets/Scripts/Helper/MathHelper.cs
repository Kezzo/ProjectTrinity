namespace ProjectTrinity.Helper
{
    public static class MathHelper
    {
        public static int Modulo(int value, int modulo)
        {
            return ((value - 1) % modulo + modulo) % modulo;
        }

        public static float Modulo(float value, int modulo)
        {
            return ((value - 1) % modulo + modulo) % modulo;
        }
    }
}