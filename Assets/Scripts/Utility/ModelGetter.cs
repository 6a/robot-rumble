using RR.Models;
using RR.Models.ModelLocator;
using RR.Models.ModelLocator.Impl;
using UnityEngine;
using UnityEngine.Assertions;

namespace RR.Utility
{
    public class ModelGetter 
    {
        private IModelLocator _modelLocator;
        private IModelLocator ModelLocator
        {
            get
            {
                if (_modelLocator == null)
                {
                    _modelLocator = GameObject.FindObjectOfType<ModelLocator>();
                    Assert.IsNotNull(_modelLocator, "You must define a " + typeof(IModelLocator).Name);
                }
                return _modelLocator;
            }
        }
        
        public T Get<T>() where T : IModel
        {
            return ModelLocator.Get<T>();
        }
    }
}