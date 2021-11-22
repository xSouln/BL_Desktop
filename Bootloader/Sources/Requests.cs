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
            public const string REQ = "#REQ::";
            public const string RES = "#RES::";
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
                public unsafe class Result<TObject> : xResponseResult where TObject : unmanaged
                {
                    public TObject* Arg;

                    public override object SetContent(xContent content)
                    {
                        Content = content;
                        Arg = (TObject*)content.Obj;
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

                public static Builder<FlashInfoT, EActions> Info = new Builder<FlashInfoT, EActions>(Handler, EActions.GET_INFO)
                {
                    Response =
                    {
                        EventReceive = (response, result) =>
                        {
                            Bootloader.FlashInfo.Value = *result.Arg;
                            xTracer.Message("Response accept: " + response.Action);
                            return true;
                        }
                    },
                    Tracer = xTracer.Message
                };
            }

            public class Set
            {
                public Set() { }
            }

            public class Try
            {
                public Try() { }
            }
        }
    }
}
