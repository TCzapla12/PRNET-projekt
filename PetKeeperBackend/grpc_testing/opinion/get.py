import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from opinion.create import get_opinion_ids
from user.create import users
from datetime import datetime
DATE_FORMAT = "%Y-%m-%d %H:%M:%S"

opinion_ids = get_opinion_ids()

channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.OpinionServiceStub(channel)

# User context (admin or non-admin) doesn't matter in Get
get_requests = [
    user_pb2.OpinionGet(
        id=opinion_ids[0]
    )
]

token = get_token(users[0]['email'], users[0]['password'], channel=channel)
get_response = stub.GetOpinions(get_requests[0], metadata=[("authorization", f"Bearer {token}")])
print("GetOpinion Response:\n", get_response)
