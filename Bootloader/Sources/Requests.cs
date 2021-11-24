using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Transceiver;
using static BootloaderDesktop.Bootloader.Types;

namespace BootloaderDesktop
{
    public partial class Bootloader
    {
        public class Requests
        {
            public const string REQ = "#RQBL:";
            public const string RES = "#RSBL:";
            public const string END = "\r";

            public static xRequestHandler Handler = new xRequestHandler();
            public static List<xResponse> Responses = new List<xResponse>();

            public Requests()
            {
                new Get();
                new Set();
                new Try();
            }

            public unsafe class Get
            {
                public unsafe class Result<TResponse> : xResponseResult where TResponse : unmanaged
                {
                    public TResponse* Response;

                    public override object SetContent(xContent content)
                    {
                        Content = content;
                        Response = (TResponse*)content.Obj;
                        return this;
                    }
                }

                public class Builder<TResponse, TAction> : xBuilderPacket<Result<TResponse>, TAction> where TResponse : unmanaged where TAction : unmanaged
                {
                    public Builder(xRequestHandler handler, TAction action) : base(Responses, handler, action)
                    {
                        response.Name = "Bootloader." + action;
                        response.Header = RES;

                        Name = "Bootloader." + action;
                        Header = REQ;
                        End = END;
                    }
                }

                public Get() { }

                public static Builder<FirmwareInfoT, EActions> FirmwareInfo = new Builder<FirmwareInfoT, EActions>(Handler, EActions.GET_INFO)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.FirmwareInfo.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };

                public static Builder<AppFlashInfoT, EActions> AppInfo = new Builder<AppFlashInfoT, EActions>(Handler, EActions.GET_APP_INFO)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.AppFlashInfo.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };

                public static Builder<StatusT, EActions> Status = new Builder<StatusT, EActions>(Handler, EActions.GET_STATUS)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.Status.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };
            }

            public unsafe class Set
            {
                public unsafe class Result<TResponse> : xResponseResult where TResponse : unmanaged
                {
                    public TResponse* Response;
                    public EErrors Error;

                    public override object SetContent(xContent content)
                    {
                        Content = content;

                        Error = *(EErrors*)content.Obj;
                        content.Obj += sizeof(EErrors);

                        Response = (TResponse*)content.Obj;
                        return this;
                    }
                }

                public unsafe class Result : xResponseResult
                {
                    public EErrors Error;

                    public override object SetContent(xContent content)
                    {
                        Content = content;

                        Error = *(EErrors*)content.Obj;
                        content.Obj += sizeof(EErrors);
                        return this;
                    }
                }

                public class Builder<TResponse, TAction, TRequest> : xBuilderPacket<Result<TResponse>, TAction, TRequest> where TResponse : unmanaged where TAction : unmanaged where TRequest : unmanaged
                {
                    public Builder(xRequestHandler handler, TAction action) : base(Responses, handler, action)
                    {
                        response.Name = "Bootloader." + action;
                        response.Header = RES;

                        Name = "Bootloader." + action;
                        Header = REQ;
                        End = END;
                    }
                }

                public class Builder<TResponse, TAction> : xBuilderPacket<Result<TResponse>, TAction> where TResponse : unmanaged where TAction : unmanaged
                {
                    public Builder(xRequestHandler handler, TAction action) : base(Responses, handler, action)
                    {
                        response.Name = "Bootloader." + action;
                        response.Header = RES;

                        Name = "Bootloader." + action;
                        Header = REQ;
                        End = END;
                    }
                }

                public Set() { }

                public static Builder<StatusT, EActions, ELockState> LockState = new Builder<StatusT, EActions, ELockState>(Handler, EActions.SET_LOCK_STATE)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.Status.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };
            }

            public unsafe class Try
            {
                public unsafe class Result<TResponse> : xResponseResult where TResponse : unmanaged
                {
                    public TResponse* Response;
                    public EErrors Error;

                    public override object SetContent(xContent content)
                    {
                        Content = content;

                        Error = *(EErrors*)content.Obj;
                        content.Obj += sizeof(EErrors);

                        Response = (TResponse*)content.Obj;
                        return this;
                    }
                }

                public unsafe class Result : xResponseResult
                {
                    public EErrors Error;

                    public override object SetContent(xContent content)
                    {
                        Content = content;

                        Error = *(EErrors*)content.Obj;
                        content.Obj += sizeof(EErrors);
                        return this;
                    }
                }

                public class Builder<TResponse, TAction, TRequest> : xBuilderPacket<Result<TResponse>, TAction, TRequest> where TResponse : unmanaged where TAction : unmanaged where TRequest : unmanaged
                {
                    public Builder(xRequestHandler handler, TAction action) : base(Responses, handler, action)
                    {
                        response.Name = "Bootloader." + action;
                        response.Header = RES;

                        Name = "Bootloader." + action;
                        Header = REQ;
                        End = END;
                    }

                    public virtual unsafe xRequest<Result<TResponse>> Prepare(TRequest request, IList<byte> data)
                    {
                        xRequest<Result<TResponse>> RequestPacket = new xRequest<Result<TResponse>>()
                        {
                            Response = new xResponse<Result<TResponse>, TAction>(null, Response),
                            Builder = this,
                            Tracer = Tracer,
                            Handler = RequestHandler
                        };

                        RequestInfoT info = new RequestInfoT
                        {
                            Action = (ushort)(object)Action,
                            Size = (ushort)(sizeof(TRequest) + (int)data?.Count)
                        };

                        List<byte> request_data = new List<byte>();

                        add_data(request_data, Header);
                        add_data(request_data, &info, sizeof(ResponseInfoT));
                        add_data(request_data, &request, sizeof(TRequest));
                        add_data(request_data, data);
                        add_data(request_data, End);

                        RequestPacket.Data = request_data.ToArray();
                        return RequestPacket;
                    }

                    public virtual unsafe xRequest<Result<TResponse>> Prepare(TRequest request, void* data, int data_size)
                    {
                        xRequest<Result<TResponse>> RequestPacket = new xRequest<Result<TResponse>>()
                        {
                            Response = new xResponse<Result<TResponse>, TAction>(null, Response),
                            Builder = this,
                            Tracer = Tracer,
                            Handler = RequestHandler
                        };

                        RequestInfoT info = new RequestInfoT
                        {
                            Action = (ushort)(object)Action,
                            Size = (ushort)(sizeof(TRequest) + data_size)
                        };

                        List<byte> request_data = new List<byte>();

                        add_data(request_data, Header);
                        add_data(request_data, &info, sizeof(ResponseInfoT));
                        add_data(request_data, &request, sizeof(TRequest));
                        add_data(request_data, data, data_size);
                        add_data(request_data, End);

                        RequestPacket.Data = request_data.ToArray();
                        return RequestPacket;
                    }

                    public virtual unsafe xRequest<Result<TResponse>> Prepare<TData>(TRequest request, TData data) where TData : unmanaged
                    {
                        xRequest<Result<TResponse>> RequestPacket = new xRequest<Result<TResponse>>()
                        {
                            Response = new xResponse<Result<TResponse>, TAction>(null, Response),
                            Builder = this,
                            Tracer = Tracer,
                            Handler = RequestHandler
                        };

                        RequestInfoT info = new RequestInfoT
                        {
                            Action = (ushort)(object)Action,
                            Size = (ushort)(sizeof(TRequest) + sizeof(TData))
                        };

                        List<byte> request_data = new List<byte>();

                        add_data(request_data, Header);
                        add_data(request_data, &info, sizeof(ResponseInfoT));
                        add_data(request_data, &request, sizeof(TRequest));
                        add_data(request_data, &data, sizeof(TData));
                        add_data(request_data, End);

                        RequestPacket.Data = request_data.ToArray();
                        return RequestPacket;
                    }
                }

                public class Builder<TResponse, TAction> : xBuilderPacket<Result<TResponse>, TAction> where TResponse : unmanaged where TAction : unmanaged
                {
                    public Builder(xRequestHandler handler, TAction action) : base(Responses, handler, action)
                    {
                        response.Name = "Bootloader." + action;
                        response.Header = RES;

                        Name = "Bootloader." + action;
                        Header = REQ;
                        End = END;
                    }
                }

                public Try() { }

                public static Builder<StatusT, EActions, RequestWriteT> Write = new Builder<StatusT, EActions, RequestWriteT>(Handler, EActions.TRY_WRITE)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.Status.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };

                public static Builder<StatusT, EActions, RequestEraseT> Erase = new Builder<StatusT, EActions, RequestEraseT>(Handler, EActions.TRY_ERASE)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.Status.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };

                public static Builder<AppFlashInfoT, EActions> JumpToMain = new Builder<AppFlashInfoT, EActions>(Handler, EActions.TRY_JUMP_TO_MAIN)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.AppFlashInfo.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };

                public static Builder<AppFlashInfoT, EActions> JumpToBoot = new Builder<AppFlashInfoT, EActions>(Handler, EActions.TRY_JUMP_TO_BOOT)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.AppFlashInfo.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };

                public static Builder<AppFlashInfoT, EActions> Reset = new Builder<AppFlashInfoT, EActions>(Handler, EActions.TRY_RESET)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.AppFlashInfo.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };

                public static Builder<FirmwareInfoT, EActions> UpdateInfo = new Builder<FirmwareInfoT, EActions>(Handler, EActions.TRY_UPDATE_INFO)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.FirmwareInfo.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };

                public static Builder<StatusT, EActions> ReadCrc = new Builder<StatusT, EActions>(Handler, EActions.TRY_READ_CRC)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.Status.Value = *result.Response;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };
            }
        }
    }
}
