using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using ZEDAssignment.APIResponse;

namespace ZEDAssignment.ResponseParser
{
    public class Parser
    {
        public static Root ParseResponse(string text)
        {
            var Response = JsonUtility.FromJson<Root>(text);
            return Response;
        }
    }
}
