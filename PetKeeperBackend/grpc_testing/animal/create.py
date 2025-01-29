import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
from read_image import read_image
import os
script_dir = os.path.dirname(os.path.abspath(__file__))
user_ids = get_user_ids()


channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AnimalServiceStub(channel)

animals = [
    {
        'name': 'Fafik',
        'type': 'dog',
        'description': 'Przyjazny, normalny pies :)',
        'photo': read_image('../images/dziwny_pies.png'),
        # 'owner_id': user_ids[0]
    },
    {    # Will actually be owned by user[1]. owner_id should be ignored
        'name': 'Iker',
        'type': 'other',
        'description': 'Gekon, lubi przyjaciół w terrarium',
        'photo': read_image('../images/gekon.png'),
        'owner_id': user_ids[2]    # Try creating for a different user as non-admin. Field should be ignored
    },
    {
        'name': 'Mruczka',
        'type': 'cat',
        'description': 'Spokojny, milutki kot',
        'photo': read_image('../images/kot1.png'),
        # 'owner_id': user_ids[0]
    },
    {
        'name': 'Inżynier',
        'type': 'cat',
        'description': 'Gruby kot. Wymagający dużo jedzenia i głaskania. Starszy',
        'photo': read_image('../images/kot2.png'),
        'owner_id': user_ids[1]  # Create for user[1] as admin user[2]
    }
]


def get_animal_ids():
    with open(os.path.join(script_dir, 'animal_ids.txt'), 'r') as f:
        return [line.strip() for line in f.readlines()]


if __name__ == "__main__":
    ids = []
    for usr_id, usr, animal in zip(user_ids + [user_ids[2]], users + [users[2]], animals):
        create_request = user_pb2.AnimalCreate(**animal)

        token = get_token(usr['email'], usr['password'], channel=channel)
        create_response = stub.CreateAnimal(create_request, metadata=[("authorization", f"Bearer {token}")])
        ids.append(create_response.id)
        print("AnimalCreate Response:\n", create_response)

    with open('animal_ids.txt', 'w') as f:
        f.writelines(i + '\n' for i in ids)
