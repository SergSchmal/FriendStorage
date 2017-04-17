using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FriendStorage.UI.Wrapper.Base
{
    public class ModelWrapper<T> : NotifyDataErrorInfoBase, IValidatableTrackingObject, IValidatableObject
    {
        private readonly Dictionary<string, object> _originalValues;

        private readonly List<IValidatableTrackingObject> _trackingObjects;

        public ModelWrapper(T model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            Model = model;
            _originalValues = new Dictionary<string, object>();
            _trackingObjects = new List<IValidatableTrackingObject>();
            InitializeComplexProperty(model);
            InitializeCollectionProperty(model);
            Validate();
        }

        protected virtual void InitializeCollectionProperty(T model)
        {
            
        }

        protected virtual void InitializeComplexProperty(T model)
        {
            
        }

        public T Model { get; }

        public bool IsValid => !HasErrors && _trackingObjects.All(t => t.IsValid);

        public bool IsChanged => _originalValues.Count > 0 || _trackingObjects.Any(o => o.IsChanged);

        public void AcceptChanges()
        {
            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects)
                trackingObject.AcceptChanges();
            OnPropertyChanged("");
        }

        public void RejectChanges()
        {
            foreach (var originalValue in _originalValues)
            {
                var propertyInfo = typeof(T).GetProperty(originalValue.Key);
                if (propertyInfo != null) propertyInfo.SetValue(Model, originalValue.Value);
            }
            _originalValues.Clear();
            foreach (var trackingObject in _trackingObjects)
                trackingObject.RejectChanges();
            Validate();
            OnPropertyChanged("");
        }

        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            return (TValue) propertyInfo.GetValue(Model);
        }

        protected TValue GetOriginalValue<TValue>(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName) ? (TValue) _originalValues[propertyName] : GetValue<TValue>(propertyName);
        }

        protected bool GetIsChanged(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName);
        }

        protected void SetValue<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) return;
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            if (propertyInfo == null) return;
            var currentValue = propertyInfo.GetValue(Model);
            if (Equals(newValue, currentValue)) return;
            UpdateOriginalValue(currentValue, newValue, propertyName);
            propertyInfo.SetValue(Model, newValue);
            Validate();
            OnPropertyChanged(propertyName);
            OnPropertyChanged(propertyName + "IsChanged");
        }

        private void Validate()
        {
            ClearErrors();
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);
            Validator.TryValidateObject(this, context, results, true);
            if (results.Any())
            {
                var propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();
                foreach (var propertyName in propertyNames)
                {
                    Errors[propertyName] = results.Where(r => r.MemberNames.Contains(propertyName)).Select(r => r.ErrorMessage).Distinct().ToList();
                    OnErrosChanged(propertyName);
                }
            }
            OnPropertyChanged(nameof(IsValid));
        }

        private void UpdateOriginalValue(object currentValue, object newValue, string propertyName)
        {
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName, currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            else
            {
                if (Equals(_originalValues[propertyName], newValue))
                {
                    _originalValues.Remove(propertyName);
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }

        protected void RegisterCollection<TWrapper, TModel>(ChangeTrackingCollection<TWrapper> wrapperCollection, List<TModel> modelCollection)
            where TWrapper : ModelWrapper<TModel>
        {
            wrapperCollection.CollectionChanged += (s, e) =>
            {
                modelCollection.Clear();
                modelCollection.AddRange(wrapperCollection.Select(w => w.Model));
                Validate();
            };
            RegisterTrackingObjects(wrapperCollection);
        }

        protected void RegisterComplex<TModel>(ModelWrapper<TModel> wrapper)
        {
            RegisterTrackingObjects(wrapper);
        }

        private void RegisterTrackingObjects(IValidatableTrackingObject trackingObject)
        {
            if (!_trackingObjects.Contains(trackingObject))
            {
                _trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
            }
        }

        private void TrackingObjectPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IsChanged))
                OnPropertyChanged(nameof(IsChanged));
            else if (args.PropertyName == nameof(IsValid))
                OnPropertyChanged(nameof(IsValid));
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}