import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from create import users, get_user_ids
ids = get_user_ids()

pass
channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.UserServiceStub(channel)


del_reqs = [
    user_pb2.UserIdentifier(id=ids[0]),
    user_pb2.UserIdentifier(email=users[1]['email']),
    user_pb2.UserIdentifier(username=users[2]['username'])
]

for usr, del_req in zip(users, del_reqs):
    token = get_token(usr['email'], usr['password'], channel=channel)
    get_response = stub.DeleteUser(del_req, metadata=[("authorization", f"Bearer {token}")])
    print("DeleteUser Response:\n", get_response)
