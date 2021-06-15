namespace Library.Types
{
    public class Velocity<T>
    {
        public Angle<T> Angle { get; set; } = new Angle<T>();
        public T MaxSpeed { get; set; }
        public T MaxRotationSpeed { get; set; }

        public T _throttlePercentage;
        public T ThrottlePercentage
        {
            get
            {
                return _throttlePercentage;
            }
            set
            {
                _throttlePercentage = value;
                _throttlePercentage = (dynamic)_throttlePercentage > 1 ? 1 : (dynamic)_throttlePercentage;
                _throttlePercentage = (dynamic)_throttlePercentage < -1 ? -1 : (dynamic)_throttlePercentage;

                ThrottleChanged();
            }
        }

        public virtual void ThrottleChanged()
        {
        }
    }
}
