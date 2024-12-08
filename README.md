# Usage
Postgresql docker container is required for the API to work.
Use the following command to run postgresql container:

`docker run --name postgres -e POSTGRES_USER=user -e POSTGRES_PASSWORD=pass -e POSTGRES_DB=testdb -p 5432:5432 -v "C:\Users\batru\psql\pg_init:/docker-entrypoint-initdb.d" -d postgres`

Change the path to `init.sql` accordingly.
To setup the connection to the database, first - use the following command to get the IP address of the postgres container:

`docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' postgres`

Change the database IP in `appsettings.json`

Run the solution using the Container (Dockerfile) mode.

# gRPC interaction and tests
Create the .venv and install `grpcio` module

Compile stub files by using the command below. This is also required when the .proto file is updated. Change the path to `user.proto` accordingly

`python -m grpc_tools.protoc -I=protos --python_out=. --grpc_python_out=. protos/user.proto`


