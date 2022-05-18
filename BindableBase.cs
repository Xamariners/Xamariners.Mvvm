using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamariners.Mvvm.Core
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanging(propertyName);
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual bool SetProperty<T>(
            ref T storage,
            T value,
            Action onPropertyChanging,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            if (onPropertyChanging != null)
                onPropertyChanging();
            this.OnPropertyChanging(propertyName);
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChangingEventHandler propertyChanging = this.PropertyChanging;
            if (propertyChanging == null)
                return;
            propertyChanging((object)this, new PropertyChangingEventArgs(propertyName));
        }
    }
}
