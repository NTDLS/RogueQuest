namespace Library.Types
{
    public class Rectangle<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Width { get; set; }
        public T Height { get; set; }

        public Rectangle()
        {
        }

        public Rectangle(T x, T y, T width, T height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
