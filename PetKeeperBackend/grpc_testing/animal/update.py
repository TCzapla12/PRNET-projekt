import grpc
import user_pb2
import user_pb2_grpc
import sys
from auth.get import get_token
from user.create import users, get_user_ids
from animal.create import get_animal_ids
from read_image import read_image
animal_ids = get_animal_ids()
user_ids = get_user_ids()


channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AnimalServiceStub(channel)

update_requests = [
    user_pb2.AnimalUpdate(  # Update Fafik
        id=animal_ids[0],
        description='Starszy, mało energetyczny, ale przyjazny i miły psina'
    ),
    user_pb2.AnimalUpdate(  # Try Updating someone else's animal as non-admin (should fail)
        id=animal_ids[2],
        name='Demon',
        description='Uwaga: agresywny do niektórych osób. Zapraszam wpierw na zapoznanie'
    ),
    user_pb2.AnimalUpdate(  # Admin-update animal (Inżynier of user[1])
        id=animal_ids[3],
        photo=read_image('../images/prawdziwy_inzynier.png'),
        description='Grubiutki kot, starsza przylepa. Wymaga dużo głaskania. Ma małe problemy z zębami :('
    ),
]

for user, user_id, update_request in zip(users, user_ids, update_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    try:
        update_response = stub.UpdateAnimal(update_request, metadata=[("authorization", f"Bearer {token}")])
        print("AnimalUpdate Response:\n", update_response)
    except grpc.RpcError as e:
        print(f"Error: {e}", file=sys.stderr)

