using System;
using System.Collections.Generic;

namespace Library.Engine
{
    public class Container
    {
        public Guid ContainerId { get; set; }
        public List<TileIdentifier> Contents { get; set; }

        public Container(Guid containerId)
        {
            ContainerId = containerId;
            Contents = new List<TileIdentifier>();
        }

        public Container()
        {
            Contents = new List<TileIdentifier>();
        }

        public void Add(TileIdentifier chunk)
        {
            Contents.Add(chunk);
        }

        public void Clear()
        {
            Contents.Clear();
        }
    }
}
