# JLLG-Mermaid


Generate an application that follows this mermaid diagram:

flowchart LR
    %% Actors
    User([Web User])
 
    %% Frontend
    User --> ReactApp[React Web Application]
 
    %% Auth
    ReactApp --> Auth0[(Auth0)]
    APIGW --> Auth0
    SvcA --> Auth0
    SvcB --> Auth0
    SvcC --> Auth0
 
    %% Core App Architecture
    ReactApp --> APIGW[API Gateway<br/>]
 
    APIGW --> SvcA[Microservice A]
    APIGW --> SvcB[Microservice B]
    APIGW --> SvcC[Microservice C]
 
    SvcA --> DB1[(MongoDB A)]
    SvcB --> DB2[(MongoDB B)]
    SvcC --> DB3[(MongoDB C)]