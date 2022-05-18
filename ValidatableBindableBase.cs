using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
namespace Xamariners.Mvvm.Core
{
    public abstract class ValidatableBindableBase :
      BindableBase,
      IValidatableBindableBase,
      INotifyDataErrorInfo
    {
        protected readonly Class1.RecursiveBindableValidator _bindableValidator;

        [JsonIgnore]
        [NotMapped]
        public bool IsValidationEnabled
        {
            get => this._bindableValidator.IsValidationEnabled;
            set => this._bindableValidator.IsValidationEnabled = value;
        }

        [JsonIgnore]
        [NotMapped]
        public Class1.RecursiveBindableValidator Errors => this._bindableValidator;

        [JsonIgnore]
        [NotMapped]
        public bool HasErrors => !this.ValidateProperties((object)null, (List<Tuple<string, bool, string>>)null);

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => this._bindableValidator.ErrorsChanged += value;
            remove => this._bindableValidator.ErrorsChanged -= value;
        }

        public ValidatableBindableBase() => this._bindableValidator = new Class1.RecursiveBindableValidator((INotifyPropertyChanged)this);

        public ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors() => this._bindableValidator.GetAllErrors();

        public bool ValidateProperties(
          object entityToValidate = null,
          List<Tuple<string, bool, string>> validators = null)
        {
            return this._bindableValidator.ValidateProperties(entityToValidate, validators);
        }

        public bool ValidatePropertyList(Dictionary<string, object> properties) => this.ValidatePropertyList(properties, (Dictionary<string, string>)null);

        public bool ValidatePropertyList(
          Dictionary<string, object> properties,
          Dictionary<string, string> propertiesDisplayName,
          List<Tuple<string, bool, string>> validators = null)
        {
            return this._bindableValidator.ValidatePropertyList(properties, validators);
        }

        public bool ValidateProperty(string propertyName, object entityToValidate = null) => !this._bindableValidator.IsValidationEnabled || this._bindableValidator.ValidateProperty(propertyName, entityToValidate);

        public void SetAllErrors(
          IDictionary<string, ReadOnlyCollection<string>> entityErrors)
        {
            this._bindableValidator.SetAllErrors(entityErrors);
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            int num = base.SetProperty<T>(ref storage, value, propertyName) ? 1 : 0;
            if (num == 0 || string.IsNullOrEmpty(propertyName) || !this._bindableValidator.IsValidationEnabled)
                return num != 0;
            this._bindableValidator.ValidateProperty(propertyName);
            return num != 0;
        }

        protected bool SetPropertySafe<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            int num = this.SetPropertySafeInternal<T>(ref storage, value, propertyName) ? 1 : 0;
            if (num == 0 || string.IsNullOrEmpty(propertyName) || !this._bindableValidator.IsValidationEnabled)
                return num != 0;
            this._bindableValidator.ValidateProperty(propertyName);
            return num != 0;
        }

        private bool SetPropertySafeInternal<T>(ref T storage, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.RaisePropertyChangedSafeInternal(propertyName);
            return true;
        }

        protected void RaisePropertyChangedSafeInternal(string propertyName)
        {
            try
            {
                this.RaisePropertyChanged(propertyName);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FFImageLoading"))
                    return;
                throw;
            }
        }

        public IEnumerable GetErrors(string propertyName) => !this.HasErrors ? (IEnumerable)Enumerable.Empty<string>() : (IEnumerable)this._bindableValidator[propertyName];
    }
}
