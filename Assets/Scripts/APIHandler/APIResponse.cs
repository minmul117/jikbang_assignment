using System;
using System.Collections.Generic;

namespace ZEDAssignment.APIResponse
{
    [Serializable]
    public class DongMeta
    {
        public int Bd_id;
        public string Dong;
        public int LandHeight;
    }

    [Serializable]
    public class RoomType
    {
        public List<string> CoordinatesBase64s;
        public RoomTypeMeta Meta;
    }

    [Serializable]
    public class RoomTypeMeta
    {
        public int RoomTypeId;
    }

    [Serializable]
    public class DongData
    {
        public List<RoomType> RoomTypes;
        public DongMeta Meta;
    }

    [Serializable]
    public class Root
    {
        public bool Success;
        public int Code;
        public List<DongData> Data;
    }
}