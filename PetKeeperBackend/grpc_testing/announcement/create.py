import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
from address.create import addresses, get_address_ids
from animal.create import animals, get_animal_ids
import os
from datetime import datetime
DATE_FORMAT = "%Y-%m-%d %H:%M:%S"
script_dir = os.path.dirname(os.path.abspath(__file__))

user_ids = get_user_ids()
animal_ids = get_animal_ids()
primary_address_ids, secondary_address_ids = get_address_ids()

channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AnnouncementServiceStub(channel)

announcements = [
    {
        'animal_id': animal_ids[3],  # Animal of user[1]
        'keeper_profit': 400,
        'is_negotiable': True,
        'description': 'Grubiutki inżynier potrzebuje opieki na tydzień',
        'start_term': int(datetime.strptime("2025-02-14 12:00:00", DATE_FORMAT).timestamp()),
        'end_term': int(datetime.strptime("2025-02-21 15:00:00", DATE_FORMAT).timestamp()),
        'address_id': primary_address_ids[1]
    },
    {
        # 'author_id': user_ids[0],
        'animal_id': animal_ids[2],  # Animal of user[2]
        'keeper_profit': 100,
        'is_negotiable': False,
        'description': 'Szukam krótkoterminowej opieki nad moim kotkiem. Bardzo nie lubi być samemu w domu',
        'start_term': int(datetime.strptime("2025-02-13 16:00:00", DATE_FORMAT).timestamp()),
        'end_term': int(datetime.strptime("2025-02-13 23:30:00", DATE_FORMAT).timestamp()),
        'address_id': primary_address_ids[2]
    }
]


def get_announcement_ids():
    with open(os.path.join(script_dir, 'announcement_ids.txt'), 'r') as f:
        return [line.strip() for line in f.readlines()]


if __name__ == "__main__":
    ids = []
    for usr_id, usr, announcement in zip(user_ids[1:], users[1:], announcements):
        create_request = user_pb2.AnnouncementCreate(**announcement)

        token = get_token(usr['email'], usr['password'], channel=channel)
        create_response = stub.CreateAnnouncement(create_request, metadata=[("authorization", f"Bearer {token}")])
        ids.append(create_response.id)
        print("AnnouncementCreate Response:\n", create_response)

    with open('announcement_ids.txt', 'w') as f:
        f.writelines(i + '\n' for i in ids)
