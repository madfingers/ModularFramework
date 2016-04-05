using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace ModularFramework.Implementers {
    public static class ModuleCreator {
        public static void Create(string sourceDirectoryPath, string targetDirectoryPath, ModuleManifest manifest) {
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
        static string ChooseModuleFileName(ModuleManifest manifest) {
            return manifest.Name ?? FileInfoExtension.GetFileNameWithoutExtension(manifest.TargetTypeAssembly);
        }
        static void CreateManifestFile(string targetDirectoryPath, ModuleManifest manifest) {
            string manifestPath = Path.Combine(targetDirectoryPath, "manifest.json");
            using(Stream fileStream = File.Create(manifestPath)) {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ModuleManifest));
                serializer.WriteObject(fileStream, manifest);
            }
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
