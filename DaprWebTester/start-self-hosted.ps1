dapr run `
    --app-id daprwebtester `
    --app-port 5150 `
    --dapr-http-port 3500 `
    --dapr-grpc-port 50001 `
    --components-path ../SupportFiles/dapr/components `
    dotnet watch