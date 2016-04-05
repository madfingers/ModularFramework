using System;
using System.Collections.Generic;
using ModularFramework.Implementers.Services;

namespace ModularFramework.Implementers {
    public class ModuleManager : MarshalByRefObject, IModuleHost, IDisposable {
        IModuleLoader loaderCore;
        IModuleLauncher launcherCore;
        IModuleHostController controllerCore;
        IList<IModuleInfo> moduleInfosCore;
        IDictionary<Type, IInteractionService> servicesCore;
        public ModuleManager() {
            servicesCore = CreateServiceCollection();
            moduleInfosCore = CreateModuleList();
            loaderCore = CreateModuleLoader();
            controllerCore = CreateController();
            launcherCore = CreateLauncher();
            SubscribeOnEvents();
        }
        public virtual string ModuleFileExtension { get { return ".module"; } }
        protected virtual IList<IModuleInfo> CreateModuleList() {
            return new List<IModuleInfo>();
        }
        protected virtual IModuleLoader CreateModuleLoader() {
            return new ModuleLoader();
        }
        protected virtual IModuleLauncher CreateLauncher() {
            return new ModuleLauncher(this);
        }
        protected virtual IDictionary<Type, IInteractionService> CreateServiceCollection() {
            IDictionary<Type, IInteractionService> collection = new Dictionary<Type, IInteractionService>();
            collection.Add(typeof(InteractionModuleProviderService), new InteractionModuleProviderService(this));
            return collection;
        }
        protected virtual IModuleHostController CreateController() {
            return new ModuleManagerController(this);
        }
        protected virtual void SubscribeOnEvents() {
            ModuleLoader.ModuleLoaded += OnModuleLoaded;
            ModuleLoader.ModuleUnloaded += OnModuleUnloaded;
        }
        protected virtual void UnsubscribeOfEvents() {
            ModuleLoader.ModuleLoaded -= OnModuleLoaded;
            ModuleLoader.ModuleUnloaded -= OnModuleUnloaded;
        }
        private void OnModuleUnloaded(object sender, ModuleUnloadedEventArgs ea) {
            moduleInfosCore.Remove(ea.ModuleInfo);
            if(ea.ModuleInfo.Owner is IInteractionModule)
                (ea.ModuleInfo.Owner as IInteractionModule).ServiceProvider = null;
            ea.ModuleInfo.Owner.Dispose();
        }
        protected virtual void OnModuleLoaded(object sender, ModuleLoadedEventArgs ea) {
            if(ea.ModuleInfo.Owner is IInteractionModule)
                (ea.ModuleInfo.Owner as IInteractionModule).ServiceProvider = this;
            moduleInfosCore.Add(ea.ModuleInfo);
            ea.ModuleInfo.Owner.Initialize();
        }
        public IModuleLauncher ModuleLauncher { get { return launcherCore; } }
        public IModuleHostController Controller { get { return controllerCore; } }
        public IEnumerable<IModuleInfo> Modules { get { return moduleInfosCore; } }
        protected internal IModuleLoader ModuleLoader { get { return loaderCore; } }
        public bool RegisterService<T>() where T : class, IInteractionService, new() {
            Type serviceType = typeof(T);
            if(servicesCore.ContainsKey(serviceType))
                return false;
            T serviceInstance = null;
            if(serviceType.IsSubclassOf(typeof(IStateBasedInteractionService)))
                serviceInstance = new T();
            servicesCore.Add(typeof(T), serviceInstance);
            return true;
        }
        public bool UnregisterService<T>() where T : class, IInteractionService, new() {
            Type serviceType = typeof(T);
            if(!servicesCore.ContainsKey(serviceType))
                return false;
            return servicesCore.Remove(serviceType);
        }
        public T GetService<T>() where T : class, IInteractionService, new() {
            Type serviceType = typeof(T);
            if(!servicesCore.ContainsKey(serviceType))
                return null;
            T seviceInstance = servicesCore[serviceType] as T;
            if(seviceInstance == null)
                seviceInstance = new T();
            return seviceInstance;
        }
        #region IPluginHost Support
        IModuleLoader IModuleHost.ModuleLoader {
            get { return ModuleLoader; }
        }
        #endregion
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    UnsubscribeOfEvents();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PluginManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}