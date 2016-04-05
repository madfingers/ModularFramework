using System.Linq;

namespace ModularFramework.Implementers {
    public class ModuleManagerController : IModuleHostController {
        IModuleHost ownerCore;
        public ModuleManagerController(IModuleHost owner) {
            ownerCore = owner;
        }
        protected internal virtual IModuleHost Owner { get { return ownerCore; } }
        public bool LoadModule(string pathToModuleDirectory, string moduleName) {
            IModuleMetadataProvider provider = CreatePluginMetadataProvider(pathToModuleDirectory);
            return LoadModule(provider, moduleName);
        }
        public bool LoadModule(IModuleMetadataProvider provider, string moduleName) {
            provider.PopulateMetadata();
            IModuleMetadata targetModuleMetadata = provider.ModuleMetadataSet.FirstOrDefault(md => md.Manifest.Name == moduleName);
            return targetModuleMetadata != null ? Owner.ModuleLoader.Load(targetModuleMetadata) : false;
        }
        public bool LoadModules(IModuleMetadataProvider provider) {
            provider.PopulateMetadata();
            int sourceCount = Owner.Modules.Count();
            foreach(IModuleMetadata moduleMetadata in provider.ModuleMetadataSet) {
                Owner.ModuleLoader.Load(moduleMetadata);
            }
            return Owner.Modules.Count() > sourceCount;
        }
        public bool LoadModules(string pathToDirectory) {
            IModuleMetadataProvider provider = CreatePluginMetadataProvider(pathToDirectory);
            return LoadModules(provider);
        }
        public bool LoadAndLaunchModules(IModuleMetadataProvider provider) {
            bool loadIsSuccessful = LoadModules(provider);
            if(loadIsSuccessful)
                Owner.ModuleLauncher.LaunchModules();
            return loadIsSuccessful;
        }
        public bool LoadAndLaunchModules(string pathToDirectory) {
            bool loadIsSuccessful = LoadModules(pathToDirectory);
            if(loadIsSuccessful)
                Owner.ModuleLauncher.LaunchModules();
            return loadIsSuccessful;
        }
        public bool UnloadModule(IModuleInfo moduleInfo) {
            return Owner.ModuleLoader.Unload(moduleInfo);
        }
        protected virtual IModuleMetadataProvider CreatePluginMetadataProvider(string pathToDirectory) {
            return new ModuleMetadataProvider(pathToDirectory, Owner.ModuleFileExtension);
        }
    }
}
