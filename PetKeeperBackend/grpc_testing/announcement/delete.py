import grpc
import user_pb2
import user_pb2_grpc
import sys
from auth.get import get_token
from user.create import users, get_user_ids
from address.create import addresses, get_address_ids
from animal.create import animals, get_animal_ids
from announcement.create import announcements, get_announcement_ids
from datetime import datetime
DATE_FORMAT = "%Y-%m-%d %H:%M:%S"

user_ids = get_user_ids()
animal_ids = get_animal_ids()
primary_address_ids, secondary_address_ids = get_address_ids()
announcement_ids = get_announcement_ids()

channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AnnouncementServiceStub(channel)

delete_requests = [
    user_pb2.AnnouncementMinimal(  # Attempt deleting other user's announcement (should fail)
        id=announcement_ids[1],  # Belongs to user[2]
    ),
    user_pb2.AnnouncementMinimal(  # Attempt deleting other user's announcement as admin
        id=announcement_ids[0],  # Belongs to user[1]
    )
]

for user, user_id, delete_request in zip(users[1:], user_ids[1:], delete_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    try:
        update_response = stub.UpdateAnnouncement(delete_request, metadata=[("authorization", f"Bearer {token}")])
        print("AnnouncementDelete Response:\n", update_response)
    except grpc.RpcError as e:
        print(f"Error: {e}", file=sys.stderr)
