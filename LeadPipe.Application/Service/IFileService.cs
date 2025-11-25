using System.Runtime.CompilerServices;

namespace LeadPipe.Application.Service;

public interface IFileService
{
    string GetLocalFile(string projectContainingLocalFolder, string localFolderToFind, string fileName);
    string GetLocalFolder(string projectContainingLocalFolder, string localFolderToFind);
    string GetMemberName(object origin, [CallerMemberName] string memberName = "");
}