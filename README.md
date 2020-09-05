# GoogleCloud.Extensions.Configuration.Firestore
.Net Core Configuration Provider that allows developers to use Google Cloud Firestore as a Configuration Source in their applications including support for secrets stored in Google Cloud SecretManager.

[![Build status](https://dev.azure.com/hectorescalante/Github%20Projects/_apis/build/status/GoogleCloud.Extensions.Configuration.Firestore)](https://dev.azure.com/hectorescalante/Github%20Projects/_build/latest?definitionId=7)

## FirestoreOptions

| Setting | Default Value | Description |
| ------- | ------------- | ----------- |
| FIRESTORECONFIG_ENABLED | true | Enable or disable configuration load |
| FIRESTORECONFIG_PROJECTID | "" | The google cloud project identifier where the firestore service exists |
| FIRESTORECONFIG_APPLICATION | AppDomain.CurrentDomain.FriendlyName | Name of the application  |
| FIRESTORECONFIG_STAGE | ASPNETCORE_ENVIRONMENT | Name of the current environment  |
| FIRESTORECONFIG_TAG | "Default" | Useful for application versioning, blue-green deployment or any other stage subclassification |

## Add Firestore Configuration in Program class

```
  public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
              config.AddFirestoreConfiguration();
            })            
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
```

## Automatic Settings Upload
For the first time, this package automatically load the appsettings.json file into a Firestore Document named as the application. 

Also a Stages collection will be created with an empty Document named as the current environment.

## Change Detection
The provider will be listening for changes in these documents:

1. ApplicationSettings/{applicationName}
2. ApplicationSettings/{applicationName}/Stages/{stageName}
3. ApplicationSettings/{applicationName}/Stages/{stageName}/Tags/{tag}

When a change is made in any document, the provider reload the configuration from general to particular (Application -> Stage -> Tag).

## Usage
Since the settings are loaded from Firestore into the Application Configuration they will be available through the IConfiguration interface at Startup, just like the settings declared in environment variables or appsettings.json file.

## Secrets
If any setting has a value begining with 'secret:' then this package will try to resolve the secret value from Google Cloud Secret Manager.

The value should have this structure: *secret:projectId:secretName:secretVersion*

>Alternatively support this structure: **secret::secretName** where the projectId is the same as the Firestore service and the secretVersion is "latest".

## Google Application Credential

Permission Roles

- Firestore: Cloud Datastore User
- SecretManager: Secret Manager Secret Accessor
 