import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from create import users, get_user_ids
from PIL import Image
from io import BytesIO
import os
ids = get_user_ids()

pass
channel = grpc.insecure_channel('localhost:8080', options=[('grpc.max_receive_message_length', 50 * 1024 * 1024)])
stub = user_pb2_grpc.UserServiceStub(channel)


get_reqs = [
    user_pb2.UserGet(user_id=user_pb2.UserIdentifier(email=users[0]['email'])),
    user_pb2.UserGet(user_id=user_pb2.UserIdentifier(username=users[1]['username'])),
    user_pb2.UserGet(user_id=user_pb2.UserIdentifier(id=ids[2]))
]

for usr, get_req in zip(users, get_reqs):
    token = get_token(usr['email'], usr['password'], channel=channel)
    get_response = stub.GetUser(get_req, metadata=[("authorization", f"Bearer {token}")])

    try:
        os.makedirs(f"../images/received/{usr['email']}")
    except FileExistsError:
        pass
    if len(get_response.avatar_png) > 0:
        im = Image.open(BytesIO(get_response.avatar_png))
        im.save(f"../images/received/{usr['email']}/avatar.png")
        get_response.ClearField("avatar_png")

    im = Image.open(BytesIO(get_response.document_pngs[0]))
    im.save(f"../images/received/{usr['email']}/documentFront.png")
    im = Image.open(BytesIO(get_response.document_pngs[1]))
    im.save(f"../images/received/{usr['email']}/documentBack.png")
    get_response.ClearField("document_pngs")

    print("GetUser Response:\n", get_response)
