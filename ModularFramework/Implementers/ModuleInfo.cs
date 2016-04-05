using System;

namespace ModularFramework.Implementers {
    public class ModuleInfo : IModuleInfo {
        AppDomain domainCore;
        IModule ownerCore;
        IModuleMetadata metadataCore;
        public ModuleInfo(AppDomain domain, IModule owner, IModuleMetadata metadata) {
            domainCore = domain;
            ownerCore = owner;
            metadataCore = metadata;
        }
        public AppDomain Domain { get { return domainCore; } }
        public IModule Owner { get { return ownerCore; } }
        public IModuleMetadata ModuleMetadata { get { return metadataCore; } }
    }
}
