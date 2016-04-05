using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace ModularFramework.Implementers {
    public class ModuleLoader : IModuleLoader {
        public event ModuleLoadedEventHandler ModuleLoaded;
        public event ModuleUnloadedEventHandler ModuleUnloaded;
        protected virtual void RaiseModuleLoadedEvent(ModuleLoadedEventArgs ea) {
            if(ModuleLoaded != null)
                ModuleLoaded(this, ea);
        }
        protected virtual void RaiseModuleUnloadedEvent(ModuleUnloadedEventArgs ea) {
            if(ModuleUnloaded != null)
                ModuleUnloaded(this, ea);
        }
        public bool Load(IModuleMetadata moduleMetadata) {
            AppDomain moduleDomain = CreateDomainForModule(moduleMetadata);
            IModule module = CreateModuleInstanceInDomain(moduleDomain, moduleMetadata);
            IModuleInfo moduleInfo = CreateModuleInfoInstance(moduleDomain, module, moduleMetadata);
            RaiseModuleLoadedEvent(new ModuleLoadedEventArgs(moduleInfo));
            return true;
        }
        protected virtual IModule CreateModuleInstanceInDomain(AppDomain domain, IModuleMetadata moduleMetadata) {
            string pluginAsmPath = Path.Combine(moduleMetadata.ModuleDirectory.FullName, moduleMetadata.Manifest.TargetTypeAssembly); 
            return (IModule)domain.CreateInstanceFromAndUnwrap(pluginAsmPath, moduleMetadata.Manifest.TargetTypeFullName, false, BindingFlags.Default, null, null, CultureInfo.InvariantCulture, null);
        }
        protected virtual AppDomain CreateDomainForModule(IModuleMetadata moduleSource) {
            return AppDomain.CreateDomain(moduleSource.Manifest.Name, GetDomainEvidence(), GetDomainSettings(moduleSource));
        }
        protected virtual IModuleInfo CreateModuleInfoInstance(AppDomain domain, IModule module, IModuleMetadata metadata) {
            return new ModuleInfo(domain, module, metadata);
        }
        protected virtual Evidence GetDomainEvidence() {
            return new Evidence();
        }
        protected virtual AppDomainSetup GetDomainSettings(IModuleMetadata moduleMetadata) {
            AppDomainSetup moduleDomainSetup = new AppDomainSetup();
            moduleDomainSetup.ApplicationBase = moduleMetadata.ModuleDirectory.FullName;
            return moduleDomainSetup;
        }
        public bool Unload(IModuleInfo moduleInfo) {
            moduleInfo.Owner.Stop();
            AppDomain.Unload(moduleInfo.Domain);
            RaiseModuleUnloadedEvent(new ModuleUnloadedEventArgs(moduleInfo));
            return true;
        }
    }
}
