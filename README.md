# Usage
## Debug configuration
Postgresql docker container is required for the API to work.
Use the following command to run postgresql container:

`docker run --name postgres -e POSTGRES_USER=user -e POSTGRES_PASSWORD=pass -e POSTGRES_DB=testdb -p 5432:5432 -v "C:\Users\batru\psql\pg_init:/docker-entrypoint-initdb.d" -d postgres`

Change the path to `init.sql` accordingly.
Database connection is set by container arguments hostname mapping and should work if database is launched before the solution, and default docker bridge subnet is used. If this is not the case, change the mapping in `Properties/LaunchSettings.json`

Run the solution using the Container (Dockerfile) mode.

## Release
To run release configuration, simply run compose from the project root:

`docker compose up`

# gRPC interaction and tests
Create the .venv and install `grpcio` module

Compile stub files by using the command below. This is also required when the .proto file is updated. Change the path to `user.proto` accordingly

`python -m grpc_tools.protoc -I=protos --python_out=. --grpc_python_out=. protos/user.proto`
