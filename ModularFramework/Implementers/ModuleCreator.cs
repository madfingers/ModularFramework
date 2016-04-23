using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace ModularFramework.Implementers {
    public static class ModuleCreator {
        public static void Create(string sourceDirectoryPath, string targetDirectoryPath, string moduleAsmPath) {
            IModuleManifest manifest = CreateManifestFromAssembly(moduleAsmPath);
            Create(sourceDirectoryPath, targetDirectoryPath, manifest);
        }
        public static IModuleManifest CreateManifestFromAssembly(string moduleAsmPath) {
            Assembly asm = Assembly.LoadFile(moduleAsmPath);
            Type moduleType = asm.GetTypes().SingleOrDefault(t => t.BaseType == typeof(BaseInteractionModule));
            var attributes = moduleType.GetCustomAttributes<ModularFrameworkAttribute>();
            IModuleManifest manifest = new ModuleManifest();
            manifest.Name = FindAttribute<ModuleNameAttribute>(attributes).Maybe(x => x.Name, String.Empty);
            manifest.Description = FindAttribute<ModuleDescriptionAttribute>(attributes).Maybe(x => x.Description, String.Empty);
            manifest.Version = FindAttribute<ModuleVersionAttribute>(attributes).Maybe(x => x.Version);
            manifest.CompanyName = FindAttribute<ModuleCompanyNameAttribute>(attributes).Maybe(x => x.CompanyName, String.Empty);
            manifest.CompanyInfo = FindAttribute<ModuleCompanyInfoAttribute>(attributes).Maybe(x => x.CompanyInfo, String.Empty);
            manifest.After = FindAttribute<ModuleLoadAfterAttribute>(attributes).Maybe(x => x.After, new string[] { });
            manifest.Requires = FindAttribute<ModuleRequireAttribute>(attributes).Maybe(x => x.Requires, new string[] { });
            manifest.TargetTypeAssembly = Path.GetFileName(moduleAsmPath);
            manifest.TargetTypeFullName = moduleType.FullName;
            manifest.Dependencies = GetModuleDependencies(asm);
            return manifest;
        }
        static string[] GetModuleDependencies(Assembly moduleAsm) {
            List<string> result = new List<string>();
            result.AddRange(moduleAsm.GetReferencedAssemblies().Select(n => n.Name + ".dll"));
            foreach(var resName in moduleAsm.GetManifestResourceNames()) {
                using(Stream stream = moduleAsm.GetManifestResourceStream(resName))
                using(ResourceReader reader = new ResourceReader(stream)) {
                    foreach(DictionaryEntry entry in reader) {
                        result.Add((string)entry.Key);
                    }
                }
            }
            return result.ToArray();
        }

        static T FindAttribute<T>(IEnumerable<ModularFrameworkAttribute> attributes) where T : ModularFrameworkAttribute {
            return (T)attributes.SingleOrDefault(a => a.GetType() == typeof(T));
        }
        public static void Create(string sourceDirectoryPath, string targetDirectoryPath, IModuleManifest manifest) {
            string tmpDirectoryPath = CreateTemporaryDirectory(sourceDirectoryPath);
            CreateManifestFile(tmpDirectoryPath, manifest);
            CopyTargetAssemblyFrom(sourceDirectoryPath, tmpDirectoryPath, manifest.TargetTypeAssembly);
            CopyDependenciesFrom(sourceDirectoryPath, tmpDirectoryPath, manifest.Dependencies);
            CompressModuleFilesFrom(tmpDirectoryPath, targetDirectoryPath, ChooseModuleFileName(manifest));
            Directory.Delete(tmpDirectoryPath, true);
        }
        static void CompressModuleFilesFrom(string sourceDirectoryPath, string targetDirectoryPath, string moduleName) {
            string moduleFilePath = Path.Combine(targetDirectoryPath, moduleName + ".module");
            if(File.Exists(moduleFilePath))
                File.Delete(moduleFilePath);
            ZipFile.CreateFromDirectory(sourceDirectoryPath, moduleFilePath);
        }
        static string CreateTemporaryDirectory(string sourceDirPath) {
            DirectoryInfo tmpDir = Directory.CreateDirectory(Path.Combine(sourceDirPath, ".tmp"));
            tmpDir.Attributes |= FileAttributes.Hidden;
            return tmpDir.FullName;
        }
        static string ChooseModuleFileName(IModuleManifest manifest) {
            return manifest.Name ?? FileInfoExtension.GetFileNameWithoutExtension(manifest.TargetTypeAssembly);
        }
        static void CreateManifestFile(string targetDirectoryPath, IModuleManifest manifest) {
            string manifestPath = Path.Combine(targetDirectoryPath, "manifest.json");
            SerializationHelper.SerializeTo(manifestPath, manifest);
        }
        static void CopyTargetAssemblyFrom(string sourceDirPath, string targetDirPath, string assemblyFileName) {
            FileInfo file = new FileInfo(Path.Combine(sourceDirPath, assemblyFileName));
            file.CopyTo(Path.Combine(targetDirPath, file.Name), true);
        }
        static void CopyDependenciesFrom(string sourceDirPath, string targetDirPath, IEnumerable<string> dependencies) {
            if(dependencies == null) return;
            DirectoryInfo dirInfo = new DirectoryInfo(sourceDirPath);
            IEnumerable<FileSystemInfo> fileSystemInfos = dirInfo.GetFileSystemInfos().Where(info => dependencies.Contains(info.Name));
            foreach(var fileSystemInfo in fileSystemInfos) {
                string pathForCopy = Path.Combine(targetDirPath, fileSystemInfo.Name);
                fileSystemInfo.CopyTo(pathForCopy, true);
            }
        }
    }
}
