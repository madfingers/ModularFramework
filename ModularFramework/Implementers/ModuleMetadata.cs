using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace ModularFramework.Implementers {
    [DataContract]
    public class ModuleManifest : IModuleManifest {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Version Version { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string CompanyInfo { get; set; }
        [DataMember]
        public IEnumerable<string> Dependencies { get; set; }
        [DataMember]
        public string TargetTypeAssembly { get; set; }
        [DataMember]
        public string TargetTypeFullName { get; set; }
        [DataMember]
        public IEnumerable<string> After { get; set; }
        [DataMember]
        public IEnumerable<string> Requires { get; set; }
    }
    public class ModuleMetadata : IModuleMetadata {
        IModuleManifest manifestCore;
        DirectoryInfo moduleDirectoryCore;
        public ModuleMetadata(IModuleManifest manifest, DirectoryInfo moduleDirectory) {
            manifestCore = manifest;
            moduleDirectoryCore = moduleDirectory;
        }
        public IModuleManifest Manifest {
            get { return manifestCore; }
        }
        public DirectoryInfo ModuleDirectory {
            get { return moduleDirectoryCore; }
        }
    }
    public class ModuleMetadataProvider : IModuleMetadataProvider {
        string moduleFileExtensionCore;
        DirectoryInfo initialModulesDirectory;
        DirectoryInfo tmpDirForUnpackedModules;
        IList<IModuleMetadata> moduleMetadataSetCore;
        public ModuleMetadataProvider(string pathToModulesDir, string moduleFileExtension) {
            moduleFileExtensionCore = moduleFileExtension;
            initialModulesDirectory = new DirectoryInfo(pathToModulesDir);
            CreateModuleSetInstance();
        }
        public IEnumerable<IModuleMetadata> ModuleMetadataSet {
            get { return moduleMetadataSetCore; }
        }
        public void Dispose() { }
        public void PopulateMetadata() {
            CreateTemporaryDirectoryForModules();
            PopulateModulesMetadata();
        }
        protected virtual void CreateModuleSetInstance() {
            moduleMetadataSetCore = new List<IModuleMetadata>();
        }
        protected virtual void CreateTemporaryDirectoryForModules() {
            tmpDirForUnpackedModules = new DirectoryInfo(Path.GetTempPath());
        }
        protected virtual void PopulateModulesMetadata() {
            var moduleCompressedFiles = initialModulesDirectory.GetFiles().Where(f => f.Extension == moduleFileExtensionCore);
            foreach(var moduleCompressedFile in moduleCompressedFiles) {
                DirectoryInfo unpackedModuleDir = ExtractCompressedModuleToTemporaryDir(moduleCompressedFile);
                IModuleMetadata moduleMetadata;
                if(TryGetModuleMetadata(unpackedModuleDir, out moduleMetadata)) {
                    moduleMetadataSetCore.Add(moduleMetadata);
                }
            }
        }
        protected virtual DirectoryInfo ExtractCompressedModuleToTemporaryDir(FileInfo moduleFile) {
            string moduleDirName = String.Format("{0}.{1}", moduleFile.GetFileNameWithoutExtension(), Guid.NewGuid());
            DirectoryInfo moduleDir = Directory.CreateDirectory(Path.Combine(tmpDirForUnpackedModules.FullName, moduleDirName));
            ZipFile.ExtractToDirectory(moduleFile.FullName, moduleDir.FullName);
            return moduleDir;
        }
        protected virtual bool TryGetModuleMetadata(DirectoryInfo unpackedModuleDir, out IModuleMetadata moduleMetadata) {
            string moduleManifestFile = unpackedModuleDir.GetFiles().SingleOrDefault(f => f.Name == "manifest.json").FullName;
            IModuleManifest moduleManifest = SerializationHelper.DeserializeFrom<ModuleManifest>(moduleManifestFile);
            moduleMetadata = moduleManifest != null ? CreateModuleMetadataInstance(moduleManifest, unpackedModuleDir) : null;
            return moduleMetadata != null;
        }
        protected virtual IModuleMetadata CreateModuleMetadataInstance(IModuleManifest manifest, DirectoryInfo dir) {
            return new ModuleMetadata(manifest, dir);
        }
    }
}
