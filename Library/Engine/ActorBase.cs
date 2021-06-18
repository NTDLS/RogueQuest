﻿using Assets;
using Library.Types;
using Library.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library.Engine
{
    public class ActorBase
    {
        #region Public properties.
        public RotationMode RotationMode { get; set; }
        public string Tag { get; set; }
        public List<ActorBase> Children { get; set; }
        public EngineCoreBase Core { get; set; }

        #endregion

        public ActorBase(EngineCoreBase core)
        {
            Core = core;
            Children = new List<ActorBase>();
            this.Visible = true;
            RotationMode = RotationMode.Upsize;
            Velocity.OnChange += Velocity_OnChange;
        }

        private void Velocity_OnChange(Velocity<double> sender)
        {
            this.Invalidate();
        }

        #region Image.

        public Image _image = null;

        public void SetImage(Image image)
        {
            _image = image;
            _size = new Size(_image.Size.Width, _image.Size.Height);
        }

        public void SetImage(string imagePath)
        {
            _image = SpriteCache.GetBitmapCached(imagePath);
            _size = new Size(_image.Size.Width, _image.Size.Height);
            Invalidate();
        }

        public Image GetImage()
        {
            return _image;
        }

        public void Invalidate()
        {
            int slop = 1;

            var x = X - Core.Display.BackgroundOffset.X;
            var y = Y - Core.Display.BackgroundOffset.Y;

            Rectangle rect = new Rectangle(
                (int)(x - (Size.Width / 2.0)) - slop,
                (int)(y - (Size.Height / 2.0)) - slop,
                Size.Width + (slop * 2),
                Size.Height + (slop * 2));

            Core.Display.DrawingSurface.Invalidate(rect);
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
            double angle = (double)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            Bitmap bitmap = new Bitmap(rawImage);

            var x = _location.X - Core.Display.BackgroundOffset.X;
            var y = _location.Y - Core.Display.BackgroundOffset.Y;

            if (angle != 0 && RotationMode != RotationMode.None)
            {
                if (RotationMode == RotationMode.Upsize) //Very expensize
                {
                    var image = GraphicsUtility.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    Rectangle rect = new Rectangle((int)(x - (image.Width / 2.0)), (int)(y - (image.Height / 2.0)), image.Width, image.Height);
                    dc.DrawImage(image, rect);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == RotationMode.Clip) //Much less expensive.
                {
                    var image = GraphicsUtility.RotateImageWithClipping(bitmap, angle, Color.Transparent);
                    Rectangle rect = new Rectangle((int)(x - (image.Width / 2.0)), (int)(y - (image.Height / 2.0)), image.Width, image.Height);
                    dc.DrawImage(image, rect);

                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
            }
            else //Almost free.
            {
                Rectangle rect = new Rectangle((int)(x - (bitmap.Width / 2.0)), (int)(y - (bitmap.Height / 2.0)), bitmap.Width, bitmap.Height);
                dc.DrawImage(bitmap, rect);
            }
        }

        private int _drawOrder = 0;
        public int DrawOrder
        {
            get
            {
                return _drawOrder;
            }
            set
            {
                _drawOrder = value;
                Invalidate();
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
                    Invalidate();
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

        public bool IsOnScreen
        {
            get
            {
                return Core.Display.VisibleBounds.IntersectsWith(Bounds);
            }
        }

        public bool Intersects(Point<double> location, Point<double> size)
        {
            var alteredHitBox = new RectangleF(
                (float)(location.X),
                (float)(location.Y),
                (float)(size.X), (float)(size.Y));

            return this.Bounds.IntersectsWith(alteredHitBox);
        }

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
                Invalidate();
                _location = value;
                Invalidate();
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
                Invalidate();
                _location.X = value;
                PositionChanged();
                Invalidate();
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
                Invalidate();
                _location.Y = value;
                PositionChanged();
                Invalidate();
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

        #region Velocity.

        private Velocity<double> _velocity = new Velocity<double>();
        public Velocity<double> Velocity
        {
            get
            {
                return _velocity;
            }
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

        /// <summary>
        /// Returns the right angle rectangle that the image occupies.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return new RectangleF(
                    (float)(_location.X) - (Size.Width / 2),
                    (float)(_location.Y - (Size.Height / 2)),
                    (float)(Size.Width),
                    (float)(Size.Height));
            }
        }

        #endregion
    }
}
