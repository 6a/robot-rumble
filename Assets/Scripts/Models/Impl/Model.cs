using RR.Utility;
using System;

namespace RR.Models
{
    public abstract class Model
    {
        protected ModelGetter Models = new ModelGetter();
        protected EventDispatcher EventDispatcher = new EventDispatcher();
    }
}
