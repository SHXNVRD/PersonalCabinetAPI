using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Requests
{
    public record ActivateCardRequest(int CardNumber, string CardCode);
}