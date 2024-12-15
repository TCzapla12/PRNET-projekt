import grpc
import user_pb2
import user_pb2_grpc

def get_token(email, password, *, channel):
    stub = user_pb2_grpc.AuthServiceStub(channel)
    id = user_pb2.UserIdentifier()
    id.email = email
    get_request = user_pb2.AuthRequest(
        user_id=id,
        password=password
    )
    get_response = stub.Authenticate(get_request)
    return get_response.token


if __name__ == "__main__":
    channel = grpc.insecure_channel('localhost:8080')
    get_token("new_user22222@example.com", "securepassword123", channel=channel)
