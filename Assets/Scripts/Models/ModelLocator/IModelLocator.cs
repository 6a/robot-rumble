using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RR.Models.ModelLocator
{
    public interface IModelLocator
    {
        T Get<T>() where T : IModel;
    }
}

