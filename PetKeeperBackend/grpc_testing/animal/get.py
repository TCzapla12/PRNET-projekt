import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
from PIL import Image
from io import BytesIO
import os

user_ids = get_user_ids()

channel = grpc.insecure_channel('localhost:8080', options=[('grpc.max_receive_message_length', 50 * 1024 * 1024)])
stub = user_pb2_grpc.AnimalServiceStub(channel)

get_requests = [
    user_pb2.AnimalGet(  # Either name or type here
        #name='Mruczek',
        type='cat',
    ),
    user_pb2.AnimalGet(owner_id=''),  # Should get all animal of user[1] (self)
    user_pb2.AnimalGet(owner_id=user_ids[0]),  # Check if user[2] can get user[0] animals
]

# The zip makes the last request be made by the users[0]
for user, user_id, request in zip(users + [users[0]], user_ids + [user_ids[0]], get_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    get_response = stub.GetAnimals(request, metadata=[("authorization", f"Bearer {token}")])
    for animal in get_response.animals:
        try:
            os.makedirs(f"../images/received/{users[user_ids.index(animal.owner_id)]['email']}/{animal.name}")
        except FileExistsError:
            pass
        im = Image.open(BytesIO(animal.photo))
        im.save(f"../images/received/{users[user_ids.index(animal.owner_id)]['email']}/{animal.name}/photo.png")
        animal.ClearField("photo")

    print("AnimalGet Response:\n", get_response, "\n" + "-"*100, "\n\n")
