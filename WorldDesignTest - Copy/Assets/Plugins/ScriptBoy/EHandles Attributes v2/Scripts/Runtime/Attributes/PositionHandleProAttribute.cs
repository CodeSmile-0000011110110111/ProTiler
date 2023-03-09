namespace EHandles
{
    public class PositionHandleProAttribute : Attribute
    {
        public float buttonSize = 0.1f;
        public Color buttonColor = Color.white;

        /// <summary>
        /// <para>[buttonSize = float]</para> 
        /// <para>[color = EHandleColor]</para> 
        /// </summary>
        public PositionHandleProAttribute()
        {

        }

        public PositionHandleProAttribute(float size)
        {
            buttonSize = size;
        }

        public PositionHandleProAttribute(float size, Color color)
        {
            buttonSize = size;
            buttonColor = color;
        }
    }
}