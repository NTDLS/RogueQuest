using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class ContainerController
    {
        public EngineCoreBase Core { get; set; }

        public List<Container> Collection { get; private set; }

        public ContainerController(EngineCoreBase core)
        {
            Core = core;
            Collection = new List<Container>();
        }

        public void Clear()
        {
            Collection.Clear();
        }

        /// <summary>
        /// Removes a single container from the collection bt the container id (which is the UID of the parent tile).
        /// </summary>
        /// <param name="parentId"></param>
        public void Remove(Guid containerId)
        {
            Collection.RemoveAll(o => o.ContainerId == containerId);
        }

        public Container GetContainer(Guid containerId)
        {
            var container = Collection.Where(o => o.ContainerId == containerId).FirstOrDefault();
            if (container != null)
            {
                return container;
            }

            container = new Container(containerId);

            Collection.Add(container);

            return container;
        }

        public void Set(List<Container> collection)
        {
            Clear();
            Collection.AddRange(collection);
        }

        public void Add(Container container)
        {
            Collection.Add(container);
        }

    }
}
