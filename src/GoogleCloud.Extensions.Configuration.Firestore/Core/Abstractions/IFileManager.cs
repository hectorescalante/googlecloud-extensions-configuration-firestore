namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Abstractions
{
  public interface IFileManager
  {
    string GetFileContent(string path);
  }
}
