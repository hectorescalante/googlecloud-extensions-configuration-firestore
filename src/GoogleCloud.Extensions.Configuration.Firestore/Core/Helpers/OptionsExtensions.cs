using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers
{
  internal static class OptionsExtensions
  {
    public static void Bind(this FirestoreOptions options)
    {
      options.SetProjectId();
      options.SetApplicationName();
      options.SetReleaseStage();
      options.SetTag();
      options.SettingsCollection = "ApplicationSettings";
      options.StagesCollection = "Stages";
      options.TagsCollection = "Tags";
      options.SettingsFileName = "appsettings.json";
    }

    public static bool IsEnabled(this FirestoreOptions options) =>
      options.Enabled = bool.Parse(Environment.GetEnvironmentVariable("FIRESTORECONFIG_ENABLED") ?? "true");
    public static string SetProjectId(this FirestoreOptions options) =>
      options.ProjectId = Environment.GetEnvironmentVariable("FIRESTORECONFIG_PROJECTID") ?? throw new ArgumentNullException("ProjectId");
    public static string SetApplicationName(this FirestoreOptions options) =>
      options.ApplicationName = Environment.GetEnvironmentVariable("FIRESTORECONFIG_APPLICATION") ?? AppDomain.CurrentDomain.FriendlyName;
    public static string SetReleaseStage(this FirestoreOptions options) =>
      options.ReleaseStage = Environment.GetEnvironmentVariable("FIRESTORECONFIG_STAGE") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new ArgumentNullException("Environment");
    public static string SetTag(this FirestoreOptions options) =>
      options.Tag = Environment.GetEnvironmentVariable("FIRESTORECONFIG_TAG") ?? "Default";


    public static string GetApplicationDocumentPath(this FirestoreOptions options) => 
      $"{options.SettingsCollection}/{options.ApplicationName}";
    public static string GetStageDocumentPath(this FirestoreOptions options) => 
      $"{options.SettingsCollection}/{options.ApplicationName}/{options.StagesCollection}/{options.ReleaseStage}";
    public static string GetTagDocumentPath(this FirestoreOptions options) => 
      $"{options.SettingsCollection}/{options.ApplicationName}/{options.StagesCollection}/{options.ReleaseStage}/{options.TagsCollection}/{options.Tag}";

  }
}
