using System.Collections.Generic;
using System.Linq;

namespace ModularFramework.Implementers {
    public class ModuleLauncher : IModuleLauncher {
        IModuleHost ownerCore;
        public ModuleLauncher(IModuleHost owner) {
            ownerCore = owner;
        }
        protected IModuleHost Owner { get { return ownerCore; } }
        public void LaunchModules() {
            var pendingModules = new List<IModuleInfo>(Owner.Modules);
            while(pendingModules.Any()) {
                var launchReadyModules = pendingModules.Where(m => CanLaunchModule(m, pendingModules)).ToList();
                foreach(ModuleInfo module in launchReadyModules)
                    LaunchModule(module, pendingModules);
            }
        }
        bool LaunchModule(IModuleInfo module, List<IModuleInfo> pendingModules) {
            if(!pendingModules.Contains(module)) return false;
            if(!CanLaunchModule(module, pendingModules)) return false;
            bool requireModulesLoaded = true;
            foreach(string requireModuleName in module.ModuleMetadata.Manifest.Requires) {
                IModuleInfo requireModule = pendingModules.FirstOrDefault(m => m.ModuleMetadata.Manifest.Name == requireModuleName);
                if(requireModule != null) {
                    bool result = LaunchModule(requireModule, pendingModules);
                    requireModulesLoaded = requireModulesLoaded ? result : requireModulesLoaded;
                }
            }
            if(!requireModulesLoaded) return false;
            module.Owner.Launch();
            pendingModules.Remove(module);
            List<IModuleInfo> afterModules = pendingModules.Where(m => m.ModuleMetadata.Manifest.After.Contains(module.ModuleMetadata.Manifest.Name)).ToList();
            foreach(IModuleInfo afterModule in afterModules) {
                LaunchModule(afterModule, pendingModules);
            }
            return true;
        }
        bool CanLaunchModule(IModuleInfo module, List<IModuleInfo> pendingModules) {
            return module.ModuleMetadata.Manifest.After.All(mn => pendingModules.FirstOrDefault(m => m.ModuleMetadata.Manifest.Name == mn) == null);
        }
    }
}
