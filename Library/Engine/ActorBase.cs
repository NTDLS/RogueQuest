using Assets;
using Library.Engine.Types;
using Library.Types;
using Library.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Library.Engine
{
    public class ActorBase
    {
        #region Public properties.

        /// <summary>
        /// Set to true when Core.BlindPlay is enabled. Allows the map to be slowly revealed.
        /// </summary>
        public bool HasBeenViewed { get; set; }

        /// <summary>
        /// Tells us exactly which tile this is. For terrain, it could be so many - so this allows us to differentiate.
        /// </summary>
        public string TilePath { get; set; }
        public bool DrawRealitiveToBackgroundOffset { get; set; } = true;
        public RotationMode RotationMode { get; set; }
        public string Tag { get; set; }
        /// <summary>
        /// Always render even if x,y is offscreen. For instance, text at 0,0 would be at the upper corner regardless of BG offset.
        /// </summary>
        public bool AlwaysRender { get; set; }
        public List<ActorBase> Children { get; set; }
        public EngineCoreBase Core { get; set; }
        public TileMetadata Meta { get; set; } = new TileMetadata();

        private Velocity<double> _velocity = new Velocity<double>();
        public Velocity<double> Velocity
        {
            get
            {
                return _velocity;
            }
        }

        #endregion

        public ActorBase(EngineCoreBase core)
        {
            Core = core;
            Children = new List<ActorBase>();
            _size = new Size(0, 0);
            this.Visible = true;
            RotationMode = RotationMode.Upsize;
            Velocity.OnChange += Velocity_OnChange;
        }

        public ActorBase Clone()
        {
            var actor = new ActorBase(Core)
            {
                X = this.X,
                Y = this.Y,
                TilePath = this.TilePath,
                AlwaysRender = AlwaysRender,
                DoNotDraw = DoNotDraw,
                DrawOrder = DrawOrder,
                RotationMode = RotationMode,
                Visible = Visible,
                DrawRealitiveToBackgroundOffset = DrawRealitiveToBackgroundOffset,
                SelectedHighlight = SelectedHighlight,
                HoverHighlight = HoverHighlight,
            };

            actor.Children = new List<ActorBase>(this.Children.Select(o => o.Clone()));
            actor.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees;
            actor.Velocity.MaxSpeed = this.Velocity.MaxSpeed;
            actor.Velocity.MaxRotationSpeed = this.Velocity.MaxRotationSpeed;
            actor.Velocity.ThrottlePercentage = this.Velocity.ThrottlePercentage;

            actor.Meta.OverrideWith(this.Meta);

            actor.SetImage(this.GetImage());

            return actor;
        }

        public TileIdentifier CloneIdentifier()
        {
            return new TileIdentifier(this.TilePath)
            {
                Meta = this.Meta
            };
        }

        private void Velocity_OnChange(Velocity<double> sender)
        {
            this.Invalidate();
        }

        /// <summary>
        /// Removes hitpoints and returns true if killed.
        /// </summary>
        /// <returns></returns>
        public bool Hit(int points)
        {
            this.Meta.HitPoints -= points;
            if (this.Meta.HitPoints <= 0)
            {
                this.QueueForDelete();
                return true;
            }
            return false;
        }

        #region Image.

        public Image _image = null;

        private bool _hoverHighlight = false;
        public bool HoverHighlight
        {
            get
            {
                return _hoverHighlight;
            }
            set
            {
                _hoverHighlight = value;
                Invalidate();
            }
        }

        private bool _selectedHighlight = false;
        public bool SelectedHighlight
        {
            get
            {
                return _selectedHighlight;
            }
            set
            {
                _selectedHighlight = value;
                Invalidate();
            }
        }

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
            int slop = 3; //This i just to take care of the debuging highlighting rectangles.

            var rect = new Rectangle(
                ScreenBounds.X - slop,
                ScreenBounds.Y - slop,
                ScreenBounds.Width + (slop * 2),
                ScreenBounds.Height + (slop * 2));

            Core.Display.DrawingSurface.Invalidate(rect);
        }

        public void Render(Graphics dc)
        {
            if (Visible && _image != null && !DoNotDraw)
            {
                if (DrawRealitiveToBackgroundOffset)
                {
                    DrawImageRealative(dc, _image);
                }
                else
                {
                    DrawImage(dc, _image);
                }
            }
        }

        private void DrawImageRealative(Graphics dc, Image rawImage, double? angleInDegrees = null)
        {
            double angle = (double)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            Bitmap bitmap = new Bitmap(rawImage);

            if (angle != 0 && RotationMode != RotationMode.None)
            {
                if (RotationMode == RotationMode.Upsize) //Very expensize
                {
                    var image = GraphicsUtility.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    dc.DrawImage(image, ScreenBounds);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == RotationMode.Clip) //Much less expensive.
                {
                    var image = GraphicsUtility.RotateImageWithClipping(bitmap, angle, Color.Transparent);
                    dc.DrawImage(image, ScreenBounds);

                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
            }
            else //Almost free.
            {
                dc.DrawImage(bitmap, ScreenBounds);
            }

            if (SelectedHighlight)
            {
                Pen pen = new Pen(Color.Red, 3);
                dc.DrawRectangle(pen, ScreenBounds);
            }

            if (HoverHighlight)
            {
                Pen pen = new Pen(Color.Yellow, 1);
                dc.DrawRectangle(pen, ScreenBounds);
            }
        }


        private void DrawImage(Graphics dc, Image rawImage, double? angleInDegrees = null)
        {
            double angle = (double)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            Bitmap bitmap = new Bitmap(rawImage);

            if (angle != 0 && RotationMode != RotationMode.None)
            {
                if (RotationMode == RotationMode.Upsize) //Very expensize
                {
                    var image = GraphicsUtility.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    dc.DrawImage(image, Bounds);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == RotationMode.Clip) //Much less expensive.
                {
                    var image = GraphicsUtility.RotateImageWithClipping(bitmap, angle, Color.Transparent);
                    dc.DrawImage(image, Bounds);

                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
            }
            else //Almost free.
            {
                dc.DrawImage(bitmap, Bounds);
            }

            if (SelectedHighlight)
            {
                Pen pen = new Pen(Color.Red, 3);
                dc.DrawRectangle(pen, BoundsI);
            }

            if (HoverHighlight)
            {
                Pen pen = new Pen(Color.Yellow, 1);
                dc.DrawRectangle(pen, BoundsI);
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

        /// <summary>
        /// This tile is treated like any other visible tile but is not actuallt painted. Its effectevly transparent.
        /// </summary>
        public bool DoNotDraw { get; set; } = false;

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

        public double AngleTo(ActorBase to)
        {
            return MathUtility.AngleTo(this, to);
        }

        public double DistanceTo(ActorBase to)
        {
            return MathUtility.DistanceTo(this, to);
        }

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

        public bool Intersects(ActorBase with, int slop = 0)
        {
            var alteredHitBox = new RectangleF(
                (float)(with.BoundLocation.X - slop),
                (float)(with.BoundLocation.Y - slop),
                (float)(with.Size.Width + (slop * 2)), (float)(with.Size.Height + (slop * 2)));

            return this.Bounds.IntersectsWith(alteredHitBox);
        }

        private Point<double> _location = new Point<double>();

        /// <summary>
        /// The xy of the center of the image. Do not modify the return location, it will not have any affect.
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

        /// <summary>
        /// The location of the center of the actor on the screen with BackgroundOffset taken into account.
        /// </summary>
        public Point<double> ScreenLocation
        {
            get
            {
                return new Point<double>(ScreenX, ScreenY);
            }
        }

        /// <summary>
        /// The absolute x,y of the upper left corner of the actor.
        /// </summary>
        public Point<double> BoundLocation
        {
            get
            {
                return new Point<double>(
                    (float)(_location.X) - (Size.Width / 2),
                    (float)(_location.Y - (Size.Height / 2)));
            }
        }

        /// <summary>
        /// The absolute X location of the center of the actor.
        /// </summary>
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

        /// <summary>
        /// The absolute Y location of the center of the actor.
        /// </summary>
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

        /// <summary>
        /// /// The X location of the center of the actor on the screen with BackgroundOffset taken into account.
        /// </summary>
        public double ScreenX
        {
            get
            {
                return _location.X - Core.Display.BackgroundOffset.X;
            }
        }

        /// <summary>
        /// /// The Y location of the center of the actor on the screen with BackgroundOffset taken into account.
        /// </summary>
        public double ScreenY
        {
            get
            {
                return _location.Y - Core.Display.BackgroundOffset.Y;
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

        /// <summary>
        /// The absolute location of the actors bounds.
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

        /// <summary>
        /// The absolute location of the actors bounds (as an interger).
        /// </summary>
        public Rectangle BoundsI
        {
            get
            {
                return new Rectangle(
                    (int)(_location.X) - (int)(Size.Width / 2.0),
                    (int)(_location.Y - (int)(Size.Height / 2.0)),
                    (int)(Size.Width),
                    (int)(Size.Height));
            }
        }


        /// <summary>
        /// The bounds of the actor on the screen with BackgroundOffset taken into account.
        /// </summary>
        public Rectangle ScreenBounds
        {
            get
            {
                return new Rectangle(
                    (int)(ScreenX) - (int)(Size.Width / 2.0),
                    (int)(ScreenY) - (int)(Size.Height / 2.0),
                    (int)(Size.Width),
                    (int)(Size.Height));
            }
        }

        #endregion

        public bool IsPointingAt( ActorBase atObj, double toleranceDegrees)
        {
            var deltaAngle = Math.Abs(MathUtility.DeltaAngle(this, atObj));
            return deltaAngle <= toleranceDegrees || deltaAngle >= (360 - toleranceDegrees);
        }

        public bool IsPointingAt( ActorBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle = 0)
        {
            var deltaAngle = Math.Abs(MathUtility.DeltaAngle(this, atObj, offsetAngle));
            if (deltaAngle <= toleranceDegrees || deltaAngle >= (360 - toleranceDegrees))
            {
                double distance = MathUtility.DistanceTo(this, atObj);

                return distance <= maxDistance;
            }

            return false;
        }


        /// <summary>
        /// Reads metadata from the asset directories and refreshes the values stored with the actor.
        /// </summary>
        public void RefreshMetadata(bool keepExplicitlyValues)
        {
            Guid? uid = Meta?.UID;

            var freshMeta = TileMetadata.GetFreshMetadata(this.TilePath);

            if (keepExplicitlyValues)
            {
                this.Meta.OverrideWith(freshMeta);
            }
            else
            {
                this.Meta = freshMeta;
            }

            if (uid != null)
            {
                this.Meta.UID = uid; //Never change the UID once it is set.
            }

            //All items need UIDs.
            if (freshMeta.ActorClass != ActorClassName.ActorTerrain && this.Meta.UID == null)
            {
                this.Meta.UID = Guid.NewGuid();
            }
        }
    }
}
