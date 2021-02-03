using System;

namespace TurboVision.Objects
{
    /// <summary>
    /// TObject is the starting point of Turbo Vision's object hierarchy. As the base object, it has no ancestor but many descendants.
    /// </summary>
    [Serializable]
	public class Object
	{
        /// <summary>
        /// Allocates space on the heap for the object and fills it with zeros. Called by all derived objects' constructors. TObject.Init will zero all fields in descendants, so you should always call TObject.Init before initializing any fields in the derived objects' constructors.
        /// </summary>
		public Object()
		{
		}

        /// <summary>
        /// Performs the necessary cleanup and disposal for dynamic objects.
        /// </summary>
		public virtual void Done()
		{
		}

        /// <summary>
        /// Disposes of the object and calls the Done finalization.
        /// </summary>
		public void Free()
		{
			Done();
		}
	}
}
