using System;

namespace ModularFramework.Implementers {
    public abstract class BaseInteractionModule : MarshalByRefObject, IInteractionModule {
        IInteractionServiceProvider serviceProviderCore;
        SharedPropertyBag sharedPropertyBagCore;
        public BaseInteractionModule() {
            sharedPropertyBagCore = new SharedPropertyBag(this);
        }
        public IInteractionServiceProvider ServiceProvider {
            get { return serviceProviderCore; }
        }
        public abstract void Initialize();
        public abstract void Launch();
        public abstract void Stop();
        public abstract void Dispose();
        #region IInteractionModule Implementation
        IInteractionServiceProvider IInteractionModule.ServiceProvider {
            get { return serviceProviderCore; }
            set { serviceProviderCore = value; }
        }
        ISharedPropertyBag IInteractionModule.SharedProperties {
            get { return sharedPropertyBagCore; }
        }
        #endregion
    }
}
