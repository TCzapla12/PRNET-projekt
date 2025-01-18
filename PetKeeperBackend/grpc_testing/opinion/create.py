import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
from announcement.create import get_announcement_ids
import os

announcement_ids = get_announcement_ids()
script_dir = os.path.dirname(os.path.abspath(__file__))
user_ids = get_user_ids()


channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.OpinionServiceStub(channel)

opinions = [
    {
        'announcement_id': announcement_ids[0],
        'keeper_id': user_ids[0],
        'rating': 10,
        'description': "Bardzo dobra opieka. Polecam allegrowicza!"
    }
]


def get_opinion_ids():
    with open(os.path.join(script_dir, 'opinion_ids.txt'), 'r') as f:
        return [line.strip() for line in f.readlines()]


if __name__ == "__main__":
    ids = []
    for usr, user_id, opinion in zip([users[1]], [user_ids[1]], opinions):
        create_request = user_pb2.OpinionCreate(**opinion)

        token = get_token(usr['email'], usr['password'], channel=channel)
        create_response = stub.CreateOpinion(create_request, metadata=[("authorization", f"Bearer {token}")])
        ids.append(create_response.id)
        print("OpinionCreate Response:\n", create_response)

    with open('opinion_ids.txt', 'w') as f:
        f.writelines(i + '\n' for i in ids)
