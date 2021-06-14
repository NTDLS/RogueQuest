namespace RougueQuest.Types
{
    public class Size<T>
    {
        public T Width { get; set; }
        public T Height { get; set; }

        public Size()
        {

        }

        public Size(T width, T height)
        {
            Width = width;
            Height = height;
        }
    }
}
