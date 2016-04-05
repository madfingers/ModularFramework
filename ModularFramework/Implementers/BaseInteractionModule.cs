using System;

namespace ModularFramework.Implementers {
    public abstract class BaseInteractionModule : MarshalByRefObject, IInteractionModule {
        IInteractionServiceProvider interactionServiceProviderCore;
        SharedPropertyBag sharedPropertyBagCore;
        public BaseInteractionModule() {
            sharedPropertyBagCore = new SharedPropertyBag(this);
        }
        public IInteractionServiceProvider ServiceProvider {
            get { return interactionServiceProviderCore; }
            private set { interactionServiceProviderCore = value; }
        }
        IInteractionServiceProvider IInteractionModule.ServiceProvider {
            get { return ServiceProvider; }
            set { ServiceProvider = value; }
        }
        ISharedPropertyBag IInteractionModule.SharedProperties {
            get { return sharedPropertyBagCore; }
        }
        public abstract void Initialize();
        public abstract void Launch();
        public abstract void Stop();
        public abstract void Dispose();
    }
}
