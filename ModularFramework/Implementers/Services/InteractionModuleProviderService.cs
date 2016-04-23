using System;
using System.Linq;

namespace ModularFramework.Implementers.Services {
    public class InteractionModuleProviderService : MarshalByRefObject, IStateBasedInteractionService {
        ModuleManager ownerCore;
        public InteractionModuleProviderService() {
        }
        public InteractionModuleProviderService(ModuleManager owner) {
            ownerCore = owner;
        }
        public IInteractionModule this[string name] {
            get {
                IModuleInfo info = ownerCore.Modules.SingleOrDefault(mi => mi.ModuleMetadata.Manifest.Name == name);
                return info?.Owner as IInteractionModule;
            }
        }
        public ServiceState State { get; set; }
        public void Initialize() { }
        public void Reset() { }
    }
}
