using System.IO;

namespace ModularFramework {
    public static class FileInfoExtension {
        public static string GetFileNameWithoutExtension(this FileInfo fileInfo) {
            return GetFileNameWithoutExtension(fileInfo.Name);
        }
        public static string GetFileNameWithoutExtension(string fileName) {
            return fileName.Substring(0, fileName.LastIndexOf('.'));
        }
        public static void CopyTo(this FileSystemInfo fileSystemInfo, string destinationPath, bool overwrite) {
            if(fileSystemInfo is FileInfo) {
                FileInfo fileInfo = fileSystemInfo as FileInfo;
                fileInfo.CopyTo(destinationPath, overwrite);
            }
            else {
                DirectoryInfo dirInfo = fileSystemInfo as DirectoryInfo;
                FileSystemInfo[] dirElements = dirInfo.GetFileSystemInfos();
                if(Directory.Exists(destinationPath)) {
                    if(overwrite)
                        Directory.Delete(destinationPath, true);
                    else
                        throw new IOException("file or directory already exists");
                }
                DirectoryInfo targetDirInfo = Directory.CreateDirectory(destinationPath);
                foreach(var dirElement in dirElements) {
                    string copyPath = Path.Combine(destinationPath, dirElement.Name);
                    dirElement.CopyTo(copyPath, overwrite);
                }
            }
        }
    }
}