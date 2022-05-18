using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Xamariners.Mvvm.Core
{
    public interface IValidatableBindableBase
    {
        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        bool IsValidationEnabled { get; set; }

        Class1.RecursiveBindableValidator Errors { get; }

        ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors();

        bool ValidateProperties(object entityToValidate = null, List<Tuple<string, bool, string>> validators = null);

        bool ValidatePropertyList(Dictionary<string, object> properties);

        bool ValidateProperty(string propertyName, object entityToValidate = null);

        void SetAllErrors(
            IDictionary<string, ReadOnlyCollection<string>> entityErrors);
    }
}
