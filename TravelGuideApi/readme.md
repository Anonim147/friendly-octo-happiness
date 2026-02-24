# Travel Buddy

## Overview
A brief description of the application

## Diagram
### Example sequence diagram
::: mermaid
sequenceDiagram
    title Example: Web Application Request-Response Flow
    participant User
    participant Browser
    participant WebServer
    participant AppLogic
    participant Database

    User->>Browser: Request a page
    Browser->>WebServer: Sends HTTP request
    WebServer->>AppLogic: Process request
    AppLogic->>Database: Query data
    Database-->>AppLogic: Return data
    AppLogic-->>WebServer: Generate response
    WebServer-->>Browser: Send HTML/CSS/JS
    Browser-->>User: Render page
:::

### Example class diargam
::: mermaid
classDiagram
    class User {
        +clicksButtons()
        +entersData()
    }
    class Browser {
        +rendersHTML()
        +executesJS()
    }
    class WebServer {
        +handlesRequests()
        +servesStaticFiles()
    }
    class AppLogic {
        +processesBusinessLogic()
        +interactsWithDatabase()
    }
    class Database {
        +storesData()
        +executesQueries()
    }

    User --> Browser
    Browser --> WebServer
    WebServer --> AppLogic
    AppLogic --> Database
:::

### More Examples here
- [Mermaid Syntax](https://learn.microsoft.com/en-us/azure/devops/project/wiki/markdown-guidance?view=azure-devops#add-mermaid-diagrams-to-a-wiki-page)
- [Mermaid Live Editor](https://mermaid.live/edit)

## Configuration Files
- [app.info.yaml](./Docs/app.info.yaml) - Application information details.
- [docker.settings.yml](./Build/docker.settings.yml) - Docker configuration.
- [deployment.settings.yaml](./Build/helm-chart/deployment.settings.yaml) - Deployment settings for Kubernetes.
- [values.yaml](./Build/helm-chart/values.yaml) - Helm chart configuration settings.

## Development
### Prerequisites
* .NET 8
* powershell
* helm

### How to run locally

## Troubleshooting

## Related Documents
