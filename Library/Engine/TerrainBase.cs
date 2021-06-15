using Library.Types;
using Library.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RougueQuest.Terrain
{
    public class TerrainBase
    {
        #region Public properties. 
        public RotationMode RotationMode { get; set; }
        public Angle<double> Angle { get; set; } = new Angle<double>();
        public string Tag { get; set; }
        public Guid UID { get; private set; } = Guid.NewGuid();
        public List<TerrainBase> Children { get; set; }

        #endregion

        public TerrainBase()
        {
            UID = Guid.NewGuid();
            Children = new List<TerrainBase>();
            _size = new Size(0, 0);
            this.Visible = true;
            RotationMode = RotationMode.Upsize;
        }

        #region Image.

        public Image _image = null;

        public void SetImage(Image image)
        {
            _image = image;

            /*
            if (size != null)
            {
                _image = Utility.GraphicsUtility.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }
            */
            _size = new Size(_image.Size.Width, _image.Size.Height);
        }

        public void SetImage(string imagePath)
        {
            _image = SpriteCache.GetBitmapCached(imagePath);

            /*
            if (size != null)
            {
                _image = Utility.GraphicsUtility.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }
            */

            _size = new Size(_image.Size.Width, _image.Size.Height);
        }

        public Image GetImage()
        {
            return _image;
        }

        public void Render(Graphics dc)
        {
            if (Visible && _image != null)
            {
                DrawImage(dc, _image);
            }
        }

        private void DrawImage(Graphics dc, Image rawImage, double? angleInDegrees = null)
        {
            double angle = (double)(angleInDegrees == null ? Angle.Degrees : angleInDegrees);

            Bitmap bitmap = new Bitmap(rawImage);

            if (angle != 0 && RotationMode != RotationMode.None)
            {
                if (RotationMode == RotationMode.Upsize) //Very expensize
                {
                    var image = GraphicsUtility.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    Rectangle rect = new Rectangle((int)(_location.X - (image.Width / 2.0)), (int)(_location.Y - (image.Height / 2.0)), image.Width, image.Height);
                    dc.DrawImage(image, rect);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == RotationMode.Clip) //Much less expensive.
                {
                    var image = GraphicsUtility.RotateImageWithClipping(bitmap, angle, Color.Transparent);
                    Rectangle rect = new Rectangle((int)(_location.X - (image.Width / 2.0)), (int)(_location.Y - (image.Height / 2.0)), image.Width, image.Height);
                    dc.DrawImage(image, rect);

                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
            }
            else //Almost free.
            {
                Rectangle rect = new Rectangle((int)(_location.X - (bitmap.Width / 2.0)), (int)(_location.Y - (bitmap.Height / 2.0)), bitmap.Width, bitmap.Height);
                dc.DrawImage(bitmap, rect);
            }
        }

        #endregion

        #region Visibility.

        private bool _Visible = false;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (value != _Visible)
                {
                    _Visible = value;
                    VisibilityChanged();
                }
            }
        }

        public virtual void VisibilityChanged()
        {
        }

        public void QueueForDelete()
        {
            Visible = false;
            if (_readyForDeletion == false)
            {
                VisibilityChanged();
            }

            foreach (var child in Children)
            {
                child.QueueForDelete();
            }

            _readyForDeletion = true;
        }

        private bool _readyForDeletion;
        public bool ReadyForDeletion
        {
            get
            {
                return _readyForDeletion;
            }
        }

        #endregion

        #region Location.

        private Point<double> _location = new Point<double>();

        /// <summary>
        /// Do not modify this location, it will not have any affect.
        /// </summary>
        public Point<double> Location
        {
            get
            {
                return new Point<double>(_location);
            }
            set
            {
                _location = value;
            }
        }

        public double X
        {
            get
            {
                return _location.X;
            }
            set
            {
                _location.X = value;
                PositionChanged();
            }
        }

        public double Y
        {
            get
            {
                return _location.Y;
            }
            set
            {
                _location.Y = value;
                PositionChanged();
            }
        }

        public Point<double> LocationCenter
        {
            get
            {
                return new Point<double>(_location.X - (Size.Width / 2.0), _location.Y - (Size.Height / 2.0));
            }
        }

        public virtual void PositionChanged()
        {
        }

        #endregion

        #region Size.

        private Size _size;
        public virtual Size Size
        {
            get
            {
                return _size;
            }
        }

        public RectangleF Bounds
        {
            get
            {
                return new RectangleF((float)(_location.X), (float)(_location.Y), Size.Width, Size.Height);
            }
        }

        public Rectangle BoundsI
        {
            get
            {
                return new Rectangle((int)(_location.X), (int)(_location.Y), Size.Width, Size.Height);
            }
        }

        #endregion
    }
}
