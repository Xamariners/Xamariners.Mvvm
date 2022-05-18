using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Xamariners.Mvvm.Core;

public class Class1
{
    public class RecursiveBindableValidator : BindableBase
    {
        public static readonly ReadOnlyCollection<string> EmptyErrorsCollection = new ReadOnlyCollection<string>((IList<string>)new List<string>());
        private IDictionary<string, ReadOnlyCollection<string>> _errors = (IDictionary<string, ReadOnlyCollection<string>>)new Dictionary<string, ReadOnlyCollection<string>>();
        private readonly INotifyPropertyChanged _entityToValidate;
        private readonly Func<string, string, string> _getResourceDelegate;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public ReadOnlyCollection<string> this[string propertyName] => !this._errors.ContainsKey(propertyName) ? RecursiveBindableValidator.EmptyErrorsCollection : this._errors[propertyName];

        public IDictionary<string, ReadOnlyCollection<string>> Errors => this._errors;

        public bool IsValidationEnabled { get; set; }

        public RecursiveBindableValidator(
          INotifyPropertyChanged entityToValidate,
          Func<string, string, string> getResourceDelegate)
          : this(entityToValidate)
        {
            this._getResourceDelegate = getResourceDelegate;
        }

        public RecursiveBindableValidator(INotifyPropertyChanged entityToValidate)
        {
            this._entityToValidate = entityToValidate ?? throw new ArgumentNullException(nameof(entityToValidate));
            this.IsValidationEnabled = true;
        }

        public ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors() => new ReadOnlyDictionary<string, ReadOnlyCollection<string>>(this._errors);

        public void SetAllErrors(
          IDictionary<string, ReadOnlyCollection<string>> entityErrors)
        {
            if (entityErrors == null)
                throw new ArgumentNullException(nameof(entityErrors));
            this._errors.Clear();
            foreach (KeyValuePair<string, ReadOnlyCollection<string>> entityError in (IEnumerable<KeyValuePair<string, ReadOnlyCollection<string>>>)entityErrors)
                this.SetPropertyErrors(entityError.Key, (IList<string>)entityError.Value);
            this.RaisePropertyChanged("Item[]");
            this.OnErrorsChanged(string.Empty);
        }

        public bool ValidateProperty(string propertyName, object entityToValidate = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            if (entityToValidate == null)
                entityToValidate = (object)this._entityToValidate;
            PropertyInfo runtimeProperty = entityToValidate.GetType().GetRuntimeProperty(propertyName);
            if (runtimeProperty == (PropertyInfo)null)
                throw new ArgumentException("InvalidPropertyNameException", propertyName);
            List<string> stringList = new List<string>();
            int num = this.TryValidateProperty(runtimeProperty, stringList, entityToValidate) ? 1 : 0;
            if (!this.SetPropertyErrors(runtimeProperty.Name, (IList<string>)stringList))
                return num != 0;
            this.OnErrorsChanged(propertyName);
            this.RaisePropertyChanged("Item[" + propertyName + "]");
            return num != 0;
        }

        public bool ValidateProperties(
          object entityToValidate = null,
          List<Tuple<string, bool, string>> validators = null)
        {
            List<string> stringList = new List<string>();
            this._errors.Clear();
            if (entityToValidate == null)
                entityToValidate = (object)this._entityToValidate;
            this.ValidatePropertiesRecursive(entityToValidate, stringList);
            if (validators != null)
            {
                foreach (Tuple<string, bool, string> validator in validators)
                {
                    if (!validator.Item2)
                    {
                        if (this.SetPropertyErrors(validator.Item1, (IList<string>)new List<string>()
            {
              validator.Item3
            }) && !stringList.Contains(validator.Item1))
                            stringList.Add(validator.Item1);
                    }
                }
            }
            foreach (string propertyName in stringList)
            {
                this.OnErrorsChanged(propertyName);
                this.RaisePropertyChanged("Item[" + propertyName + "]");
            }
            return this._errors.Values.Count == 0;
        }

        public bool ValidatePropertyList(
          Dictionary<string, object> properties,
          List<Tuple<string, bool, string>> validators)
        {
            List<string> stringList1 = new List<string>();
            this._errors.Clear();
            foreach (KeyValuePair<string, object> property in properties)
            {
                string key = property.Key;
                string name = ((IEnumerable<string>)key.Split('.')).LastOrDefault<string>();
                List<string> stringList2 = new List<string>();
                this.TryValidateProperty(property.Value.GetType().GetProperty(name), stringList2, property.Value, key);
                if (this.SetPropertyErrors(key, (IList<string>)stringList2) && !stringList1.Contains(key))
                    stringList1.Add(key);
            }
            foreach (Tuple<string, bool, string> validator in validators)
            {
                if (!validator.Item2)
                {
                    List<string> propertyNewErrors = new List<string>()
          {
            validator.Item3
          };
                    if (this.SetPropertyErrors(validator.Item1, (IList<string>)propertyNewErrors) && !stringList1.Contains(validator.Item1))
                        stringList1.Add(validator.Item1);
                }
            }
            foreach (string propertyName in stringList1)
            {
                this.OnErrorsChanged(propertyName);
                this.RaisePropertyChanged("Item[" + propertyName + "]");
            }
            return this._errors.Values.Count == 0;
        }

        private List<string> ValidatePropertiesRecursive(
          object instance,
          List<string> stringList,
          int level = 4)
        {
            if (level == 0)
                return stringList;
            if (instance == null)
                return (List<string>)null;
            foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>)instance.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>)(c => c.GetCustomAttributes().Any<Attribute>((Func<Attribute, bool>)(a => a.GetType().GetTypeInfo().BaseType.Name == "ValidationAttribute")))).ToList<PropertyInfo>())
            {
                List<string> stringList1 = new List<string>();
                object instance1 = propertyInfo.GetValue(instance, (object[])null);
                Type[] source = new Type[7]
                {
          typeof (Enum),
          typeof (string),
          typeof (Decimal),
          typeof (DateTime),
          typeof (DateTimeOffset),
          typeof (TimeSpan),
          typeof (Guid)
                };
                if (propertyInfo.PropertyType.GetTypeInfo().IsPrimitive || ((IEnumerable<Type>)source).Contains<Type>(propertyInfo.PropertyType) || propertyInfo.PropertyType.GetTypeInfo().IsGenericType && propertyInfo.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    this.TryValidateProperty(propertyInfo, stringList1, instance);
                    if (this.SetPropertyErrors(propertyInfo.Name, (IList<string>)stringList1) && !stringList.Contains(propertyInfo.Name))
                        stringList.Add(propertyInfo.Name);
                }
                else
                    this.ValidatePropertiesRecursive(instance1, stringList, level - 1);
            }
            return stringList;
        }

        private bool TryValidateProperty(
          PropertyInfo propertyInfo,
          List<string> propertyErrors,
          object entityToValidate = null,
          string fullPropertyName = null)
        {
            List<ValidationResult> source = new List<ValidationResult>();
            if (entityToValidate == null)
                entityToValidate = (object)this._entityToValidate;
            ValidationContext validationContext = new ValidationContext(entityToValidate)
            {
                MemberName = propertyInfo.Name
            };
            int num = Validator.TryValidateProperty(propertyInfo.GetValue(entityToValidate), validationContext, (ICollection<ValidationResult>)source) ? 1 : 0;
            if (!source.Any<ValidationResult>())
                return num != 0;
            propertyErrors.AddRange(source.Select<ValidationResult, string>((Func<ValidationResult, string>)(c => c.ErrorMessage.Replace(propertyInfo.Name, fullPropertyName ?? propertyInfo.Name))));
            return num != 0;
        }

        private bool SetPropertyErrors(string propertyName, IList<string> propertyNewErrors)
        {
            bool flag = false;
            if (!this._errors.ContainsKey(propertyName))
            {
                if (propertyNewErrors.Count > 0)
                {
                    this._errors.Add(propertyName, new ReadOnlyCollection<string>(propertyNewErrors));
                    flag = true;
                }
            }
            else if (propertyNewErrors.Count != this._errors[propertyName].Count || this._errors[propertyName].Intersect<string>((IEnumerable<string>)propertyNewErrors).Count<string>() != propertyNewErrors.Count)
            {
                if (propertyNewErrors.Count > 0)
                    this._errors[propertyName] = new ReadOnlyCollection<string>(propertyNewErrors);
                else
                    this._errors.Remove(propertyName);
                flag = true;
            }
            return flag;
        }

        private void OnErrorsChanged(string propertyName)
        {
            if (!this.IsValidationEnabled)
                return;
            EventHandler<DataErrorsChangedEventArgs> errorsChanged = this.ErrorsChanged;
            if (errorsChanged == null)
                return;
            errorsChanged((object)this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
