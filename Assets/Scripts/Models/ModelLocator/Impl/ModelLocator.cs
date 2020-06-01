using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RR.Models.ModelLocator.Impl
{
    public abstract class ModelLocator : MonoBehaviour, IModelLocator
    {
        private readonly List<IModel> _models = new List<IModel>();

        public void Awake()
        {
            CreateModels();
            InitializeModels();
        }

        protected abstract void CreateModels();
        
        protected void AddModel<TModel>() where TModel : IModel, new()
        {
            var model = new TModel();
            AddModel(model);
        }

        protected void AddModel(IModel model)
        {
            if (!_models.Contains(model))
            {
                _models.Add(model);
            }
        }

        private void InitializeModels()
        {
            foreach (var model in _models)
            {
                model.Initialize();
            }
        }

        public TModel Get<TModel>() where TModel : IModel
        {
            var key = typeof(TModel);
            
            foreach (var model in _models.OfType<TModel>())
            {
                return model;
            }

            throw new KeyNotFoundException(key.Name + " not found. Was it added to the ModelLocator?");
        }
    }
}
