using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ModularFramework {
    public interface IModuleInfo {
        AppDomain Domain { get; }
        IModuleMetadata ModuleMetadata { get; }
        IModule Owner { get; }
    }
    public interface IModule : IDisposable {
        void Initialize();
        void Launch();
        void Stop();
    }
    public interface IInteractionModule : IModule {
        ISharedPropertyBag SharedProperties { get; }
        IInteractionServiceProvider ServiceProvider { get; set; }
    }
    public interface IInteractionService { }
    public interface IStateBasedInteractionService : IInteractionService {
        ServiceState State { get; set; }
        void Initialize();
        void Reset();
    }
    public interface IInteractionServiceProvider {
        T GetService<T>() where T : class, IInteractionService, new();
    }
    public interface IInteractionServiceRegistrator {
        bool RegisterService<T>() where T : class, IInteractionService, new();
        bool UnregisterService<T>() where T : class, IInteractionService, new();
    }
    //
    public interface IModuleHost : IInteractionServiceProvider, IInteractionServiceRegistrator {
        IModuleLoader ModuleLoader { get; }
        IModuleLauncher ModuleLauncher { get; }
        IEnumerable<IModuleInfo> Modules { get; }
        IModuleHostController Controller { get; }
        string ModuleFileExtension { get; }
    }
    public interface IModuleHostController {
        bool LoadModule(IModuleMetadataProvider provider, string moduleName);
        bool LoadModule(string directory, string moduleName);
        bool LoadModules(IModuleMetadataProvider provider);
        bool LoadModules(string directory);
        bool LoadAndLaunchModules(string directory);
        bool LoadAndLaunchModules(IModuleMetadataProvider provider);
        bool UnloadModule(IModuleInfo moduleInfo);
    }
    public interface IModuleLoader {
        event ModuleLoadedEventHandler ModuleLoaded;
        event ModuleUnloadedEventHandler ModuleUnloaded;
        bool Load(IModuleMetadata moduleProvider);
        bool Unload(IModuleInfo module);
    }
    //
    public interface IModuleManifest {
        [DataMember]
        string Name { get; set; }
        [DataMember]
        Version Version { get; set; }
        [DataMember]
        string Description { get; set; }
        [DataMember]
        string CompanyName { get; set; }
        [DataMember]
        string CompanyInfo { get; set; }
        [DataMember]
        string TargetTypeFullName { get; set; }
        [DataMember]
        string TargetTypeAssembly { get; set; }
        [DataMember]
        IEnumerable<string> Dependencies { get; set; }
        [DataMember]
        IEnumerable<string> After { get; set; }
        [DataMember]
        IEnumerable<string> Requires { get; set; }
    }
    public interface IModuleMetadata {
        IModuleManifest Manifest { get; }
        DirectoryInfo ModuleDirectory { get; }
    }
    public interface IModuleMetadataProvider : IDisposable {
        void PopulateMetadata();
        IEnumerable<IModuleMetadata> ModuleMetadataSet { get; }
    }
    public interface ISharedPropertyBag {
        T GetProperty<T>(string name);
        T GetProperty<T>(string name, object[] index);
        void SetProperty<T>(string name, T value);
        void SetProperty<T>(string name, T value, object[] index);
    }
    public interface IModuleLauncher {
        void LaunchModules();
    }
}
