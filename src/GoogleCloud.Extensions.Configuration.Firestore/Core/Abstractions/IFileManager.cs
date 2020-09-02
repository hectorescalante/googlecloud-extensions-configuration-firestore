namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Abstractions
{
  internal interface IFileManager
  {
    string GetFileContent(string path);
  }
}
