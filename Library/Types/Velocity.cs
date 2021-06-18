namespace Library.Types
{
    public class Velocity<T>
    {
        public delegate void ChangeEvent(Velocity<T> sender);
        public event ChangeEvent OnChange;

        public Angle<T> Angle { get; private set; } = new Angle<T>();
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

        public Velocity()
        {
            Angle.OnChange += Angle_OnChange;
        }

        private void Angle_OnChange(Angle<T> sender)
        {
            OnChange?.Invoke(this);
        }

        public virtual void ThrottleChanged()
        {
        }
    }
}
