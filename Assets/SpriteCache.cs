using System.Collections.Generic;
using System.Drawing;

namespace Assets
{
    public static class SpriteCache
    {
        private static Dictionary<string, Bitmap> _bitmapCache = new Dictionary<string, Bitmap>();

        public static Bitmap GetBitmapCached(string path)
        {
            Bitmap result = null;

            path = path.ToLower();

            lock (_bitmapCache)
            {
                if (_bitmapCache.ContainsKey(path))
                {
                    result = _bitmapCache[path].Clone() as Bitmap;
                }
                else
                {
                    using (var image = Image.FromFile(path))
                    using (var newbitmap = new Bitmap(image))
                    {
                        result = newbitmap.Clone() as Bitmap;
                        _bitmapCache.Add(path, result);
                    }
                }
            }

            return result;
        }
    }
}
