# Deployment Resources

- deployment-version
  - used to add specific version during build and showed in swagger or frond-end apps
  - example: 0.9.225
```c#
// Version information for an assembly consists of the following four values:
// {Major Version}.{Minor Version}.{Build Number}
```
---
## How to use it in `Github Actions`
### TODO

---
---
## How to use it in `Azure DevOps`
### TODO

---
## Folder is default for `Jenkins` if use it for builds and deployment 

### How to use it in your `Jenkinsfile` 

```Groovy
pipeline {
 agent any
    environment {
        // Version is defined in external file in folder resources
        PROJECT_VERSION =  libraryResource 'deployment-version'
     }
    stages {

        stage('Set Build Version') {
            steps {                
                script{                    
                    currentBuild.displayName = "${PROJECT_VERSION}"
                }
            }            
        }
        stage('Publish NodeJS Project') {
            steps {
                dir('src/frond-end'){
                        bat 'npm version ' + NPM_VERSION
                        bat 'npm install'
                        bat 'npm run build'
                }
            }
        }
        stage('Publish .NET Project') {
            steps {                
                dir('src/back-end'){
                    bat 'dotnet publish CoinGarden-World-Full.sln -c Release -o ../../Publish/Release /P:AssemblyVersion='+ PROJECT_VERSION +' /P:Version='+ PROJECT_VERSION 
                }
            }
        }

        stage('Get Artifacts') {
            steps {
                zip zipFile: 'Projects.zip', archive: false, dir: 'Publish/Release'                                  
                archiveArtifacts artifacts: 'Projects.zip', fingerprint: true
            }        
        }
				
    }
}
```