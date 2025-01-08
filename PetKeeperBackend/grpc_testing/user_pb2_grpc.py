# Generated by the gRPC Python protocol compiler plugin. DO NOT EDIT!
"""Client and server classes corresponding to protobuf-defined services."""
import grpc
import warnings

import user_pb2 as user__pb2

GRPC_GENERATED_VERSION = '1.67.0'
GRPC_VERSION = grpc.__version__
_version_not_supported = False

try:
    from grpc._utilities import first_version_is_lower
    _version_not_supported = first_version_is_lower(GRPC_VERSION, GRPC_GENERATED_VERSION)
except ImportError:
    _version_not_supported = True

if _version_not_supported:
    raise RuntimeError(
        f'The grpc package installed is at version {GRPC_VERSION},'
        + f' but the generated code in user_pb2_grpc.py depends on'
        + f' grpcio>={GRPC_GENERATED_VERSION}.'
        + f' Please upgrade your grpc module to grpcio>={GRPC_GENERATED_VERSION}'
        + f' or downgrade your generated code using grpcio-tools<={GRPC_VERSION}.'
    )


class AuthServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.Authenticate = channel.unary_unary(
                '/AuthService/Authenticate',
                request_serializer=user__pb2.AuthRequest.SerializeToString,
                response_deserializer=user__pb2.AuthResponse.FromString,
                _registered_method=True)


class AuthServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def Authenticate(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_AuthServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'Authenticate': grpc.unary_unary_rpc_method_handler(
                    servicer.Authenticate,
                    request_deserializer=user__pb2.AuthRequest.FromString,
                    response_serializer=user__pb2.AuthResponse.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'AuthService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))
    server.add_registered_method_handlers('AuthService', rpc_method_handlers)


 # This class is part of an EXPERIMENTAL API.
class AuthService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def Authenticate(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AuthService/Authenticate',
            user__pb2.AuthRequest.SerializeToString,
            user__pb2.AuthResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)


class UserServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.CreateUser = channel.unary_unary(
                '/UserService/CreateUser',
                request_serializer=user__pb2.UserCreate.SerializeToString,
                response_deserializer=user__pb2.UserIdentifier.FromString,
                _registered_method=True)
        self.GetUser = channel.unary_unary(
                '/UserService/GetUser',
                request_serializer=user__pb2.UserGet.SerializeToString,
                response_deserializer=user__pb2.UserFull.FromString,
                _registered_method=True)
        self.UpdateUser = channel.unary_unary(
                '/UserService/UpdateUser',
                request_serializer=user__pb2.UserUpdate.SerializeToString,
                response_deserializer=user__pb2.UserUpdate.FromString,
                _registered_method=True)
        self.UpdateUserCredentials = channel.unary_unary(
                '/UserService/UpdateUserCredentials',
                request_serializer=user__pb2.UserUpdateCredentials.SerializeToString,
                response_deserializer=user__pb2.UserUpdate.FromString,
                _registered_method=True)
        self.DeleteUser = channel.unary_unary(
                '/UserService/DeleteUser',
                request_serializer=user__pb2.UserIdentifier.SerializeToString,
                response_deserializer=user__pb2.UserIdentifier.FromString,
                _registered_method=True)


class UserServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def CreateUser(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetUser(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def UpdateUser(self, request, context):
        """Only updated fields are returned
        """
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def UpdateUserCredentials(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DeleteUser(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_UserServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'CreateUser': grpc.unary_unary_rpc_method_handler(
                    servicer.CreateUser,
                    request_deserializer=user__pb2.UserCreate.FromString,
                    response_serializer=user__pb2.UserIdentifier.SerializeToString,
            ),
            'GetUser': grpc.unary_unary_rpc_method_handler(
                    servicer.GetUser,
                    request_deserializer=user__pb2.UserGet.FromString,
                    response_serializer=user__pb2.UserFull.SerializeToString,
            ),
            'UpdateUser': grpc.unary_unary_rpc_method_handler(
                    servicer.UpdateUser,
                    request_deserializer=user__pb2.UserUpdate.FromString,
                    response_serializer=user__pb2.UserUpdate.SerializeToString,
            ),
            'UpdateUserCredentials': grpc.unary_unary_rpc_method_handler(
                    servicer.UpdateUserCredentials,
                    request_deserializer=user__pb2.UserUpdateCredentials.FromString,
                    response_serializer=user__pb2.UserUpdate.SerializeToString,
            ),
            'DeleteUser': grpc.unary_unary_rpc_method_handler(
                    servicer.DeleteUser,
                    request_deserializer=user__pb2.UserIdentifier.FromString,
                    response_serializer=user__pb2.UserIdentifier.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'UserService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))
    server.add_registered_method_handlers('UserService', rpc_method_handlers)


 # This class is part of an EXPERIMENTAL API.
class UserService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def CreateUser(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/UserService/CreateUser',
            user__pb2.UserCreate.SerializeToString,
            user__pb2.UserIdentifier.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetUser(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/UserService/GetUser',
            user__pb2.UserGet.SerializeToString,
            user__pb2.UserFull.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def UpdateUser(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/UserService/UpdateUser',
            user__pb2.UserUpdate.SerializeToString,
            user__pb2.UserUpdate.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def UpdateUserCredentials(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/UserService/UpdateUserCredentials',
            user__pb2.UserUpdateCredentials.SerializeToString,
            user__pb2.UserUpdate.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def DeleteUser(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/UserService/DeleteUser',
            user__pb2.UserIdentifier.SerializeToString,
            user__pb2.UserIdentifier.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)


class AddressServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.CreateAddress = channel.unary_unary(
                '/AddressService/CreateAddress',
                request_serializer=user__pb2.AddressCreate.SerializeToString,
                response_deserializer=user__pb2.AddressMinimal.FromString,
                _registered_method=True)
        self.GetUserAddresses = channel.unary_unary(
                '/AddressService/GetUserAddresses',
                request_serializer=user__pb2.AddressGet.SerializeToString,
                response_deserializer=user__pb2.AddressList.FromString,
                _registered_method=True)
        self.GetAddresses = channel.unary_unary(
                '/AddressService/GetAddresses',
                request_serializer=user__pb2.AddressGet.SerializeToString,
                response_deserializer=user__pb2.AddressList.FromString,
                _registered_method=True)
        self.UpdateAddress = channel.unary_unary(
                '/AddressService/UpdateAddress',
                request_serializer=user__pb2.AddressUpdate.SerializeToString,
                response_deserializer=user__pb2.AddressUpdate.FromString,
                _registered_method=True)
        self.DeleteAddress = channel.unary_unary(
                '/AddressService/DeleteAddress',
                request_serializer=user__pb2.AddressMinimal.SerializeToString,
                response_deserializer=user__pb2.AddressMinimal.FromString,
                _registered_method=True)


class AddressServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def CreateAddress(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetUserAddresses(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetAddresses(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def UpdateAddress(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DeleteAddress(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_AddressServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'CreateAddress': grpc.unary_unary_rpc_method_handler(
                    servicer.CreateAddress,
                    request_deserializer=user__pb2.AddressCreate.FromString,
                    response_serializer=user__pb2.AddressMinimal.SerializeToString,
            ),
            'GetUserAddresses': grpc.unary_unary_rpc_method_handler(
                    servicer.GetUserAddresses,
                    request_deserializer=user__pb2.AddressGet.FromString,
                    response_serializer=user__pb2.AddressList.SerializeToString,
            ),
            'GetAddresses': grpc.unary_unary_rpc_method_handler(
                    servicer.GetAddresses,
                    request_deserializer=user__pb2.AddressGet.FromString,
                    response_serializer=user__pb2.AddressList.SerializeToString,
            ),
            'UpdateAddress': grpc.unary_unary_rpc_method_handler(
                    servicer.UpdateAddress,
                    request_deserializer=user__pb2.AddressUpdate.FromString,
                    response_serializer=user__pb2.AddressUpdate.SerializeToString,
            ),
            'DeleteAddress': grpc.unary_unary_rpc_method_handler(
                    servicer.DeleteAddress,
                    request_deserializer=user__pb2.AddressMinimal.FromString,
                    response_serializer=user__pb2.AddressMinimal.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'AddressService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))
    server.add_registered_method_handlers('AddressService', rpc_method_handlers)


 # This class is part of an EXPERIMENTAL API.
class AddressService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def CreateAddress(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AddressService/CreateAddress',
            user__pb2.AddressCreate.SerializeToString,
            user__pb2.AddressMinimal.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetUserAddresses(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AddressService/GetUserAddresses',
            user__pb2.AddressGet.SerializeToString,
            user__pb2.AddressList.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetAddresses(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AddressService/GetAddresses',
            user__pb2.AddressGet.SerializeToString,
            user__pb2.AddressList.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def UpdateAddress(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AddressService/UpdateAddress',
            user__pb2.AddressUpdate.SerializeToString,
            user__pb2.AddressUpdate.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def DeleteAddress(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AddressService/DeleteAddress',
            user__pb2.AddressMinimal.SerializeToString,
            user__pb2.AddressMinimal.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)


class AnimalServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.CreateAnimal = channel.unary_unary(
                '/AnimalService/CreateAnimal',
                request_serializer=user__pb2.AnimalCreate.SerializeToString,
                response_deserializer=user__pb2.AnimalMinimal.FromString,
                _registered_method=True)
        self.GetAnimals = channel.unary_unary(
                '/AnimalService/GetAnimals',
                request_serializer=user__pb2.AnimalGet.SerializeToString,
                response_deserializer=user__pb2.AnimalList.FromString,
                _registered_method=True)
        self.DeleteAnimal = channel.unary_unary(
                '/AnimalService/DeleteAnimal',
                request_serializer=user__pb2.AnimalMinimal.SerializeToString,
                response_deserializer=user__pb2.AnimalMinimal.FromString,
                _registered_method=True)
        self.UpdateAnimal = channel.unary_unary(
                '/AnimalService/UpdateAnimal',
                request_serializer=user__pb2.AnimalUpdate.SerializeToString,
                response_deserializer=user__pb2.AnimalUpdate.FromString,
                _registered_method=True)


class AnimalServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def CreateAnimal(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetAnimals(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DeleteAnimal(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def UpdateAnimal(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_AnimalServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'CreateAnimal': grpc.unary_unary_rpc_method_handler(
                    servicer.CreateAnimal,
                    request_deserializer=user__pb2.AnimalCreate.FromString,
                    response_serializer=user__pb2.AnimalMinimal.SerializeToString,
            ),
            'GetAnimals': grpc.unary_unary_rpc_method_handler(
                    servicer.GetAnimals,
                    request_deserializer=user__pb2.AnimalGet.FromString,
                    response_serializer=user__pb2.AnimalList.SerializeToString,
            ),
            'DeleteAnimal': grpc.unary_unary_rpc_method_handler(
                    servicer.DeleteAnimal,
                    request_deserializer=user__pb2.AnimalMinimal.FromString,
                    response_serializer=user__pb2.AnimalMinimal.SerializeToString,
            ),
            'UpdateAnimal': grpc.unary_unary_rpc_method_handler(
                    servicer.UpdateAnimal,
                    request_deserializer=user__pb2.AnimalUpdate.FromString,
                    response_serializer=user__pb2.AnimalUpdate.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'AnimalService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))
    server.add_registered_method_handlers('AnimalService', rpc_method_handlers)


 # This class is part of an EXPERIMENTAL API.
class AnimalService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def CreateAnimal(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AnimalService/CreateAnimal',
            user__pb2.AnimalCreate.SerializeToString,
            user__pb2.AnimalMinimal.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetAnimals(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AnimalService/GetAnimals',
            user__pb2.AnimalGet.SerializeToString,
            user__pb2.AnimalList.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def DeleteAnimal(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AnimalService/DeleteAnimal',
            user__pb2.AnimalMinimal.SerializeToString,
            user__pb2.AnimalMinimal.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def UpdateAnimal(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AnimalService/UpdateAnimal',
            user__pb2.AnimalUpdate.SerializeToString,
            user__pb2.AnimalUpdate.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)


class AnnouncementServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.CreateAnnouncement = channel.unary_unary(
                '/AnnouncementService/CreateAnnouncement',
                request_serializer=user__pb2.AnnouncementCreate.SerializeToString,
                response_deserializer=user__pb2.AnnouncementMinimal.FromString,
                _registered_method=True)
        self.GetAnnouncements = channel.unary_unary(
                '/AnnouncementService/GetAnnouncements',
                request_serializer=user__pb2.AnnouncementGet.SerializeToString,
                response_deserializer=user__pb2.AnnouncementList.FromString,
                _registered_method=True)
        self.UpdateAnnouncement = channel.unary_unary(
                '/AnnouncementService/UpdateAnnouncement',
                request_serializer=user__pb2.AnnouncementUpdate.SerializeToString,
                response_deserializer=user__pb2.AnnouncementUpdate.FromString,
                _registered_method=True)
        self.DeleteAnnouncement = channel.unary_unary(
                '/AnnouncementService/DeleteAnnouncement',
                request_serializer=user__pb2.AnnouncementMinimal.SerializeToString,
                response_deserializer=user__pb2.AnnouncementMinimal.FromString,
                _registered_method=True)


class AnnouncementServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def CreateAnnouncement(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetAnnouncements(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def UpdateAnnouncement(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DeleteAnnouncement(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_AnnouncementServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'CreateAnnouncement': grpc.unary_unary_rpc_method_handler(
                    servicer.CreateAnnouncement,
                    request_deserializer=user__pb2.AnnouncementCreate.FromString,
                    response_serializer=user__pb2.AnnouncementMinimal.SerializeToString,
            ),
            'GetAnnouncements': grpc.unary_unary_rpc_method_handler(
                    servicer.GetAnnouncements,
                    request_deserializer=user__pb2.AnnouncementGet.FromString,
                    response_serializer=user__pb2.AnnouncementList.SerializeToString,
            ),
            'UpdateAnnouncement': grpc.unary_unary_rpc_method_handler(
                    servicer.UpdateAnnouncement,
                    request_deserializer=user__pb2.AnnouncementUpdate.FromString,
                    response_serializer=user__pb2.AnnouncementUpdate.SerializeToString,
            ),
            'DeleteAnnouncement': grpc.unary_unary_rpc_method_handler(
                    servicer.DeleteAnnouncement,
                    request_deserializer=user__pb2.AnnouncementMinimal.FromString,
                    response_serializer=user__pb2.AnnouncementMinimal.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'AnnouncementService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))
    server.add_registered_method_handlers('AnnouncementService', rpc_method_handlers)


 # This class is part of an EXPERIMENTAL API.
class AnnouncementService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def CreateAnnouncement(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AnnouncementService/CreateAnnouncement',
            user__pb2.AnnouncementCreate.SerializeToString,
            user__pb2.AnnouncementMinimal.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetAnnouncements(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AnnouncementService/GetAnnouncements',
            user__pb2.AnnouncementGet.SerializeToString,
            user__pb2.AnnouncementList.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def UpdateAnnouncement(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AnnouncementService/UpdateAnnouncement',
            user__pb2.AnnouncementUpdate.SerializeToString,
            user__pb2.AnnouncementUpdate.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def DeleteAnnouncement(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/AnnouncementService/DeleteAnnouncement',
            user__pb2.AnnouncementMinimal.SerializeToString,
            user__pb2.AnnouncementMinimal.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)
