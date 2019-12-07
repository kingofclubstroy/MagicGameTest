namespace spriteHandleing
{
    public class BaseSprite
    {

        public string name;
        public int height;
        public int width;

        public Pixelnfo[,] pixelArray;

        public BaseSprite(string name, Pixelnfo[,] array)
        {
            this.pixelArray = array;
            this.name = name;
        }


    }

    
}

