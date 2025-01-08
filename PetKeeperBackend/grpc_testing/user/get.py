import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from create import users, get_user_ids
ids = get_user_ids()

pass
channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.UserServiceStub(channel)


get_reqs = [
    user_pb2.UserGet(user_id=user_pb2.UserIdentifier(email=users[0]['email'])),
    user_pb2.UserGet(user_id=user_pb2.UserIdentifier(username=users[1]['username'])),
    user_pb2.UserGet(user_id=user_pb2.UserIdentifier(id=ids[2]))
]

for usr, get_req in zip(users, get_reqs):
    token = get_token(usr['email'], usr['password'], channel=channel)
    get_response = stub.GetUser(get_req, metadata=[("authorization", f"Bearer {token}")])
    print("GetUser Response:\n", get_response)
