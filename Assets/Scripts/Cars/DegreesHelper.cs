namespace Cars
{
    public static class DegreesHelper
    {
        public static float MapDegreeTo0360(float angle)
        {
            while (angle < 0)
                angle += 360;

            while (angle >= 360)
                angle -= 360;

            return angle;
        }
    }
}